using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Data.SQLite;


using FixHelper;

using BackOfficeEngine.Helper;
using BackOfficeEngine.Helper.IdGenerator;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.DB.SQLite;
using BackOfficeEngine.AppConstants;

namespace BackOfficeEngine.Model
{
    public class Order : BaseOrder,IDataBaseWritable
    {

        internal static ConcurrentDictionary<string, Order> ClOrdIDMap = new ConcurrentDictionary<string, Order>();
        internal static ConcurrentDictionary<string, Order> NonProtocolIDMap = new ConcurrentDictionary<string, Order>();
        private static readonly object OrdersLock = new object();
        public static ObservableCollectionEx<Order> Orders { get; } = new ObservableCollectionEx<Order>();

        #region static methods 
        public static void ClearOrders()
        {
            lock (OrdersLock)
            {
                foreach (Order order in Orders)
                {
                    Util.DeleteFile(order.MessagesFilePath);
                }
                Util.ClearFileLocks();
                using(SQLiteHandler handler = new SQLiteHandler())
                {
                    handler.Truncate(new Order());
                }
                Orders.Clear();
            }
        }

        public static void ExportOrders(string filePath)
        {
            lock (OrdersLock)
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {                
                    foreach (Order order in Orders)
                    {
                        sw.WriteLine(order.GetExportRepr());
                    }
                }
            }
        }

