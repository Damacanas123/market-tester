using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix.Fields;

namespace BackOfficeEngine.MessageEnums
{
    public enum MessageOrigin
    {
        Outbound,Inbound
    }
    public enum TimeInForce
    {
        UNKNOWN,FillOrKill, ImmediateOrCancel, Day,GoodTillCancel,GoodTillDate,GoodTillSession
    }
    public enum OrdType
    {
        UNKNOWN, Limit,Market,MarketToLimit
    }
    public enum Side
    {
        UNKNOWN, Buy,Sell
    }
    public enum MsgType
    {
        UNKNOWN, New, Replace, Cancel, AckNew,AckReplace,AckCancel, Reject, Trade,PendingNew,PendingReplace,PendingCancel
    }
    public enum OrdStatus
    {
        UNKNOWN, InitialPending, New,PendingNew,PendingReplace,PendingCancel,PartialFilled,Filled,Canceled,Rejected,Suspended
    }
    //reject in OrdStatus enum is a general reject. When a reject is received the corresponding request message has to be traced by ClOrdId and the logic for that reject 
    //should be implemented for each type seperately via an interface
    public class StringToEnumConverter<T>
    {
        public T Convert(string s)
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            return values.FirstOrDefault((o) => o.ToString() == s);
        }
    }
    public static class FixTimeInForceConverter
    {
        public static TimeInForce Convert(string value)
        {
            switch (value[0])
            {
                case QuickFix.Fields.TimeInForce.FILL_OR_KILL:
                    return TimeInForce.FillOrKill;
                case QuickFix.Fields.TimeInForce.IMMEDIATE_OR_CANCEL:
                    return TimeInForce.ImmediateOrCancel;
                case QuickFix.Fields.TimeInForce.DAY:
                    return TimeInForce.Day;
                case QuickFix.Fields.TimeInForce.GOOD_TILL_CANCEL:
                    return TimeInForce.GoodTillCancel;
                case QuickFix.Fields.TimeInForce.GOOD_TILL_DATE:
                    return TimeInForce.GoodTillDate;
                case 'S':
                    return TimeInForce.GoodTillSession;
                default:
                    return TimeInForce.UNKNOWN;
            }
        }
        public static char Convert(TimeInForce value)
        {
            switch (value)
            {
                case TimeInForce.FillOrKill:
                    return QuickFix.Fields.TimeInForce.FILL_OR_KILL;
                case TimeInForce.ImmediateOrCancel:
                    return QuickFix.Fields.TimeInForce.IMMEDIATE_OR_CANCEL;
                case TimeInForce.Day:
                    return QuickFix.Fields.TimeInForce.DAY;
                case TimeInForce.GoodTillCancel:
                    return QuickFix.Fields.TimeInForce.GOOD_TILL_CANCEL;
                case TimeInForce.GoodTillDate:
                    return QuickFix.Fields.TimeInForce.GOOD_TILL_DATE;
                case TimeInForce.GoodTillSession:
                    return 'S';
                default :
                    return 'U'; // U for TimeInForce.UNKNOWN
            }
        }
    }

    public static class FixOrdTypeConverter
    {
        public static OrdType Convert(string value)
        {
            switch (value[0])
            {
                case QuickFix.Fields.OrdType.LIMIT:
                    return OrdType.Limit;
                case QuickFix.Fields.OrdType.MARKET:
                    return OrdType.Market;
                case QuickFix.Fields.OrdType.MARKET_WITH_LEFTOVER_AS_LIMIT:
                    return OrdType.MarketToLimit;
                default:
                    return OrdType.UNKNOWN;
            }
        }
        public static char Convert(OrdType value)
        {
            switch (value)
            {
                case OrdType.Limit:
                    return QuickFix.Fields.OrdType.LIMIT;
                case OrdType.Market:
                    return QuickFix.Fields.OrdType.MARKET;
                case OrdType.MarketToLimit:
                    return QuickFix.Fields.OrdType.MARKET_WITH_LEFTOVER_AS_LIMIT;
                default:
                    return 'U'; // U for OrdType.UNKNOWN
            }
        }
    }

    public static class FixSideConverter
    {
        public static Side Convert(string value)
        {
            switch (value[0])
            {
                case QuickFix.Fields.Side.BUY:
                    return Side.Buy;
                case QuickFix.Fields.Side.SELL:
                    return Side.Sell;
                default:
                    return Side.UNKNOWN;
            }
        }
        public static char Convert(Side value)
        {
            switch (value)
            {
                case Side.Buy:
                    return QuickFix.Fields.Side.BUY;
                case Side.Sell:
                    return QuickFix.Fields.Side.SELL;
                default:
                    return 'U'; // U for Side.UNKNOWN
            }
        }
    }

    public static class FixMsgTypeConverter
    {
        public static MsgType Convert(string msgType,string execType = null)
        {
            switch (msgType)
            {
                case QuickFix.Fields.MsgType.NEWORDERSINGLE:
                    return MsgType.New;
                case QuickFix.Fields.MsgType.ORDERCANCELREPLACEREQUEST:
                    return MsgType.Replace;
                case QuickFix.Fields.MsgType.ORDERCANCELREQUEST:
                    return MsgType.Cancel;
                case QuickFix.Fields.MsgType.EXECUTIONREPORT:
                    switch (execType[0])
                    {
                        case ExecType.NEW:
                            return MsgType.AckNew;
                        case ExecType.REPLACE:
                            return MsgType.AckReplace;
                        case ExecType.CANCELLED:
                            return MsgType.AckCancel;
                        case ExecType.REJECTED:
                            return MsgType.Reject;
                        case ExecType.TRADE:
                            return MsgType.Trade;
                        case ExecType.PENDING_NEW:
                            return MsgType.PendingNew;
                        case ExecType.PENDING_REPLACE:
                            return MsgType.PendingReplace;
                        case ExecType.PENDING_CANCEL:
                            return MsgType.PendingCancel;
                        default:
                            return MsgType.UNKNOWN;
                    }
                case QuickFix.Fields.MsgType.ORDERCANCELREJECT:
                    return MsgType.Reject;
                default:
                    return MsgType.UNKNOWN;
            }
        }
        
    }

    public static class FixOrdStatusConverter
    {
        public static OrdStatus Convert(char ordStatus)
        {
            switch (ordStatus)
            {
                case QuickFix.Fields.OrdStatus.NEW:
                    return OrdStatus.New;
                case QuickFix.Fields.OrdStatus.PENDING_NEW:
                    return OrdStatus.PendingNew;
                case QuickFix.Fields.OrdStatus.PENDING_REPLACE:
                    return OrdStatus.PendingReplace;
                case QuickFix.Fields.OrdStatus.PENDING_CANCEL:
                    return OrdStatus.PendingCancel;
                case QuickFix.Fields.OrdStatus.PARTIALLY_FILLED:
                    return OrdStatus.PartialFilled;
                case QuickFix.Fields.OrdStatus.FILLED:
                    return OrdStatus.Filled;
                case QuickFix.Fields.OrdStatus.REJECTED:
                    return OrdStatus.Rejected;
                case QuickFix.Fields.OrdStatus.SUSPENDED:
                    return OrdStatus.Suspended;
                case QuickFix.Fields.OrdStatus.CANCELED:
                    return OrdStatus.Canceled;
                default:
                    return OrdStatus.UNKNOWN;
            }
        }
        public static char Convert(OrdStatus ordStatus)
        {
            switch (ordStatus)
            {
                case OrdStatus.New:
                    return QuickFix.Fields.OrdStatus.NEW;
                case OrdStatus.PendingNew:
                    return QuickFix.Fields.OrdStatus.PENDING_NEW;
                case OrdStatus.PendingReplace:
                    return QuickFix.Fields.OrdStatus.PENDING_REPLACE;
                case OrdStatus.PendingCancel:
                    return QuickFix.Fields.OrdStatus.PENDING_CANCEL;
                case OrdStatus.PartialFilled:
                    return QuickFix.Fields.OrdStatus.PARTIALLY_FILLED;
                case OrdStatus.Filled:
                    return QuickFix.Fields.OrdStatus.FILLED;
                case OrdStatus.Rejected:
                    return QuickFix.Fields.OrdStatus.REJECTED;
                case OrdStatus.Suspended:
                    return QuickFix.Fields.OrdStatus.SUSPENDED;
                case OrdStatus.Canceled:
                    return QuickFix.Fields.OrdStatus.CANCELED;
                default:
                    return 'U'; //U for unknown
            }
        }
    }

}
