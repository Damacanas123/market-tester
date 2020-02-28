using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using QuickFix;
using QuickFix.Fields;
using BackOfficeEngine.MessageEnums;


namespace BackOfficeEngine.Model
{
    internal class QuickFixMessage : Message,IMessage
    {
        private const string UTCTimestamp = "yyyyMMdd-HH:mm:ss.fff";
        public ProtocolType protocolType { get; set; }
        private void PreConstructorCommonWork()
        {
            this.protocolType = ProtocolType.Fix;
        }
        internal QuickFixMessage() : base() 
        {
            PreConstructorCommonWork();
        }
        public QuickFixMessage(string msgstr) : base(msgstr) 
        {
            PreConstructorCommonWork();
        }
        public QuickFixMessage(Message src) : base(src) 
        {
            PreConstructorCommonWork();
        }
        public QuickFixMessage(string msgstr, bool validate) : base(msgstr, validate)
        {
            PreConstructorCommonWork();
        }
        public static QuickFixMessage CreateRequestMessage(MessageEnums.MsgType msgType)
        {
            
            Message msg;
            switch (msgType)
            {
                case MessageEnums.MsgType.New:
                    msg = new QuickFix.FIX50SP2.NewOrderSingle();
                    break;
                case MessageEnums.MsgType.Replace:
                    msg = new QuickFix.FIX50SP2.OrderCancelReplaceRequest();
                    break;
                case MessageEnums.MsgType.Cancel:
                    msg = new QuickFix.FIX50SP2.OrderCancelRequest();
                    break;
                default:
                    throw new ArgumentException("Unsupported message type");
            }
            return new QuickFixMessage(msg);
        }

        
        public MessageEnums.MsgType GetMsgType()
        {
            string execType = null;
            if (IsSetField(Tags.ExecType))
            {
                execType = GetField(Tags.ExecType);
            }
            return FixMsgTypeConverter.Convert(Header.GetField(Tags.MsgType), execType);
        }
        public string GetClOrdID()
        {
            return GetField(Tags.ClOrdID);
        }

        public string GetOrigClOrdID()
        {
            return GetField(Tags.OrigClOrdID);
        }

        public string GetAccount()
        {
            return GetField(Tags.Account);
        }

        public MessageEnums.TimeInForce GetTimeInForce()
        {
            string timeInForce = GetField(Tags.TimeInForce);
            return FixTimeInForceConverter.Convert(timeInForce);

        }

        public bool IsOffHours()
        {
            if (!IsSetField(Tags.NoTradingSessions))
            {
                return false;
            }
            Group offHoursGroup = new Group(Tags.NoTradingSessions, 0);
            GetGroup(1, offHoursGroup);
            return "A" == offHoursGroup.GetField(Tags.TradingSessionID);
        }

        public decimal GetOrderQty()
        {
            return decimal.Parse(GetField(Tags.OrderQty), CultureInfo.InvariantCulture);
        }

        public decimal GetPrice()
        {
            return decimal.Parse(GetField(Tags.Price), CultureInfo.InvariantCulture);
        }

        public string GetSymbol()
        {
            return GetField(Tags.Symbol);
        }

        public MessageEnums.OrdType GetOrdType()
        {
            return FixOrdTypeConverter.Convert(GetField(Tags.OrdType));
        }

        public MessageEnums.Side GetSide()
        {
            return FixSideConverter.Convert(GetField(Tags.Side));
        }

        public DateTime GetExecutionTime() 
        {
            return DateTime.ParseExact(GetField(Tags.TransactTime),UTCTimestamp,CultureInfo.InvariantCulture);
        }

        public decimal GetLastPx()
        {
            return decimal.Parse(GetField(Tags.LastPx), CultureInfo.InvariantCulture);
        }

        public string GetGenericField(int field)
        {
            return GetField(field);
        }

        public void SetClOrdID(string value)
        {
            SetField(new ClOrdID(value));
        }

        public void SetOrigClOrdID(string value)
        {
            SetField(new OrigClOrdID(value));
        }

        public void SetAccount(string value)
        {
            SetField(new QuickFix.Fields.Account(value));
        }

        public void SetTimeInForce(MessageEnums.TimeInForce value)
        {
            SetField(new QuickFix.Fields.TimeInForce(FixTimeInForceConverter.Convert(value)));
        }

