using System;

using System.Globalization;
using FixHelper;
using BackOfficeEngine.GeneralBase;
using MarketTester.Helper;
using System.Diagnostics.Contracts;

using BackOfficeEngine.MessageEnums;
using BackOfficeEngine;

namespace MarketTester.Model.Scheduler
{
    public class SchedulerRawItem : BaseNotifier
    {
        private int delay = 0;

        public int Delay
        {
            get { return delay; }
            set
            {
                delay = value;
                NotifyPropertyChanged(nameof(Delay));
            }
        }
        //
        private string connectorName;

        public string ConnectorName
        {
            get { return connectorName; }
            set
            {
                connectorName = value;
                NotifyPropertyChanged(nameof(ConnectorName));
            }
        }

        private ProtocolType protocolType;

        public ProtocolType ProtocolType
        {
            get { return protocolType; }
            set
            {
                protocolType = value;
                NotifyPropertyChanged(nameof(ProtocolType));
            }
        }



        private string schedulerOrderID;
        public string SchedulerOrderID
        {
            get
            {
                return schedulerOrderID;
            }
            set
            {
                schedulerOrderID = value;
                NotifyPropertyChanged(nameof(SchedulerOrderID));
            }
        }
        //
        private MsgType msgType;

        public MsgType MsgType
        {
            get { return msgType; }
            set
            {
                msgType = value;
                NotifyPropertyChanged(nameof(MsgType));
            }
        }
        //
        private string account;

        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                NotifyPropertyChanged(nameof(Account));
            }
        }
        //
        private Side side;

        public Side Side
        {
            get { return side; }
            set
            {
                side = value;
                NotifyPropertyChanged(nameof(Side));
            }
        }
        //
        private OrdType ordType;

        public OrdType OrdType
        {
            get { return ordType; }
            set
            {
                ordType = value;
                NotifyPropertyChanged(nameof(OrdType));
            }
        }
        //
        private TimeInForce timeInForce;

        public TimeInForce TimeInForce
        {
            get { return timeInForce; }
            set
            {
                timeInForce = value;
                NotifyPropertyChanged(nameof(TimeInForce));
            }
        }
        //
        private decimal orderQty = -1m;

        public decimal OrderQty
        {
            get { return orderQty; }
            set
            {
                orderQty = value;
                NotifyPropertyChanged(nameof(OrderQty));
            }
        }
        //
        private string symbol;

        public string Symbol
        {
            get { return symbol; }
            set
            {
                symbol = value;
                NotifyPropertyChanged(nameof(Symbol));
            }
        }

        private DateTime expireDate;

        public DateTime ExpireDate
        {
            get { return expireDate; }
            set
            {
                expireDate = value;
                NotifyPropertyChanged(nameof(ExpireDate));
            }
        }
        //
        private decimal price = -1m;

        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                NotifyPropertyChanged(nameof(Price));
            }
        }
        
        private string allocID;

        public string AllocID
        {
            get { return allocID; }
            set
            {
                allocID = value;
                NotifyPropertyChanged(nameof(AllocID));
            }
        }



        
        

        public SchedulerRawItem()
        {
        }

        private const char ValueDelimiter = '|';

        public SchedulerRawItem(string pipe)
        {
            string []values = pipe.Split(ValueDelimiter);
            MsgType = FixMsgTypeConverter.Convert(values[0]);
            Account = values[1];
            Side = FixSideConverter.Convert(values[2]);
            OrdType = FixOrdTypeConverter.Convert(values[3]);
            TimeInForce = FixTimeInForceConverter.Convert(values[4]);
            OrderQty = decimal.Parse(values[5],CultureInfo.InvariantCulture);
            Symbol = values[6];
            ExpireDate = DateTime.ParseExact(values[7],Util.UTCTimestampFormat,CultureInfo.InvariantCulture);
            Price = decimal.Parse(values[8], CultureInfo.InvariantCulture);
            AllocID = values[9];
            Delay = Int32.Parse(values[10],CultureInfo.InvariantCulture);
            connectorName = values[11];
            SchedulerOrderID = values[12];
        }
        public override string ToString()
        {
            return MsgType + ValueDelimiter +
                Account + ValueDelimiter + 
                Side + ValueDelimiter + 
                OrdType + ValueDelimiter + 
                TimeInForce + ValueDelimiter +
                OrderQty.ToString(CultureInfo.InvariantCulture) + ValueDelimiter +
                Symbol + ValueDelimiter + 
                ExpireDate.ToString(Util.LocalMktDateFormat,CultureInfo.InvariantCulture) + ValueDelimiter +
                Price.ToString(CultureInfo.InvariantCulture) + ValueDelimiter +
                AllocID + ValueDelimiter +
                Delay.ToString(CultureInfo.InvariantCulture) + ValueDelimiter +
                connectorName + ValueDelimiter +
                SchedulerOrderID;
        }        
    }
}
