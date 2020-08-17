using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using FixLogAnalyzer.Exceptions;
using QuickFix.Fields;

namespace FixLogAnalyzer
{
    internal class Order
    {
        internal static string dateFormat = "yyyyMMdd-HH:mm:ss.ffffff";
        public string symbol;
        public string account;
        public decimal orderQty;
        public decimal price;
        public decimal cumulativeQty
        {
            get { return orderQty - leavesQty; }
        }
        private string ordStatus;
        private string OrderStatus
        {
            get
            {
                switch (ordStatus[0])
                {
                    case OrdStatus.NEW:
                        return "New";
                    case OrdStatus.PARTIALLY_FILLED:
                        return "Partial Fill";
                    case OrdStatus.FILLED:
                        return "Filled";
                    case OrdStatus.REJECTED:
                        return "Rejected";
                    case OrdStatus.CANCELED:
                        return "Canceled";
                    case 'I':
                        return "Initial Pending";
                    default:
                        return "Unknown";
                }
            }
        }
        private string side;
        public string TextSide
        {
            get
            {
                if (side == Side.BUY.ToString())
                {
                    return "Buy";
                }
                else if (side == Side.SELL.ToString())
                {
                    return "Sell";
                }
                else return "Unknown";
            }
        }
        public decimal leavesQty;
        public decimal avgPx = 0;
        public bool IsOpen
        {
            get { return leavesQty != 0; }
        }
        public string orderID;
        internal DateTime firstLogTime;
        public List<string> clOrdIds = new List<string>();
        public Order(FixMessage newMsg)
        {
            this.symbol = newMsg.GetField(Tags.Symbol);
            string orderQtyS = newMsg.GetField(Tags.OrderQty);
            if(orderQtyS == null)
            {
                throw new FixMessageNullValueException("OrderQty not found");
            }
            this.orderQty = Decimal.Parse(orderQtyS, CultureInfo.InvariantCulture);
            if (newMsg.IsSet(Tags.Price))
            {
                this.price = Decimal.Parse(newMsg.GetField(Tags.OrderQty), CultureInfo.InvariantCulture);
            }
            
            this.side = newMsg.GetField(Tags.Side);
            if(this.side == null)
            {
                throw new FixMessageNullValueException("Side not found");
            }
            this.leavesQty = this.orderQty;
            this.ordStatus = "I";//initial pending
            this.account = newMsg.GetField(Tags.Account);
            string clOrdId = newMsg.GetField(Tags.ClOrdID);
            if(clOrdId == null)
            {
                throw new FixMessageNullValueException("ClOrdID not found");
            }
            this.clOrdIds.Add(newMsg.GetField(Tags.ClOrdID));
            firstLogTime = newMsg.logtime;
        }

        public void AddExecutionReport(FixMessage msg)
        {
            if(this.clOrdIds[0] == "X224343018")
            {

            }
            string execType = msg.GetField(Tags.ExecType);
            if (msg.IsSet(Tags.LeavesQty))
            {
                leavesQty = Decimal.Parse(msg.GetField(Tags.LeavesQty), CultureInfo.InvariantCulture);
            }
            if (msg.IsSet(Tags.OrderQty))
            {
                orderQty = Decimal.Parse(msg.GetField(Tags.OrderQty), CultureInfo.InvariantCulture);
            }
            if (msg.IsSet(Tags.Price))
            {
                price = Decimal.Parse(msg.GetField(Tags.Price), CultureInfo.InvariantCulture);
            }
            if (msg.IsSet(Tags.OrdStatus))
            {
                ordStatus = msg.GetField(Tags.OrdStatus);
            }

            if (msg.IsSet(Tags.ClOrdID))
            {
                string clOrdID = msg.GetField(Tags.ClOrdID);
                if (!clOrdIds.Contains(clOrdID))
                {
                    clOrdIds.Add(clOrdID);
                }                
            }
            if (msg.IsSet(Tags.OrderID))
            {
                orderID = msg.GetField(Tags.OrderID);
            }
            if (msg.IsSet(Tags.LastQty) && msg.IsSet(Tags.LastPx))
            {
                decimal lastQty = Decimal.Parse(msg.GetField(Tags.LastQty), CultureInfo.InvariantCulture);
                decimal lastPx = Decimal.Parse(msg.GetField(Tags.LastPx), CultureInfo.InvariantCulture);
                avgPx = (cumulativeQty * avgPx + lastPx * lastQty) / (cumulativeQty + lastQty);
            }
        }

        public static string csvHeader = "First Logtime,OrderID,ClOrdIds,Account,Symbol,Side,OrderQty,Price,OrdStatus,AvgPx,LeavesQty";
        public string GetCsvRepr()
        {
            string clOrdIdAll = "";
            foreach(string clOrdId in clOrdIds)
            {
                clOrdIdAll += clOrdId + "|";
            }
            clOrdIdAll = clOrdIdAll.Substring(0, clOrdIdAll.Length - 1);
            return $"{firstLogTime.ToString(dateFormat)},{orderID},{clOrdIdAll},{account},{symbol},{TextSide},{orderQty},{price},{OrderStatus},{avgPx},{leavesQty}";
        }
    }
}
