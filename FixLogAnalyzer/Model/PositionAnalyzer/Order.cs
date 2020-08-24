
using FixLogAnalyzer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix.Fields;
using FixHelper;

namespace FixLogAnalyzer.Model
{
    internal class Order
    {
        internal static string INITIAL_PENDING = "Initial Pending";
        private List<FixMessage> Messages { get; set; } = new List<FixMessage>();
        /// <summary>
        /// Collection of faulty messages along with explanations
        /// </summary>
        

        /// <summary>
        /// This collection stores non responded NewOrder,Replace and Cancel requests. Key is ClOrdID
        /// </summary>
        

        internal Order()
        {            
            OrdStatus = INITIAL_PENDING;
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
            Messages.Add(msg);
            bool isIncomingOrNew = true;
            string msgType = msg.GetField(Tags.MsgType);
            
            if(msgType == MsgType.ORDERCANCELREPLACEREQUEST || msgType == MsgType.ORDERCANCELREQUEST)
            {
                isIncomingOrNew = false;
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
                value = msg.GetField(Tags.AvgPx);
                if (value != null)
                {
                    AvgPx = value;
                }
                value = msg.GetField(Tags.LeavesQty);
                if (value != null)
                {
                    LeavesQty = value;
                }
                value = msg.GetField(Tags.Price);
                if (value != null)
                {
                    Price = value;
                }
                value = msg.GetField(Tags.OrderQty);
                if (value != null)
                {
                    OrderQty = value;
                }
            }
        }

    }
}
