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
using BackOfficeEngine.Helper;
using BackOfficeEngine.Exceptions;

namespace BackOfficeEngine
{
    //singleton
    public class Engine : IConnectorSubscriber
    {

        #region internal fields 
        internal static string resourcePath;
        #endregion
        #region public
        public event OnLogonEventHandler OnLogonEvent;
        public event OnLogoutEventHandler OnLogoutEvent;
        public event OnCreateSessionEventHandler OnCreateSessionEvent;
        public event UnknownClOrdIDReceivedEventHandler UnknownClOrdIDReceivedEvent;
        public event OnSessionMessageRejectEventHandler SessionMessageRejectEvent;
        public event OnApplicationMessageRejectEventHandler ApplicationMessageRejectEvent;
        #endregion

        

        #region private
        private static Engine instance;
        private int updateInterval;
        private int dequeueAmountPerUpdate;
        private ConcurrentDictionary<string, PseudoOrder> m_nonProtocolPseudoIDMap = new ConcurrentDictionary<string, PseudoOrder>();
        private ConcurrentDictionary<string, string> m_clOrdID_To_nonProtocolPseudoIdMap = new ConcurrentDictionary<string, string>();
        private Dictionary<string,IConnector> m_connectors { get; set; } = new Dictionary<string,IConnector>();
        //tuple : (msg,connectorName)
        private ConcurrentQueue<(IMessage,string)> m_messageQueue = new ConcurrentQueue<(IMessage, string)>();
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
                Order.NonProtocolIDMap[order.NonProtocolID] = order;
                Order.ClOrdIDMap[order.ClOrdID] = order;
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

        public void CancelAllOrders()
        {
            foreach(Order order in Order.Orders)
            {
                if (order.OrdStatus == OrdStatus.New ||
                    order.OrdStatus == OrdStatus.PartialFilled)
                {
                    if (m_connectors.ContainsKey(order.ConnectorName))
                    {

                        IMessage msg = order.PrepareCancelMessage();
                        SendMessage(msg, order.ConnectorName);
                    }
                }                
            }
        }

        public string NewConnection(string configFilePath,ProtocolType protocolType)
        {
            if (m_connectors.ContainsKey(Util.GetFileNameWithoutExtensionFromFullPath(configFilePath)))
            {
                throw new Exception("A connection is already configured with the same name. Note that name is the file name without its extension");
            }
            switch(protocolType)
            {
                case ProtocolType.Fix50sp2:
                    IConnector connector = QuickFixConnector.GetInstance(configFilePath,this);
                    m_connectors[connector.Name] = connector;
                    return connector.Name;
                default:
                    throw new NotImplementedException("Unimplemented protocol type : " + protocolType);
            }
        }
        public void ConfigureConnection(string connectorName,string configFilePath)
        {
            m_connectors[connectorName].ConfigureConnection(configFilePath);
        }

        public void ConfigureConnection(string connectorName, string configFilePath,BISTCredentialParams credentialParams)
        {
            m_connectors[connectorName].ConfigureConnection(configFilePath,credentialParams);
        }

        public void Connect(string connectorName)
        {
            m_connectors[connectorName].Connect();
        }

        public (IMessage,string) PrepareMessageNew(NewMessageParameters prms,string connectorName)
        {
            if (!m_connectors.ContainsKey(connectorName))
            {
                throw new ConnectorNotPresentException(connectorName);
            }
            IMessage newOrderMessage;
            string nonProtocolId;
            nonProtocolId = NonProtocolIDGenerator.Instance.GetNextId();
            PseudoOrder order;
            (newOrderMessage, order) = PseudoOrder.CreateNewOrder(prms, nonProtocolId);
            m_nonProtocolPseudoIDMap[nonProtocolId] = order;
            m_clOrdID_To_nonProtocolPseudoIdMap[newOrderMessage.GetClOrdID()] = nonProtocolId;

            Order orderReal = new Order(newOrderMessage, nonProtocolId,connectorName);
            Order.NonProtocolIDMap[nonProtocolId] = orderReal;
            Order.ClOrdIDMap[newOrderMessage.GetClOrdID()] = orderReal;
            return (newOrderMessage,nonProtocolId);
        }

        public IMessage PrepareMessageReplace(ReplaceMessageParameters prms)
        {
            return m_nonProtocolPseudoIDMap[prms.nonProtocolID].PrepareReplaceMessage(prms);
        }

        public IMessage PrepareMessageCancel(CancelMessageParameters prms)
        {
            return m_nonProtocolPseudoIDMap[prms.nonProtocolID].PrepareCancelMessage();
        }



