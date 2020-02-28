using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Model;
using QuickFix;

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

        public List<IConnectorSubscriber> subscribers { get; }

        private QuickFixConnector() 
        {
            subscribers = new List<IConnectorSubscriber>();
        }

        internal static IConnector GetInstance(string configFilePath)
        {
            if (m_instances.TryGetValue(configFilePath, out IConnector instance))
            {
                return instance;
            }
            else
            {
                IConnector new_instance = new QuickFixConnector();
                new_instance.ConfigureConnection(configFilePath);
                new_instance.Connect();
                m_instances[configFilePath] = new_instance;
                return new_instance;
            }

        }





        void IConnector.ConfigureConnection(string configFilePath)
        {
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
            foreach (IConnectorSubscriber subscriber in subscribers)
            {
                subscriber.OnInboundMessage(this, sessionID.ToString(),msg);
            }
        }

        void IApplication.OnCreate(SessionID sessionID)
        {

        }

        void IApplication.OnLogon(SessionID sessionID)
        {
            Session session = Session.LookupSession(sessionID);
            switch(sessionID.SessionQualifier)
            {
                case "Primary":
                    primarySession = session;
                    break;
                case "Secondary":
                    secondarySession = session;
                    break;
                case "RD":
                    RDSession = session;
                    break;
                case "DC1":
                    DC1Session = session;
                    break;
                case "DC2":
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
        }

        void IApplication.ToApp(Message message, SessionID sessionId)
        {
            
        }

        public void SendMsgOrderEntry(IMessage msg)
        {
            primarySession.Send(new Message(msg.ToString()));
        }
    }
}
