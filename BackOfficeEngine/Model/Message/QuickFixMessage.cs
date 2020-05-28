using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using QuickFix;
using QuickFix.Fields;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Exceptions;
using FixHelper;
using BackOfficeEngine.Helper;

namespace BackOfficeEngine.Model
{
    public class QuickFixMessage : Message,IMessage
    {
        private const string UTCTimestampFormat = "yyyyMMdd-HH:mm:ss.fff";
        public ProtocolType protocolType { get; set; }
        public DateTime TimeStamp { get; set; }
        

        private void PreConstructorCommonWork()
        {
            this.protocolType = ProtocolType.Fix50sp2;
        }
        public QuickFixMessage() : base() 
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
        public static QuickFixMessage CreateRequestMessage(MessageEnums.MsgType msgType,ProtocolType protocolType)
        {
            
            Message msg = null;
            switch (msgType)
            {
                case MessageEnums.MsgType.New:
                    switch (protocolType)
                    {
                        case ProtocolType.Fix40:
                            msg = new QuickFix.FIX40.NewOrderSingle();
                            break;
                        case ProtocolType.Fix41:
                            msg = new QuickFix.FIX41.NewOrderSingle();
                            break;
                        case ProtocolType.Fix42:
                            msg = new QuickFix.FIX42.NewOrderSingle();
                            break;
                        case ProtocolType.Fix43:
                            msg = new QuickFix.FIX43.NewOrderSingle();
                            break;
                        case ProtocolType.Fix44:
                            msg = new QuickFix.FIX44.NewOrderSingle();
                            break;
                        case ProtocolType.Fix50:
                            msg = new QuickFix.FIX50.NewOrderSingle();
                            break;
                        case ProtocolType.Fix50sp2:
                            msg = new QuickFix.FIX50SP2.NewOrderSingle();
                            break;
                    }
                    break;
                case MessageEnums.MsgType.Replace:
                    switch (protocolType)
                    {
                        case ProtocolType.Fix40:
                            msg = new QuickFix.FIX40.OrderCancelReplaceRequest();
                            break;
                        case ProtocolType.Fix41:
                            msg = new QuickFix.FIX41.OrderCancelReplaceRequest();
                            break;
                        case ProtocolType.Fix42:
                            msg = new QuickFix.FIX42.OrderCancelReplaceRequest();
                            break;
                        case ProtocolType.Fix43:
                            msg = new QuickFix.FIX43.OrderCancelReplaceRequest();
                            break;
                        case ProtocolType.Fix44:
                            msg = new QuickFix.FIX44.OrderCancelReplaceRequest();
                            break;
                        case ProtocolType.Fix50:
                            msg = new QuickFix.FIX50.OrderCancelReplaceRequest();
                            break;
                        case ProtocolType.Fix50sp2:
                            msg = new QuickFix.FIX50SP2.OrderCancelReplaceRequest();
                            break;
                    }
                    break;
                case MessageEnums.MsgType.Cancel:
                    switch (protocolType)
                    {
                        case ProtocolType.Fix40:
                            msg = new QuickFix.FIX40.OrderCancelRequest();
                            break;
                        case ProtocolType.Fix41:
                            msg = new QuickFix.FIX41.OrderCancelRequest();
                            break;
                        case ProtocolType.Fix42:
                            msg = new QuickFix.FIX42.OrderCancelRequest();
                            break;
                        case ProtocolType.Fix43:
                            msg = new QuickFix.FIX43.OrderCancelRequest();
                            break;
                        case ProtocolType.Fix44:
                            msg = new QuickFix.FIX44.OrderCancelRequest();
                            break;
                        case ProtocolType.Fix50:
                            msg = new QuickFix.FIX50.OrderCancelRequest();
                            break;
                        case ProtocolType.Fix50sp2:
                            msg = new QuickFix.FIX50SP2.OrderCancelRequest();
                            break;
                    }
                    break;
            }
            if(msg == null)
            {
                throw new UnSupportedMessageType("Unsupported message type");
            }
#if ITXR
            SetField(msg, 100, "XIST");
            SetField(msg, 47, "A");
            SetField(msg, 21, "1");
            SetField(msg, 22, "4");
            SetField(msg, 15, "TRY");
            SetField(msg, 120, "TRY");
            SetField(msg, 109, "FIDESSA");
            SetField(msg, 50, "JPM02");
#endif
            return new QuickFixMessage(msg);
        }

        private static void SetField(Message msg,int tag,string value)
        {
            if (AllFixTags.GetInstance().headerTagToObjectMap.ContainsKey(tag))
            {
                msg.Header.SetField(new StringField(tag,value));
            }
            else
            {
                msg.SetField(new StringField(tag, value));
            }
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
            return DateTime.ParseExact(GetField(Tags.TransactTime),UTCTimestampFormat,CultureInfo.CurrentCulture);
        }

        public decimal GetLastPx()
        {
            return decimal.Parse(GetField(Tags.LastPx), CultureInfo.InvariantCulture);
        }

        public string GetGenericField(int field)
        {
            if (Header.IsSetField(field))
            {
                return Header.GetField(field);
            }
            else
            {
                return GetField(field);
            }            
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
            if (AllFixTags.GetInstance().headerTagToObjectMap.ContainsKey(field))
            {
                Header.SetField(new StringField(field, value));
            }
            else
            {
                SetField(new StringField(field, value));
            }            
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
            if (Header.IsSetField(field))
            {
                return true;
            }
            else
            {
                return IsSetField(field);
            }            
        }

        public bool IsSetLastPx()
        {
            return IsSetField(Tags.LastPx);
        }

        public DateTime GetSendingTime()
        {
            return DateTime.ParseExact(GetField(Tags.SendingTime),UTCTimestampFormat,CultureInfo.CurrentCulture);
        }

        public DateTime GetTransactTime()
        {
            return DateTime.ParseExact(GetField(Tags.TransactTime), UTCTimestampFormat, CultureInfo.CurrentCulture);
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

        public DateTime GetSendTime()
        {
            return DateTime.ParseExact(GetField(Tags.SendingTime),UTCTimestampFormat,CultureInfo.CurrentCulture);
        }

        public DateTime GetReceiveTime()
        {
            throw new NotImplementedException();
        }

        public void SetSendTime(DateTime time)
        {
            SetField(new SendingTime(time));
        }

        public void SetReceiveTime(DateTime time)
        {
            throw new NotImplementedException();
        }

        public decimal GetAvgPx()
        {
            return decimal.Parse(GetField(Tags.AvgPx),CultureInfo.InvariantCulture);
        }

        public void SetAvgPx(decimal value)
        {
            SetField(new AvgPx(value));
        }

        public bool IsSetAvgPx()
        {
            return IsSetField(Tags.AvgPx);
        }

        public MessageEnums.OrdStatus GetOrdStatus()
        {
            return FixOrdStatusConverter.Convert(GetField(Tags.OrdStatus)[0]);
        }

        public void SetOrdStatus(MessageEnums.OrdStatus value)
        {
            SetField(new QuickFix.Fields.OrdStatus(FixOrdStatusConverter.Convert(value)));
        }

        public bool IsSetOrdStatus()
        {
            return IsSetField(Tags.OrdStatus);
        }
    }
}
