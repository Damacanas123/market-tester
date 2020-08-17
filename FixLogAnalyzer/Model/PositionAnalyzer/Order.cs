
using FixLogAnalyzer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix.Fields;

namespace FixLogAnalyzer.Model.PositionAnalyzer
{
    internal class Order
    {
        private List<FixMessage> Messages { get; set; } = new List<FixMessage>();
        /// <summary>
        /// Collection of faulty messages along with explanations
        /// </summary>
        private List<(FixMessage,string)> FaultyMessages { get; set; } = new List<(FixMessage, string)>();

        /// <summary>
        /// This collection stores non responded NewOrder,Replace and Cancel requests. Key is ClOrdID
        /// </summary>
        private Dictionary<string, FixMessage> UnhandledRequests { get; set; } = new Dictionary<string, FixMessage>();

        internal Order(FixMessage msg)
        {
            if(msg.GetField(Tags.MsgType) != "D")
            {
                throw new OrderBadInitialization("The message is not a NewOrderSingle");
            }

        }

        private string lastPx;
        internal string LastPx
        {
            get { return lastPx; }
            private set
            {
                lastPx = value;
            }
        }

        private string lastQty;
        internal string LastQty
        {
            get { return lastQty; }
            private set
            {
                lastQty = value;
            }
        }

        private string ordStatus;
        internal string OrdStatus
        {
            get { return ordStatus; }
            private set
            {
                ordStatus = value;
            }
        }

        private string cumulativeQty;
        internal string CumulativeQty
        {
            get { return cumulativeQty; }
            private set
            {
                cumulativeQty = value;
            }
        }

        private string avgPx;
        internal string AvgPx
        {
            get { return avgPx; }
            private set
            {
                avgPx = value;
            }
        }

        private string leavesQty;
        internal string LeavesQty
        {
            get { return leavesQty; }
            private set
            {
                leavesQty = value;
            }
        }

        private string price;
        internal string Price
        {
            get { return price; }
            private set
            {
                price = value;
            }
        }

        private string orderQty;
        internal string OrderQty
        {
            get { return orderQty; }
            private set
            {
                orderQty = value;
            }
        }

        internal void AddMessage(FixMessage msg)
        {
            bool isIncomingOrNew = true;
            string msgType = msg.GetField(Tags.MsgType);
            if (msgType == null)
            {
                FaultyMessages.Add((msg, "Message type not set"));
                return;
            }
            if(msgType == MsgType.ORDERCANCELREPLACEREQUEST || msgType == MsgType.ORDERCANCELREQUEST)
            {
                isIncomingOrNew = false;
            }
            string clOrdID = msg.GetField(Tags.ClOrdID);
            if(msgType == MsgType.NEWORDERSINGLE || msgType == MsgType.ORDERCANCELREQUEST || msgType == MsgType.ORDERCANCELREPLACEREQUEST)
            {
                if(clOrdID == null)
                {
                    FaultyMessages.Add((msg, "ClOrdID not set"));
                }
                UnhandledRequests[clOrdID] = msg;
            }
            
            if (isIncomingOrNew)
            {
                string value = msg.GetField(Tags.LastPx);
                if(value != null)
                {
                    LastPx = value;
                }
                value = msg.GetField(Tags.LastQty);
                if(value != null)
                {
                    LastQty = value;
                }
                value = msg.GetField(Tags.OrdStatus);
                if(value != null)
                {
                    OrdStatus = value;
                }
                value = msg.GetField(Tags.CumQty);
                if(value != null)
                {
                    CumulativeQty = value;
                }

                //if (msg.IsSetGenericField(QuickFix.Fields.Tags.CumQty))
                //    CumulativeQty = decimal.Parse(msg.GetGenericField(QuickFix.Fields.Tags.CumQty), CultureInfo.InvariantCulture);
                //if (msg.IsSetAvgPx())
                //    AvgPx = msg.GetAvgPx();
                //if (msg.IsSetGenericField(QuickFix.Fields.Tags.LeavesQty))
                //    LeavesQty = decimal.Parse(msg.GetGenericField(QuickFix.Fields.Tags.LeavesQty), CultureInfo.InvariantCulture);
                //if (msg.IsSetPrice())
                //    Price = msg.GetPrice();
                //if (msg.IsSetOrderQty())
                //    OrderQty = msg.GetOrderQty();
            }
        }

    }
}
