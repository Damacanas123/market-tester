using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Helper.IdGenerator;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.GeneralBase;
namespace BackOfficeEngine.Model
{
    public class BaseOrder : BaseNotifier
    {
        protected string nonProtocolID = "";
        public string NonProtocolID
        {
            get { return nonProtocolID; }
            private set { nonProtocolID = value; NotifyPropertyChanged(nameof(NonProtocolID)); }
        }
        protected decimal price;
        public decimal Price
        {
            get { return price; }
            private set { price = value; NotifyPropertyChanged(nameof(Price)); }
        }
        protected decimal orderQty;
        public decimal OrderQty
        {
            get { return orderQty; }
            private set { orderQty = value; NotifyPropertyChanged(nameof(OrderQty)); }
        }
        protected Account account;
        public Account Account
        {
            get { return account; }
            private set { account = value; NotifyPropertyChanged(nameof(Account)); }
        }
        protected string symbol = "";
        public string Symbol
        {
            get { return symbol; }
            private set { symbol = value;NotifyPropertyChanged(nameof(Symbol)); }
        }
        protected string clOrdID = "";
        public string ClOrdID
        {
            get { return clOrdID; }
            private set { clOrdID = value;NotifyPropertyChanged(nameof(ClOrdID)); }
        }
        protected string origClOrdID = "";
        public string OrigClOrdID
        {
            get { return origClOrdID; }
            private set { origClOrdID = value; NotifyPropertyChanged(nameof(OrigClOrdID)); }
        }
        protected Side side;
        public Side Side
        {
            get { return side; }
            private set { side = value; NotifyPropertyChanged(nameof(Side)); }
        }
        protected OrdType ordType;
        public OrdType OrdType
        {
            get { return ordType; }
            private set { ordType = value; NotifyPropertyChanged(nameof(OrdType)); }
        }
        protected TimeInForce timeInForce;
        public TimeInForce TimeInForce
        {
            get { return timeInForce; }
            private set { timeInForce = value; NotifyPropertyChanged(nameof(TimeInForce)); }
        }
        internal ProtocolType protocolType;
        
        

        

        internal static (IMessage, BaseOrder) CreateNewOrder(NewMessageParameters prms, string nonProtocolPseudoID)
        {
            IMessage newOrderMessage;
            
            switch (prms.protocolType)
            {
                case ProtocolType.Fix:
                    newOrderMessage = QuickFixMessage.CreateRequestMessage(MsgType.New);
                    newOrderMessage.SetClOrdID(ClOrdIdGenerator.Instance.GetNextId());
                    newOrderMessage.SetAccount(prms.account);
                    newOrderMessage.SetOrderQty(prms.orderQty);
                    newOrderMessage.SetOrdType(prms.ordType);
                    newOrderMessage.SetSymbol(prms.symbol);
                    newOrderMessage.SetTimeInForce(prms.timeInForce);
                    newOrderMessage.SetSide(prms.side);
                    newOrderMessage.SetTransactTime(DateTime.Now);
                    
                    switch (prms.price)
                    {
                        case decimal.MaxValue:
                            break;
                        default:
                            newOrderMessage.SetPrice(prms.price);
                            break;
                    }
                    break;
                default:
                    throw new NotImplementedException("Unimplemented protocol type : " + prms.protocolType);
            }
            BaseOrder order = new BaseOrder(newOrderMessage, nonProtocolPseudoID);
            order.protocolType = prms.protocolType;
            return (newOrderMessage, order);
        }

        protected BaseOrder(IMessage newOrderMessage, string nonProtocolID)
        {
            this.nonProtocolID = nonProtocolID;
            account = Account.GetInstance(newOrderMessage.GetAccount());
            symbol = newOrderMessage.GetSymbol();
            orderQty = newOrderMessage.GetOrderQty();
            price = newOrderMessage.IsSetPrice() ? newOrderMessage.GetPrice() : 0;
            clOrdID = newOrderMessage.GetClOrdID();
            origClOrdID = clOrdID;
            side = newOrderMessage.GetSide();
            ordType = newOrderMessage.GetOrdType();
            timeInForce = newOrderMessage.GetTimeInForce();
        }

        protected BaseOrder(BaseOrder other)
        {
            this.nonProtocolID = other.nonProtocolID;
            account = other.account;
            symbol = other.symbol;
            orderQty = other.orderQty;
            price = other.price;
            clOrdID = other.clOrdID;
            origClOrdID = clOrdID;
            side = other.side;
            ordType = other.ordType;
            timeInForce = other.timeInForce;
        }

        protected BaseOrder() { }

        internal virtual IMessage PrepareReplaceMessage(ReplaceMessageParameters prms)
        {
            IMessage replaceRequest = null;
            switch (protocolType)
            {
                case ProtocolType.Fix:
                    replaceRequest = QuickFixMessage.CreateRequestMessage(MsgType.Replace);
                    replaceRequest.SetClOrdID(ClOrdIdGenerator.Instance.GetNextId());
                    replaceRequest.SetOrigClOrdID(clOrdID);
                    switch (prms.orderQty)
                    {
                        case decimal.MaxValue:
                            break;
                        default:
                            replaceRequest.SetOrderQty(prms.orderQty);
                            break;
                    }
                    switch (prms.price)
                    {
                        case decimal.MaxValue:
                            break;
                        default:
                            replaceRequest.SetPrice(prms.price);
                            break;
                    }
                    replaceRequest.SetSide(side);
                    replaceRequest.SetOrdType(ordType);
                    replaceRequest.SetTransactTime(DateTime.Now);
                    replaceRequest.SetTransactTime(DateTime.Now);
                    replaceRequest.SetAccount(account.name);
                    break;
            }
            return replaceRequest;
        }

        internal virtual IMessage PrepareCancelMessage(CancelMessageParameters prms)
        {
            IMessage cancelRequest = null;
            switch (protocolType)
            {
                case ProtocolType.Fix:
                    cancelRequest = QuickFixMessage.CreateRequestMessage(MsgType.Cancel);
                    cancelRequest.SetClOrdID(ClOrdIdGenerator.Instance.GetNextId());
                    cancelRequest.SetOrigClOrdID(clOrdID);
                    cancelRequest.SetSide(side);
                    //cancelRequest.SetOrdType(ordType);
                    cancelRequest.SetOrderQty(orderQty);
                    cancelRequest.SetTransactTime(DateTime.Now);
                    cancelRequest.SetAccount(account.name);
                    cancelRequest.SetSymbol(symbol);
                    break;
            }
            return cancelRequest;
        }
    }
}
