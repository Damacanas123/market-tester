using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Model;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.Helper;
using QuickFix;
using QuickFix.Fields;

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
        private BISTCredentialParams CredentialParams { get; set; }

        public List<IConnectorSubscriber> subscribers { get; }
        public string Name { get; set; }

        private QuickFixConnector() 
        {
            subscribers = new List<IConnectorSubscriber>();
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

        }

        void IApplication.FromApp(Message message, SessionID sessionID)
        {
            IMessage msg = new QuickFixMessage(message);
            msg.ReceiveTime = DateTime.Now;
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
        }

        void IApplication.ToApp(Message message, SessionID sessionId)
        {
            
        }


        public void SendMsgOrderEntry(IMessage msg)
        {
            //Session session = m_symbolMap[msg.GetSymbol()];
            Message quickFixMsg = new Message(msg.ToString());
            primarySession.Send(quickFixMsg);
        }

        public void SendMsgOrderEntry(string msg)
        {
            //Session session = m_symbolMap[msg.GetSymbol()];
            primarySession.Send(msg);
        }

        public void SendMsgOrderEntry(string msg,bool overrideSessionTags)
        {
            if (overrideSessionTags)
            {
                //Session session = m_symbolMap[msg.GetSymbol()];
                Message quickFixMsg = new Message(msg);
                primarySession.Send(quickFixMsg);
            }
            else
            {
                primarySession.Send(msg);
            }
        }
    }
}
