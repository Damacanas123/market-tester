using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Model;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.Helper;
using QuickFix;
using QuickFix.Fields;
using System.Threading;

namespace BackOfficeEngine.Connection
{
    internal class QuickFixConnector : IApplication, IConnector
    {
        private QuickFix.Transport.SocketInitiator m_initiator;
        private static Dictionary<string, IConnector> m_instances = new Dictionary<string, IConnector>();
        private Session primarySession;
        private Session secondarySession;
        private Session RDSession;
        private Session DC1Session;
        private Session DC2Session;
        private Dictionary<string, Session> m_symbolMap = new Dictionary<string, Session>();
        private const string SessionQualifierPrimary = "Primary";
        private const string SessionQualifierSecondary = "Secondary";
        private const string SessionQualifierRD = "RD";
        private const string SessionQualifierDC1 = "DC1";
        private const string SessionQualifierDC2 = "DC2";
        private ConcurrentQueue<(string,SessionID,DateTime)> m_messageQueue = new ConcurrentQueue<(string, SessionID, DateTime)>();
        private BISTCredentialParams CredentialParams { get; set; }

        public List<IConnectorSubscriber> subscribers { get; }
        public string Name { get; set; }

        private QuickFixConnector() 
        {
            subscribers = new List<IConnectorSubscriber>();
            //enqueue messages to subscribers.
            new Thread(() =>
            {
                while (true)
                {
                    
                    while(m_messageQueue.TryDequeue(out var triplet))
                    {
                        string m = triplet.Item1;
                        SessionID sessionID = triplet.Item2;
                        DateTime timeStamp = triplet.Item3;
                        foreach(IConnectorSubscriber subscriber in subscribers)
                        {
                            subscriber.OnMessage(m,this.Name, sessionID.ToString(),timeStamp);
                        }
                        if (Fix.IsSetTag(m,Tags.ClOrdID))
                        {
                            //in order not to enqueue messages that lack CLordId and OrigClOrdID that may be sent from free format scheduler
                            if (!Fix.IsSetTag(m, Tags.MsgType))
                                continue;
                            string msgType = Fix.GetTag(m,Tags.MsgType);
                            if (msgType == MsgType.ORDERCANCELREPLACEREQUEST || msgType == MsgType.ORDER_CANCEL_REQUEST)
                            {
                                if (!(Fix.IsSetTag(m, Tags.ClOrdID) && Fix.IsSetTag(m, Tags.OrigClOrdID)))
                                    continue;
                            }
                            IMessage msg = new QuickFixMessage(m);
                            msg.TimeStamp = timeStamp;
                            //commented out because string messages are now being used in result of a performance improvement
                            //if you ever want to use send time and receive times you have to think of a workaround
                            //this feature haven't been used anyway
                            //msg.SendTime = m.GetSendTime();
                            //msg.ReceiveTime = m.GetReceiveTime();
                            foreach (IConnectorSubscriber subscriber in subscribers)
                            {                                
                                subscriber.EnqueueMessageThatContainsClOrdID(this, msg);
                                if (msg.GetMsgType() == MessageEnums.MsgType.Reject)
                                {
                                    subscriber.OnApplicationMessageReject(this, msg, MessageEnums.MessageOrigin.Outbound);
                                }
                            }
                        }                        
                    }
                    Thread.Sleep(500);
                }
            }).Start();
        }

        internal static IConnector GetInstance(string configFilePath,IConnectorSubscriber subscriber)
        {
            if (m_instances.TryGetValue(configFilePath, out IConnector instance))
            {
                return instance;
            }
            else
            {
                
                instance = new QuickFixConnector();
                instance.subscribers.Add(subscriber);
                m_instances[configFilePath] = instance;
                instance.Name = Util.GetFileNameWithoutExtensionFromFullPath(configFilePath);
            }
            return instance;
        }