        public static void ImportOrders(string filePath)
        {
            lock (OrdersLock)
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Order order = new Order(line);
                        ClOrdIDMap[order.ClOrdID] = order;
                        NonProtocolIDMap[order.NonProtocolID] = order;
                        Orders.Add(order);
                    }
                }
            }
        }

        

        #endregion



        private decimal cumulativeQty = 0;
        public decimal CumulativeQty
        {
            get { return cumulativeQty; }
            private set { cumulativeQty = value; NotifyPropertyChanged(nameof(CumulativeQty)); }
        }
        private decimal lastQty = 0;
        public decimal LastQty
        {
            get { return lastQty; }
            private set { lastQty = value; NotifyPropertyChanged(nameof(LastQty)); }
        }
        private decimal lastPx = 0;
        public decimal LastPx
        {
            get { return lastPx; }
            private set { lastPx = value; NotifyPropertyChanged(nameof(LastPx)); }
        }
        private decimal avgPx = 0;
        public decimal AvgPx
        {
            get { return avgPx; }
            private set { avgPx = value; NotifyPropertyChanged(nameof(AvgPx)); }
        }
        private string date = Util.GetNowString();
        public string Date
        {
            get { return date; }
            private set
            {
                if(date != value)
                {
                    date = value;
                    NotifyPropertyChanged(nameof(Date));
                }
            }
        }
        private OrdStatus ordStatus = OrdStatus.InitialPending;
        public OrdStatus OrdStatus
        {
            get { return ordStatus; }
            private set { ordStatus = value; NotifyPropertyChanged(nameof(OrdStatus)); }
        }
        private bool isImported = false;
        public bool IsImported
        {
            get { return isImported; }
            private set
            {
                if ( value != isImported)
                {
                    isImported = value;
                    NotifyPropertyChanged(nameof(IsImported));
                }
            }
        }
        
        public string ConnectorName { get; set; }
        public object MessagesLock { get; set; } = new object();
        public ObservableCollection<IMessage> Messages { get; set; } = new ObservableCollection<IMessage>();

        private string MessagesFilePath { get { return CommonFolders.OrderMessagesBaseDir + NonProtocolID + ".fixmessages"; } }
        

        public string TableName 
        {
            get
            {
                return "Orders";
            }
        }
        public string DatabaseID
        {
            get
            {
                return NonProtocolID;
            }
        }

        private static Dictionary<string, TableField> fields = new Dictionary<string, TableField> {
            {nameof(DatabaseID), new TableField(nameof(DatabaseID),typeof(string),"PRIMARY KEY",18) },
            {nameof(NonProtocolID), new TableField(nameof(NonProtocolID),typeof(string),"",18) },
            {nameof(Price), new TableField(nameof(Price),typeof(string),"",30) },
            {nameof(OrderQty), new TableField(nameof(OrderQty),typeof(string),"",30) },
            {nameof(Account), new TableField(nameof(Account),typeof(string),"",30) },
            {nameof(Symbol), new TableField(nameof(Symbol),typeof(string),"",25) },
            {nameof(ClOrdID), new TableField(nameof(ClOrdID),typeof(string),"",30)},
            {nameof(OrigClOrdID), new TableField(nameof(OrigClOrdID),typeof(string),"",30) },
            {nameof(Side), new TableField(nameof(Side),typeof(string),"",10) },
            {nameof(OrdType), new TableField(nameof(OrdType),typeof(string),"",10) },
            {nameof(TimeInForce), new TableField(nameof(TimeInForce),typeof(string),"",10) },
            {nameof(protocolType), new TableField(nameof(protocolType),typeof(string),"",30) },
            {nameof(CumulativeQty), new TableField(nameof(CumulativeQty),typeof(string),"",30) },
            {nameof(LastPx), new TableField(nameof(LastPx),typeof(string),"",30) },
            {nameof(LastQty), new TableField(nameof(LastQty),typeof(string),"",30) },
            {nameof(AvgPx), new TableField(nameof(AvgPx),typeof(string),"",30) },
            {nameof(OrdStatus), new TableField(nameof(OrdStatus),typeof(string),"",30) },
            {nameof(Date), new TableField(nameof(Date),typeof(string),"",8) },
            {nameof(IsImported), new TableField(nameof(IsImported),typeof(bool),"",0) },
            {nameof(ConnectorName), new TableField(nameof(ConnectorName),typeof(string),"",60) }
                };

        public Dictionary<string,TableField> Fields
        {
            get
            {
                return fields;
            }
        }  

        public Dictionary<string, object> Values
        {
            get
            {
                return new Dictionary<string, object>
                {
                    {nameof(DatabaseID), DatabaseID },
                    {nameof(NonProtocolID),NonProtocolID },
                    {nameof(Price),Price },
                    {nameof(OrderQty),OrderQty },
                    {nameof(Account),Account },
                    {nameof(Symbol),Symbol },
                    {nameof(ClOrdID),ClOrdID },
                    {nameof(OrigClOrdID),OrigClOrdID },
                    {nameof(Side),Side },
                    {nameof(OrdType),OrdType },
                    {nameof(TimeInForce),TimeInForce },
                    {nameof(protocolType),protocolType },
                    {nameof(CumulativeQty),CumulativeQty },
                    {nameof(LastPx),LastPx },
                    {nameof(LastQty),LastQty },
                    {nameof(AvgPx),AvgPx },
                    {nameof(OrdStatus),OrdStatus },
                    {nameof(Date),Date },
                    {nameof(IsImported),IsImported },
                    {nameof(ConnectorName),ConnectorName }
                };
            }
        }

        private void ConstructorCommonWork()
        {
            lock (OrdersLock)
            {
                Orders.Add(this);
            }
            
        }
        internal Order(SQLiteDataReader reader)
        {
            try
            {
                NonProtocolID = reader[nameof(NonProtocolID)].ToString();
                Price = decimal.Parse(reader[nameof(Price)].ToString(), CultureInfo.CurrentCulture);
                OrderQty = decimal.Parse(reader[nameof(OrderQty)].ToString(), CultureInfo.CurrentCulture);
                Account = Account.GetInstance(reader[nameof(Account)].ToString());
                Symbol = reader[nameof(Symbol)].ToString();
                ClOrdID = reader[nameof(ClOrdID)].ToString();
                OrigClOrdID = reader[nameof(OrigClOrdID)].ToString();
                Side = new StringToEnumConverter<Side>().Convert(reader[nameof(Side)].ToString());
                OrdType = new StringToEnumConverter<OrdType>().Convert(reader[nameof(OrdType)].ToString());
                TimeInForce = new StringToEnumConverter<TimeInForce>().Convert(reader[nameof(TimeInForce)].ToString());
                protocolType = new StringToEnumConverter<ProtocolType>().Convert(reader[nameof(protocolType)].ToString());
                CumulativeQty = decimal.Parse(reader[nameof(CumulativeQty)].ToString(), CultureInfo.CurrentCulture);
                LastPx = decimal.Parse(reader[nameof(LastPx)].ToString(), CultureInfo.CurrentCulture);
                LastQty = decimal.Parse(reader[nameof(LastQty)].ToString(), CultureInfo.CurrentCulture);
                AvgPx = decimal.Parse(reader[nameof(AvgPx)].ToString(), CultureInfo.CurrentCulture);
                OrdStatus = new StringToEnumConverter<OrdStatus>().Convert(reader[nameof(OrdStatus)].ToString());
                Date = reader[nameof(Date)].ToString();
                IsImported = reader[nameof(IsImported)].ToString() == "0" ? false : true;
                ConnectorName = reader[nameof(ConnectorName)].ToString();
                LoadMessages();
                ConstructorCommonWork();
            }
            catch(Exception ex)
            {
                Util.LogError(ex);
            }
        }

        internal Order(IMessage newOrderMessage, string nonProtocolID,string connectorName) : base(newOrderMessage, nonProtocolID)
        {
            ConstructorCommonWork();
            ConnectorName = connectorName;
            using (SQLiteHandler handler = new SQLiteHandler())
            {
                handler.Insert(this);
            }
        }

        

        /// <summary>
        /// dummy constructor for database writes should not be considered for application logic
        /// </summary>
        internal Order() { }

        protected Order(BaseOrder other,string connectorName) : base(other)
        {
            ConnectorName = connectorName;
            using (SQLiteHandler handler = new SQLiteHandler())
            {
                handler.Insert(this);
            }
            ConstructorCommonWork();
        }
        internal static new(IMessage, Order) CreateNewOrder(NewMessageParameters prms, string nonProtocolPseudoID,string connectorName)
        {
            IMessage newOrderMessage;
            BaseOrder baseOrder;
            (newOrderMessage, baseOrder) = BaseOrder.CreateNewOrder(prms, nonProtocolPseudoID);
            Order order = new Order(baseOrder,connectorName);
            return (newOrderMessage, order);
        }
        

        

        

        internal void AddMessage(IMessage msg)
        {
            lock (MessagesLock)
            {
                Messages.Add(msg);
            }            
            Util.AppendStringToFile(MessagesFilePath, msg.ToString());
            switch (msg.GetMsgType())
            {
                case MsgType.PendingNew:
                    OrdStatus = OrdStatus.PendingNew;
                    break;
                case MsgType.PendingReplace:
                    OrdStatus = OrdStatus.PendingReplace;
                    break;
                case MsgType.PendingCancel:
                    OrdStatus = OrdStatus.PendingCancel;
                    break;
                case MsgType.AckNew:
                    OrdStatus = OrdStatus.New;
                    ClOrdID = msg.GetClOrdID();
                    break;
                case MsgType.AckReplace:
                    switch (cumulativeQty)
                    {
                        case 0:
                            ordStatus = OrdStatus.New;
                            break;
                        default:
                            ordStatus = OrdStatus.PartialFilled;
                            break;
                    }
                    OrderQty = msg.GetOrderQty();
                    Price = msg.GetPrice();
                    OrigClOrdID = msg.GetOrigClOrdID();
                    ClOrdID = msg.GetClOrdID();
                    break;
                case MsgType.AckCancel:
                    OrdStatus = OrdStatus.Canceled;
                    ClOrdID = msg.GetClOrdID();
                    break;
                case MsgType.New:
                    break;
                case MsgType.Replace:
                    break;
                case MsgType.Cancel:
                    break;
                case MsgType.Reject:
                    IMessage requestMsg = FindRequestOfReject(msg);
                    switch (requestMsg.GetMsgType())
                    {
                        case MsgType.New:
                            OrdStatus = OrdStatus.Rejected;
                            break;
                    }
                    break;
                case MsgType.Trade:
                    decimal lastShares = msg.GetLastQty();
                    decimal lastPx = msg.GetLastPx();
                    if (msg.IsSetLastPx())
                        LastPx = msg.GetLastPx();
                    if (msg.IsSetLastQty())
                        LastQty = msg.GetLastQty();
                    if (msg.IsSetOrdStatus())
                        OrdStatus = msg.GetOrdStatus();
                    if (msg.IsSetGenericField(QuickFix.Fields.Tags.CumQty))
                        CumulativeQty = decimal.Parse(msg.GetGenericField(QuickFix.Fields.Tags.CumQty),CultureInfo.InvariantCulture);
                    if (msg.IsSetAvgPx())
                        AvgPx = msg.GetAvgPx();
                    Account.AddTrade(new TradeParameters(Side, lastShares, lastPx, Symbol));
                    break;
            }
            if(msg.IsSetOrdStatus())
                OrdStatus = msg.GetOrdStatus();
            using(SQLiteHandler handler = new SQLiteHandler())
            {
                handler.Update(this);
            }
        }

        private IMessage FindRequestOfReject(IMessage rejectMsg)
        {
            string rejectClOrdID = rejectMsg.GetClOrdID();
            IMessage rejectMessage;
            lock (MessagesLock)
            {
                rejectMessage = Messages.First((o) => o.GetClOrdID() == rejectClOrdID);
            }
            return rejectMessage;
        }


        private void LoadMessages()
        {
            using (StreamReader sr = new StreamReader(MessagesFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        switch (protocolType)
                        {
                            case ProtocolType.Fix50sp2:
                                lock (MessagesLock)
                                {
                                    Messages.Add(new QuickFixMessage(line));
                                }
                                break;
                        }
                    }
                }
            }            
        }

        public string GetExportRepr()
        {
            string repr = "";
            repr += NonProtocolID + "|";
            repr += Price.ToString(CultureInfo.InvariantCulture) + "|";
            repr += OrderQty.ToString(CultureInfo.InvariantCulture) + "|";
            repr += Account.ToString() + "|";
            repr += Symbol + "|";
            repr += ClOrdID + "|";
            repr += OrigClOrdID + "|";
            repr += Side + "|";
            repr += OrdType + "|";
            repr += TimeInForce + "|";
            repr += protocolType + "|";
            repr += CumulativeQty.ToString(CultureInfo.InvariantCulture) + "|";
            repr += LastPx.ToString(CultureInfo.InvariantCulture) + "|";
            repr += LastQty.ToString(CultureInfo.InvariantCulture) + "|";
            repr += AvgPx.ToString(CultureInfo.InvariantCulture) + "|";
            repr += OrdStatus + "|";
            repr += date + "|";
            repr += ConnectorName + "|";
            lock (MessagesLock)
            {
                repr += Messages.Count.ToString(CultureInfo.InvariantCulture) + "|";
                foreach (IMessage msg in Messages)
                {
                    repr += msg.ToString() + "|";
                }
            }            
            return repr;
        }
        internal Order(string exportRepr)
        {
            string [] arguements = exportRepr.Split('|');
            this.NonProtocolID = NonProtocolIDGenerator.Instance.GetNextId();
            this.Price = decimal.Parse(arguements[1],CultureInfo.InvariantCulture);
            this.OrderQty = decimal.Parse(arguements[2], CultureInfo.InvariantCulture);
            this.Account = Account.GetInstance(arguements[3]);
            this.Symbol = arguements[4];
            this.ClOrdID = arguements[5];
            this.OrigClOrdID = arguements[6];
            if (Side.TryParse(arguements[7], out Side side))
                this.Side = side;
            if (OrdType.TryParse(arguements[8], out OrdType ordType))
                this.OrdType = ordType;
            if (TimeInForce.TryParse(arguements[9], out TimeInForce timeInForce))
                this.TimeInForce = timeInForce;
            if (ProtocolType.TryParse(arguements[10], out ProtocolType protocolType))
                this.protocolType = protocolType;
            this.CumulativeQty = decimal.Parse(arguements[11], CultureInfo.InvariantCulture);
            this.LastPx = decimal.Parse(arguements[12], CultureInfo.InvariantCulture);
            this.LastQty = decimal.Parse(arguements[13], CultureInfo.InvariantCulture);
            this.AvgPx = decimal.Parse(arguements[14], CultureInfo.InvariantCulture);
            if (OrdStatus.TryParse(arguements[15], out OrdStatus ordStatus))
                this.OrdStatus = ordStatus;
            Date = arguements[16];
            ConnectorName = arguements[17];
            IsImported = true;
            int messageCount = int.Parse(arguements[18], CultureInfo.InvariantCulture);
            string messages = "";
            for (int i = 0; i < messageCount;i++)
            {
                switch (protocolType)
                {
                    case ProtocolType.Fix50sp2:
                        IMessage msg = new QuickFixMessage(arguements[19 + i]);
                        lock (MessagesLock)
                        {
                            Messages.Add(msg);
                        }
                        
                        if (msg.GetMsgType() == MsgType.Trade)
                        {
                            Account.AddTrade(new TradeParameters(Side, msg.GetLastQty(), msg.GetLastPx(), Symbol));
                        }
                        messages += arguements[19 + i] + Environment.NewLine;
                        break;
                }                
            }
            Util.AppendStringToFile(MessagesFilePath, messages);
            using(SQLiteHandler handler = new SQLiteHandler())
            {
                handler.Insert(this);
            }
        }




    }



}
