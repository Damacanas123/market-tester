﻿using System;
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
        private ConcurrentQueue<Message> m_messageQueue = new ConcurrentQueue<Message>();
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
                    while(m_messageQueue.TryDequeue(out Message m))
                    {
                        foreach(IConnectorSubscriber subscriber in subscribers)
                        {
                            subscriber.EnqueueMessage(this, new QuickFixMessage(m));
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
            if(message.Header.GetField(Tags.MsgType) == MsgType.REJECT)
            {
                IMessage msg = new QuickFixMessage(message);
                if (msg.GetMsgType() == MessageEnums.MsgType.Reject)
                {
                    foreach (IConnectorSubscriber subscriber in subscribers)
                    {
                        subscriber.OnApplicationMessageReject(this, msg, MessageEnums.MessageOrigin.Outbound);
                    }
                }
            }            
        }

        void IApplication.FromApp(Message message, SessionID sessionID)
        {
            IMessage msg = new QuickFixMessage(message);
            if (msg.GetMsgType() == MessageEnums.MsgType.Reject)
            {
                foreach(IConnectorSubscriber subscriber in subscribers)
                {
                    subscriber.OnApplicationMessageReject(this, msg, MessageEnums.MessageOrigin.Outbound);
                }
            }
            msg.ReceiveTime = DateTime.Now;
            if (message.IsSetField(Tags.ClOrdID))
            {
                m_messageQueue.Enqueue(message);
            }
            foreach (IConnectorSubscriber subscriber in subscribers)
            {
                subscriber.OnInboundMessage(this, sessionID.ToString(), msg);
            }
            if (sessionID.SessionQualifier == SessionQualifierRD)
            {
                if (message.Header.GetField(Tags.MsgType) == MsgType.SECURITYDEFINITION)
                {
                    m_symbolMap[message.GetField(Tags.Symbol)] = message.GetField(FixHelper.GeniumExtensionTags.PartitionId) == "1" ? primarySession : secondarySession;                    
                }
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
                        subscriber.OnApplicationMessageReject(this, msg, MessageEnums.MessageOrigin.Inbound);
                    }
                }
            }
        }

        void IApplication.ToApp(Message message, SessionID sessionId)
        {
            if ("DGF".Contains(message.Header.GetField(Tags.MsgType)))
            {
                m_messageQueue.Enqueue(message);
            }
            IMessage msg = new QuickFixMessage(message);
            if (msg.GetMsgType() == MessageEnums.MsgType.Reject)
            {
                foreach (IConnectorSubscriber subscriber in subscribers)
                {
                    subscriber.OnApplicationMessageReject(this, msg, MessageEnums.MessageOrigin.Inbound);
                }
            }
        }


        public void SendMsgOrderEntry(IMessage msg)
        {
            Message quickFixMsg = new Message(msg.ToString());
            if (m_symbolMap.TryGetValue(msg.GetSymbol(),out Session session))
            { 
                session.Send(quickFixMsg);
            }
            else
            {
                primarySession.Send(quickFixMsg);
            }
            
        }

        public void SendMsgOrderEntry(string msg)
        {
            if (m_symbolMap.TryGetValue(Util.GetTag(msg,"55"), out Session session))
            {
                session.Send(msg);
            }
            else
            {
                primarySession.Send(msg);
            }
        }

        public void SendMsgOrderEntry(string msg,bool overrideSessionTags)
        {
            if (m_symbolMap.TryGetValue(Util.GetTag(msg, "55"), out Session session))
            {

            }
            else
            {
                session = primarySession;
            }
            if (overrideSessionTags)
            {
                Message quickFixMsg = new Message(msg);
                session.Send(quickFixMsg);
            }
            else
            {
                session.Send(msg);
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
            session.Send(amr);   
        }
    }
}
