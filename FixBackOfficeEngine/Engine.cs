using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using BackOfficeEngine.Model;
using BackOfficeEngine.Connection;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Helper.IdGenerator;
using BackOfficeEngine.Events;
using BackOfficeEngine.Bootstrap;

namespace BackOfficeEngine
{
    //singleton
    public class Engine : IConnectorSubscriber
    {

        #region internal fields 
        internal static string resourcePath;
        #endregion
        #region public

        public event InboundMessageEventHandler InboundMessageEvent;
        public event OnLogonEventHandler OnLogonEvent;
        public event OnLogoutEventHandler OnLogoutEvent;
        public event OnCreateSessionEventHandler OnCreateSessionEvent;
        #endregion

        #region public collections
        
        #endregion

        #region private
        private static Engine instance;
        private int updateInterval;
        private int dequeueAmountPerUpdate;
        private ConcurrentDictionary<string, Order> m_nonProtocolIDMap = new ConcurrentDictionary<string, Order>();
        private ConcurrentDictionary<string, PseudoOrder> m_nonProtocolPseudoIDMap = new ConcurrentDictionary<string, PseudoOrder>();
        private ConcurrentDictionary<string, Order> m_clOrdIDMap = new ConcurrentDictionary<string, Order>();
        private ConcurrentDictionary<string, string> m_clOrdID_To_nonProtocolPseudoIdMap = new ConcurrentDictionary<string, string>();
        private List<IConnector> m_connectors = new List<IConnector>();
        private ConcurrentQueue<IMessage> m_messageQueue = new ConcurrentQueue<IMessage>();
        #endregion


        private Engine(int updateIntervalMilli,string resourcePath)
        {
            this.updateInterval = updateIntervalMilli;
            this.dequeueAmountPerUpdate = updateIntervalMilli / 5;
            Engine.resourcePath = resourcePath;
            List<Order> orders;
            orders = EngineBootstrapper.Bootstrap();
            UpdateCollectionsFromDatabase(orders);
            new Thread(MessageDequeuer).Start();
        }

        private void UpdateCollectionsFromDatabase(List<Order> orders)
        {
            foreach(Order order in orders)
            {
                m_nonProtocolIDMap[order.NonProtocolID] = order;
                m_clOrdIDMap[order.ClOrdID] = order;
            }
        }

        public static Engine GetInstance()
        {
            if(instance == null)
            {
                throw new ArgumentException("Engine is not initialized yet. Call other overload of GetInstance static method.");
            }
            return instance;
        }

        public static Engine GetInstance(int updateIntervalMilli, string resourcePath)
        {
            if(instance == null)
            {
                instance = new Engine(updateIntervalMilli, resourcePath);
            }
            return instance;
        }

        public int NewConnection(string configFilePath, ProtocolType protocolType)
        {
            switch(protocolType)
            {
                case ProtocolType.Fix:
                    IConnector connector = QuickFixConnector.GetInstance(configFilePath);
                    connector.Subscribe(this);
                    m_connectors.Add(connector);
                    return m_connectors.Count - 1;
                default:
                    throw new NotImplementedException("Unimplemented protocol type : " + protocolType);
            }
        }

        public (IMessage,string) PrepareMessageNew(NewMessageParameters prms)
        {
            IMessage newOrderMessage;
            string nonProtocolId;
            nonProtocolId = NonProtocolIDGenerator.Instance.GetNextId();
            PseudoOrder order;
            (newOrderMessage, order) = PseudoOrder.CreateNewOrder(prms, nonProtocolId);
            m_nonProtocolPseudoIDMap[nonProtocolId] = order;
            m_clOrdID_To_nonProtocolPseudoIdMap[newOrderMessage.GetClOrdID()] = nonProtocolId;
            return (newOrderMessage,nonProtocolId);
        }

        public IMessage PrepareMessageReplace(ReplaceMessageParameters prms)
        {
            return m_nonProtocolPseudoIDMap[prms.nonProtocolID].PrepareReplaceMessage(prms);
        }

        public IMessage PrepareMessageCancel(CancelMessageParameters prms)
        {
            return m_nonProtocolPseudoIDMap[prms.nonProtocolID].PrepareCancelMessage(prms);
        }