        void IConnector.ConfigureConnection(string configFilePath)
        {
            QuickFix.SessionSettings settings = new QuickFix.SessionSettings(configFilePath);
            QuickFix.FileStoreFactory storeFactory = new QuickFix.FileStoreFactory(settings);
            QuickFix.FileLogFactory logFactory = new QuickFix.FileLogFactory(settings);
            m_initiator = new QuickFix.Transport.SocketInitiator(this, storeFactory, settings, logFactory);
        }

        void IConnector.ConfigureConnection(string configFilePath,BISTCredentialParams credentialParams)
        {
            CredentialParams = credentialParams;
            QuickFix.SessionSettings settings = new QuickFix.SessionSettings(configFilePath);
            QuickFix.FileStoreFactory storeFactory = new QuickFix.FileStoreFactory(settings);
            QuickFix.FileLogFactory logFactory = new QuickFix.FileLogFactory(settings);
            m_initiator = new QuickFix.Transport.SocketInitiator(this, storeFactory, settings, logFactory);
        }

        void IConnector.Connect()
        {
            this.m_initiator.Start();
        }

        void IConnector.Disconnect()
        {
            this.m_initiator.Stop();
        }
        void IConnector.Subscribe(IConnectorSubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        void IApplication.FromAdmin(Message message, SessionID sessionID)
        {
            message.TimeStamp = DateTime.Now;
            m_messageQueue.Enqueue((message.ToString(), sessionID,message.TimeStamp));
            if (message.Header.GetField(Tags.MsgType) == MsgType.REJECT)
            {
                IMessage msg = new QuickFixMessage(message);
                foreach (IConnectorSubscriber subscriber in subscribers)
                {
                    subscriber.OnSessionMessageReject(this, msg, MessageEnums.MessageOrigin.Outbound);
                }
            }            
        }

        void IApplication.FromApp(Message message, SessionID sessionID)
        {
            message.TimeStamp = DateTime.Now;            
            m_messageQueue.Enqueue((message.ToString(),sessionID,message.TimeStamp));
            if (message.Header.GetField(Tags.MsgType) == MsgType.SECURITYDEFINITION)
            {
                m_symbolMap[message.GetField(Tags.Symbol)] = message.GetField(FixHelper.GeniumExtensionTags.PartitionId) == "1" ? primarySession : secondarySession;                    
            }
            
        }

        

        void IApplication.OnCreate(SessionID sessionID)
        {
            foreach(IConnectorSubscriber subscriber in subscribers)
            {
                subscriber.OnCreateSession(this, sessionID.ToString());
            }
        }

        void IApplication.OnLogon(SessionID sessionID)
        {
            Session session = Session.LookupSession(sessionID);
            switch(sessionID.SessionQualifier)
            {
                case SessionQualifierPrimary:
                    primarySession = session;
                    break;
                case SessionQualifierSecondary:
                    secondarySession = session;
                    break;
                case SessionQualifierRD:
                    RDSession = session;
                    SendApplicationMessageRequest(RDSession);
                    break;
                case SessionQualifierDC1:
                    DC1Session = session;
                    break;
                case SessionQualifierDC2:
                    DC2Session = session;
                    break;
            }
            foreach (IConnectorSubscriber subscriber in subscribers)
            {
                subscriber.OnLogon(this, sessionID.ToString());
            }
        }

        void IApplication.OnLogout(SessionID sessionID)
        {
            foreach (IConnectorSubscriber subscriber in subscribers)
            {
                subscriber.OnLogout(this, sessionID.ToString());
            }
        }

        void IApplication.ToAdmin(Message message, SessionID sessionID)
        {
            
            if(message.Header.GetField(Tags.MsgType) == MsgType.LOGON && CredentialParams != null)
            {
                message.SetField(new Username(CredentialParams.Username));
                message.SetField(new Password(CredentialParams.Password));
            }
            if (message.Header.GetField(Tags.MsgType) == MsgType.REJECT)
            {
                IMessage msg = new QuickFixMessage(message);
                if (msg.GetMsgType() == MessageEnums.MsgType.Reject)
                {
                    foreach (IConnectorSubscriber subscriber in subscribers)
                    {
                        subscriber.OnSessionMessageReject(this, msg, MessageEnums.MessageOrigin.Inbound);
                    }
                }
            }
            message.TimeStamp = DateTime.Now;
            m_messageQueue.Enqueue((message.ToString(), sessionID,message.TimeStamp));
        }

        void IApplication.ToApp(Message message, SessionID sessionId)
        {
            
            //comment ou for performance reasons for scheduler
            //IMessage msg = new QuickFixMessage(message);
            //if (msg.GetMsgType() == MessageEnums.MsgType.Reject)
            //{
            //    foreach (IConnectorSubscriber subscriber in subscribers)
            //    {
            //        subscriber.OnApplicationMessageReject(this, msg, MessageEnums.MessageOrigin.Inbound);
            //    }
            //}
            message.TimeStamp = DateTime.Now;
            m_messageQueue.Enqueue((message.ToString(), sessionId,message.TimeStamp));
        }
#if ITXR
        private void SetISINCode(IMessage msg)
        {
            if (Fix.ISINCodeMap.TryGetValue(msg.GetSymbol(),out string isinCode))
            {
                msg.SetGenericField(Tags.SecurityID, isinCode);
            }
        }

        private void SetISINCode(Message msg)
        {
            if (Fix.ISINCodeMap.TryGetValue(msg.GetField(Tags.Symbol), out string isinCode))
            {
                msg.SetField(new SecurityID(isinCode));
            }
        }
#endif
        public void SendMsgOrderEntry(IMessage msg)
        {
#if ITXR
            SetISINCode(msg);
#endif
            Message quickFixMsg = new Message(msg.ToString());
            if (m_symbolMap.TryGetValue(msg.GetSymbol(),out Session session))
            { 
                session?.Send(quickFixMsg);
            }
            else
            {
                primarySession?.Send(quickFixMsg);
            }
            
        }
        public void SendMsgOrderEntry(Message msg)
        {
#if ITXR
            SetISINCode(msg);
#endif
            if (msg.IsSetField(Tags.Symbol) && m_symbolMap.TryGetValue(msg.GetString(Tags.Symbol), out Session session))
            {

            }            
            else
            {
                session = primarySession;                
            }
            session?.Send(msg);

        }

        public void SendMsgOrderEntry(IMessage msg,bool overrideSessionTags)
        {
#if ITXR
            SetISINCode(msg);
#endif
            if (m_symbolMap.TryGetValue(msg.GetSymbol(), out Session session))
            {

            }
            else
            {
                session = primarySession;
            }
            if (overrideSessionTags)
            {
                Message quickFixMsg = new Message(msg.ToString());
                session?.Send(quickFixMsg);
            }
            else
            {
                session?.Send(msg.ToString());
            }

        }

        

        public void SendMsgOrderEntry(string msg,bool overrideSessionTags)
        {
            if (m_symbolMap.TryGetValue(Fix.GetTag(msg, "55"), out Session session))
            {

            }
            else
            {
                session = primarySession;
            }
            if (overrideSessionTags)
            {
                Message quickFixMsg = new Message(msg);
                quickFixMsg.SetField(new TransactTime(DateTime.Now));
                session?.Send(quickFixMsg);
            }
            else
            {
                session?.Send(msg);
                //if you are sending string directly they do not fall into ToApp callback.That's why you need to enqueue manually
                m_messageQueue.Enqueue((msg, session.SessionID,DateTime.Now));
            }
        }

        

        public void SendApplicationMessageRequest(Session session)
        {
            QuickFix.FIX50SP2.ApplicationMessageRequest amr = new QuickFix.FIX50SP2.ApplicationMessageRequest();
            amr.SetField(new ApplReqType(1));
            amr.SetField(new ApplReqID((1).ToString()));
            QuickFix.FIX50SP2.ApplicationMessageRequest.NoApplIDsGroup group = new QuickFix.FIX50SP2.ApplicationMessageRequest.NoApplIDsGroup();
            group.SetField(new RefApplID("R"));
            group.SetField(new ApplBegSeqNum(1));
            group.SetField(new ApplEndSeqNum(0));
            amr.AddGroup(group);
            session?.Send(amr);   
        }
    }
}
