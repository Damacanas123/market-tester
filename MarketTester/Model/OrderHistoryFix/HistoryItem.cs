using BackOfficeEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix.Fields;
using System.Globalization;
using BackOfficeEngine.GeneralBase;
using MarketTester.Worksheet;
using FixHelper;

namespace MarketTester.Model.OrderHistoryFix
{
    public class HistoryItem : BaseNotifier
    {
        public HistoryItem(IMessage msg,string clOrdID,string origClOrdID)
        {
            backingMsg = msg;
            ClOrdID = clOrdID;
            OrigClOrdID = origClOrdID;

        }
        private IMessage backingMsg { get; set; }
        public string MessageString
        {
            get { return backingMsg.ToString(); }
        }

        private TimeSpan normalizedTimeStamp;

        public TimeSpan NormalizedTimeStamp
        {
            get { return normalizedTimeStamp; }
            set
            {
                normalizedTimeStamp = value;
                NotifyPropertyChanged(nameof(NormalizedTimeStamp));
            }
        }

        private TimeSpan normalizedTimeDiff;

        public TimeSpan NormalizedTimeDiff
        {
            get { return normalizedTimeDiff; }
            set
            {
                normalizedTimeDiff = value;
                NotifyPropertyChanged(nameof(NormalizedTimeDiff));
            }
        }


        public DateTime TimeStamp { get; set; }


        public override string ToString()
        {
            return MessageString;
        }

        private bool isPreGenerated;

        public bool IsPreGenerated
        {
            get { return isPreGenerated; }
            set
            {
                isPreGenerated = value;
                NotifyPropertyChanged(nameof(IsPreGenerated));
            }
        }

        public string Origin
        {
            get
            {
                if (BackOfficeEngine.Helper.Fix.OrderEntryOutboundMessageTypes.Contains(backingMsg.GetMsgType()))
                {
                    return ExcelConstants.MESSAGE_ORIGIN_OUTBOUND;
                }
                else
                {
                    return ExcelConstants.MESSAGE_ORIGIN_INBOUND;
                }
            }
        }


        private string clOrdID;

        public string ClOrdID
        {
            get { return clOrdID; }
            set
            {
                clOrdID = value;
                NotifyPropertyChanged(nameof(ClOrdID));
            }
        }

        private string origClOrdID;

        public string OrigClOrdID
        {
            get { return origClOrdID; }
            set
            {
                origClOrdID = value;
                NotifyPropertyChanged(nameof(OrigClOrdID));
            }
        }


        public string MessageType
        {
            get { return FixHelper.FixValues.MsgTypesOrderEntry[backingMsg.GetGenericField(Tags.MsgType)]; }
        }

        public string OrderId
        {
            get { return backingMsg.IsSetGenericField(Tags.OrderID) ? backingMsg.GetGenericField(Tags.OrderID) : ""; }
        }

        public string ExecType
        {
            get 
            {
                string execType = "";
                if (backingMsg.IsSetGenericField(Tags.ExecType))
                {
                    execType = GetExecutionTypeString(backingMsg);
                }
                return execType; 
            }
        }

        public string ExecId
        {
            get { return backingMsg.IsSetGenericField(Tags.ExecID) ? backingMsg.GetGenericField(Tags.ExecID) : ""; }
        }

        public string OrdStatus
        {
            get { return backingMsg.IsSetOrdStatus() ? backingMsg.GetOrdStatus().ToString() : ""; }
        }

        public string OrderQty
        {
            get { return backingMsg.IsSetOrderQty() ? backingMsg.GetOrderQty().ToString(CultureInfo.InvariantCulture) : ""; }
        }

        public string Price
        {
            get { return backingMsg.IsSetPrice() ? backingMsg.GetPrice().ToString(CultureInfo.InvariantCulture) : ""; }
        }

        public string CumQty
        {
            get { return backingMsg.IsSetGenericField(Tags.CumQty) ? backingMsg.GetGenericField(Tags.CumQty) : ""; }
        }

        public string AvgPx
        {
            get { return backingMsg.IsSetAvgPx() ? backingMsg.GetAvgPx().ToString(CultureInfo.InvariantCulture) : ""; }
        }

        public string LeavesQty
        {
            get { return backingMsg.IsSetGenericField(Tags.LeavesQty) ? backingMsg.GetGenericField(Tags.LeavesQty) : ""; }
        }

        public string LastShares
        {
            get { return backingMsg.IsSetLastQty() ? backingMsg.GetLastQty().ToString(CultureInfo.InvariantCulture) : ""; }
        }

        public string LastPx
        {
            get { return backingMsg.IsSetLastPx() ? backingMsg.GetLastPx().ToString(CultureInfo.InvariantCulture) : ""; }
        }

        public List<string> Columns
        {
            get
            {
                return new List<string>()
                {
                    MessageType,
                    ClOrdID,
                    OrigClOrdID,
                    OrderId,
                    GetExecutionTypeString(backingMsg),
                    ExecId,
                    GetOrdStatusString(backingMsg),
                    OrderQty,
                    Price,
                    CumQty,
                    AvgPx,
                    LeavesQty,
                    LastShares,
                    LastPx,
                    MessageString
                };
            }
        }

        private static string GetExecutionTypeString(IMessage m)
        {
            try
            {
                string execTypeString = "";
                if (m.IsSetGenericField(Tags.ExecType))
                {
                    char execType = m.GetGenericField(Tags.ExecType)[0];
                    execTypeString = FixValues.ExecType[execType.ToString()];
                }
                return execTypeString;
            }
            catch (Exception ex)
            {
                return "";
            }
            
            
        }

        private static string GetOrdStatusString(IMessage m)
        {
            string ordStatus = "";
            if (m.IsSetGenericField(Tags.OrdStatus))
            {
                char execType = m.GetGenericField(Tags.OrdStatus)[0];
                ordStatus = FixValues.OrdStatus[execType.ToString()];
            }
            return ordStatus;
        }


        public static List<string> ColumnNames { get; } = new List<string>()
        {
            "MessageType",
            "ClOrdID",
            "OrigClOrdID",
            "OrderId",
            "ExecType",
            "ExecId",
            "OrdStatus",
            "OrderQty",
            "Price",
            "CumQty",
            "AvgPx",
            "LeavesQty",
            "LastShares",
            "LastPx",
            "MessageString"
        };
    }
}