        public void SetTimeInForce(string value)
        {
            SetField(new QuickFix.Fields.TimeInForce(value[0]));
        }

        public void SetOffHours(bool value)
        {
            if (value)
            {
                SetOffHoursTrue();
            }
            else
            {
                SetOffHoursFalse();
            }
        }
        private void SetOffHoursTrue()
        {
            if (!IsSetField(Tags.NoTradingSessions))
            {
                var tradingSessionsGroup = new QuickFix.FIX50SP2.NewOrderSingle.NoTradingSessionsGroup();
                tradingSessionsGroup.SetField(new TradingSessionID("A"));
                AddGroup(tradingSessionsGroup);
            }
            else
            {
                Group offHoursGroup = new Group(Tags.NoTradingSessions, 0);
                GetGroup(1, offHoursGroup);
                offHoursGroup.SetField(new TradingSessionID("A"));
            }
        }

        private void SetOffHoursFalse()
        {
            if (IsSetField(Tags.NoTradingSessions))
            {
                RemoveGroup(1, Tags.NoTradingSessions);
            }
        }

        public void SetOrderQty(decimal value)
        {
            SetField(new OrderQty((decimal)value));
        }

        public void SetPrice(decimal value)
        {
            SetField(new Price((decimal)value));
        }

        public void SetSymbol(string value)
        {
            SetField(new Symbol(value));
        }

        public void SetOrdType(MessageEnums.OrdType value)
        {
            SetField(new QuickFix.Fields.OrdType(FixOrdTypeConverter.Convert(value)));
        }

        public void SetSide(MessageEnums.Side value)
        {
            SetField(new QuickFix.Fields.Side(FixSideConverter.Convert(value)));
        }

        public void SetLastPx(decimal value)
        {
            SetField(new LastPx((decimal)value));
        }

        public void SetGenericField(int field, string value)
        {
            SetField(new StringField(field, value));
        }

        public bool IsSetClOrdID()
        {
            return IsSetField(Tags.ClOrdID);
        }

        public bool IsSetOrigClOrdID()
        {
            return IsSetField(Tags.OrigClOrdID);
        }

        public bool IsSetAccount()
        {
            return IsSetField(Tags.Account);
        }

        public bool IsSetTimeInForce()
        {
            return IsSetField(Tags.TimeInForce);
        }

        public bool IsSetOrderQty()
        {
            return IsSetField(Tags.OrderQty);
        }

        public bool IsSetPrice()
        {
            return IsSetField(Tags.Price);
        }

        public bool IsSetSymbol()
        {
            return IsSetField(Tags.Symbol);
        }

        public bool IsSetOrdType()
        {
            return IsSetField(Tags.OrdType);
        }

        public bool IsSetSide()
        {
            return IsSetField(Tags.Side);
        }

        public bool IsSetExecutionTime()
        {
            return IsSetField(Tags.TransactTime);
        }

        public bool IsSetGenericField(int field)
        {
            return IsSetField(field);
        }

        public bool IsSetLastPx()
        {
            return IsSetField(Tags.LastPx);
        }

        public DateTime GetSendingTime()
        {
            return DateTime.ParseExact(GetField(Tags.SendingTime),UTCTimestamp,CultureInfo.InvariantCulture);
        }

        public DateTime GetTransactTime()
        {
            return DateTime.ParseExact(GetField(Tags.TransactTime), UTCTimestamp, CultureInfo.InvariantCulture);
        }

        public void SetSendingTime(DateTime timestamp)
        {
            SetField(new SendingTime(timestamp));
        }

        public void SetTransactTime(DateTime timestamp)
        {
            SetField(new TransactTime(timestamp));
        }

        public bool IsSetSendingTime()
        {
            return IsSetField(Tags.SendingTime);
        }

        public bool IsSetTransactTime()
        {
            return IsSetField(Tags.TransactTime);
        }

        public decimal GetLastQty()
        {
            return Decimal.Parse(GetField(Tags.LastQty),CultureInfo.InvariantCulture);
        }

        public void SetLastQty(decimal value)
        {
            SetField(new LastQty(value));
        }

        public bool IsSetLastQty()
        {
            return IsSetField(Tags.LastQty);
        }
    }
}