        public void SendMessageNew(NewMessageParameters prms,string connectorName)
        {
            if (m_connectors.ContainsKey(connectorName))
            {
                IMessage newOrderMessage;
                Order order;
                string nonProtocolID = NonProtocolIDGenerator.Instance.GetNextId();
                (newOrderMessage, order) = Order.CreateNewOrder(prms, nonProtocolID, connectorName);
                Order.NonProtocolIDMap[nonProtocolID] = order;
                Order.ClOrdIDMap[newOrderMessage.GetClOrdID()] = order;
                m_connectors[connectorName].SendMsgOrderEntry(newOrderMessage);
            }
        }

        public void SendMessageReplace(ReplaceMessageParameters prms,string connectorName)
        {
            if (m_connectors.ContainsKey(connectorName))
            {
                IMessage replaceMessage = Order.NonProtocolIDMap[prms.nonProtocolID].PrepareReplaceMessage(prms);
                m_connectors[connectorName].SendMsgOrderEntry(replaceMessage);
            }
        }

        public void SendMessageCancel(CancelMessageParameters prms,string connectorName)
        {
            if (m_connectors.ContainsKey(connectorName))
            {
                IMessage cancelMessage = Order.NonProtocolIDMap[prms.nonProtocolID].PrepareCancelMessage();
                m_connectors[connectorName].SendMsgOrderEntry(cancelMessage);                
            }
            
        }

        public void SendMessage(IMessage msg,string connectorName)
        {
            m_connectors[connectorName].SendMsgOrderEntry(msg);
        }

        public void SendMessage(IMessage msg, string connectorName,bool overrideSessionTags)
        {
            m_connectors[connectorName].SendMsgOrderEntry(msg,overrideSessionTags);
        }

        public void SendMessage(string fixMsg,string connectorName,bool overrideSessionTags)
        {
            m_connectors[connectorName].SendMsgOrderEntry(fixMsg, overrideSessionTags);
        }
        public void Disconnect(string connectorName)
        {
            m_connectors[connectorName].Disconnect();
        }
        

        void IConnectorSubscriber.OnLogon(IConnector connector, string sessionID)
        {
            OnLogonEvent?.Invoke(this, new OnLogonEventArgs(connector.Name, sessionID));
        }

        void IConnectorSubscriber.OnLogout(IConnector connector, string sessionID)
        {
            OnLogoutEvent?.Invoke(this, new OnLogoutEventArgs(connector.Name, sessionID));
        }

        void IConnectorSubscriber.OnCreateSession(IConnector connector, string sessionID)
        {
            OnCreateSessionEvent?.Invoke(this, new OnCreateSessionEventArgs(connector.Name, sessionID));
        }

        void IConnectorSubscriber.EnqueueMessage(IConnector connector, IMessage msg)
        {
            m_messageQueue.Enqueue((msg, connector.Name));
        }
        private void MessageDequeuer()
        {
            while (true)
            {
                int dequeuedMsgCount = 0;
                while(dequeuedMsgCount < dequeueAmountPerUpdate && m_messageQueue.Count != 0)
                {
                    Console.WriteLine(m_messageQueue.Count);
                    IMessage msg;
                    if (m_messageQueue.TryDequeue(out var tuple))
                    {
                        msg = tuple.Item1;
                        Order order;
                        void ReplaceOrCancel()
                        {
                            order = Order.ClOrdIDMap[msg.GetOrigClOrdID()];
                            order.AddMessage(msg);
                            Order.ClOrdIDMap[msg.GetClOrdID()] = order;
                        }
                        switch (msg.GetMsgType())
                        {
                            
                            case MsgType.New:
                                order = Order.ClOrdIDMap[msg.GetClOrdID()];
                                order.AddMessage(msg);
                                break;
                            case MsgType.Replace:
                                ReplaceOrCancel();
                                break;
                            case MsgType.Cancel:
                                ReplaceOrCancel();
                                break;
                            default:
                                if (!Order.ClOrdIDMap.ContainsKey(msg.GetClOrdID()))
                                {
                                    UnknownClOrdIDReceivedEvent?.Invoke(this, new UnknownClOrdIDReceivedEventArgs(msg));
                                    break;
                                }
                                order = Order.ClOrdIDMap[msg.GetClOrdID()];
                                order.AddMessage(msg);
                                break;
                        }
                    }
                    dequeuedMsgCount++;
                }
                Thread.Sleep(updateInterval);
            }
        }

        void IConnectorSubscriber.OnSessionMessageReject(IConnector connector, IMessage msg, MessageOrigin messageOrigin)
        {
            SessionMessageRejectEvent?.Invoke(this, new OnSessionMessageRejectEventArgs(msg, messageOrigin));
        }

        void IConnectorSubscriber.OnApplicationMessageReject(IConnector connector, IMessage msg, MessageOrigin messageOrigin)
        {
            ApplicationMessageRejectEvent?.Invoke(this, new OnApplicationMessageRejectEventArgs(msg, messageOrigin));
        }
    }
}