        public (IMessage,string) SendMessageNew(NewMessageParameters prms,int connectorIndex)
        {
            IMessage newOrderMessage;
            Order order;
            string nonProtocolID = NonProtocolIDGenerator.Instance.GetNextId();
            (newOrderMessage,order) = Order.CreateNewOrder(prms, nonProtocolID);
            m_nonProtocolIDMap[nonProtocolID] = order;
            m_clOrdIDMap[newOrderMessage.GetClOrdID()] = order;
            m_connectors[connectorIndex].SendMsgOrderEntry(newOrderMessage);
            return (newOrderMessage, nonProtocolID);               
        }

        public IMessage SendMessageReplace(ReplaceMessageParameters prms,int connectorIndex)
        {
            IMessage replaceMessage = m_nonProtocolIDMap[prms.nonProtocolID].PrepareReplaceMessage(prms);
            m_connectors[connectorIndex].SendMsgOrderEntry(replaceMessage);
            m_messageQueue.Enqueue(replaceMessage);
            return replaceMessage;
        }

        public IMessage SendMessageCancel(CancelMessageParameters prms,int connectorIndex)
        {
            IMessage cancelMessage = m_nonProtocolIDMap[prms.nonProtocolID].PrepareCancelMessage(prms);
            m_connectors[connectorIndex].SendMsgOrderEntry(cancelMessage);
            m_messageQueue.Enqueue(cancelMessage);
            return cancelMessage;
        }

        public void SendMessage(IMessage msg,int connectorIndex)
        {
            m_messageQueue.Enqueue(msg);
            m_connectors[connectorIndex].SendMsgOrderEntry(msg);
        }

        public void Disconnect(int connectorIndex)
        {
            m_connectors[connectorIndex].Disconnect();
        }
        void IConnectorSubscriber.OnInboundMessage(IConnector connector, string sessionID, IMessage msg)
        {
            InboundMessageEvent?.Invoke(this, new InboundMessageEventArgs(msg));
            if (msg.IsSetClOrdID())
            {
                m_messageQueue.Enqueue(msg);
            }
        }

        void IConnectorSubscriber.OnLogon(IConnector connector, string sessionID)
        {
            OnLogonEvent?.Invoke(this, new OnLogonEventArgs(m_connectors.IndexOf(connector), sessionID));
        }

        void IConnectorSubscriber.OnLogout(IConnector connector, string sessionID)
        {
            OnLogoutEvent?.Invoke(this, new OnLogoutEventArgs(m_connectors.IndexOf(connector), sessionID));
        }

        void IConnectorSubscriber.OnCreateSession(IConnector connector, string sessionID)
        {
            OnCreateSessionEvent?.Invoke(this, new OnCreateSessionEventArgs(m_connectors.IndexOf(connector), sessionID));
        }

        private void MessageDequeuer()
        {
            while (true)
            {
                int dequeuedMsgCount = 0;
                while(dequeuedMsgCount < dequeueAmountPerUpdate && m_messageQueue.Count != 0)
                {
                    Console.WriteLine(m_messageQueue.Count);
                    if (m_messageQueue.TryDequeue(out IMessage msg))
                    {
                        Order order;
                        void ReplaceOrCancel()
                        {
                            order = m_clOrdIDMap[msg.GetOrigClOrdID()];
                            order.AddMessage(msg);
                            m_clOrdIDMap[msg.GetClOrdID()] = order;
                        }
                        switch (msg.GetMsgType())
                        {
                            case MsgType.New:
                                string nonProtocolID = m_clOrdID_To_nonProtocolPseudoIdMap[msg.GetClOrdID()];
                                order = new Order(msg, nonProtocolID);
                                m_nonProtocolIDMap[nonProtocolID] = order;
                                m_clOrdIDMap[msg.GetClOrdID()] = order;
                                break;
                            case MsgType.Replace:
                                ReplaceOrCancel();
                                break;
                            case MsgType.Cancel:
                                ReplaceOrCancel();
                                break;
                            default:
                                order = m_clOrdIDMap[msg.GetClOrdID()];
                                order.AddMessage(msg);
                                break;
                        }
                    }
                    dequeuedMsgCount++;
                }
                Thread.Sleep(updateInterval);
            }
        }

        
    }
}
