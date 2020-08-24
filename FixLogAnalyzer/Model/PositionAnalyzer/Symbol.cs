using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix.Fields;
using FixHelper;
using System.Globalization;

namespace FixLogAnalyzer.Model
{
    internal class Symbol
    {
        private string SymbolCode { get; set; }
        
        internal Symbol(string symbolCode)
        {
            SymbolCode = symbolCode;
            Position = new Position(symbolCode);
        }

        private Dictionary<string, Order> Orders { get; set; } = new Dictionary<string, Order>();

        internal void HandleMessage(FixMessage msg)
        {
            string msgType = msg.GetField(Tags.MsgType);
            string clOrdID = msg.GetField(Tags.ClOrdID);
            if (clOrdID != null)
            {
                if (!Orders.TryGetValue(clOrdID, out Order order))
                {
                    order = new Order();
                    Orders[clOrdID] = order;
                }
                order.AddMessage(msg);
            }
            if (msgType == MsgType.EXECUTIONREPORT)
            {
                //we know for sure that ExecType is set
                char execType = msg.GetField(Tags.ExecType)[0];
                if (execType == ExecType.TRADE)
                {
                    
                    char side = msg.GetField(Tags.ExecType)[0];
                    decimal lastShares = decimal.Parse(msg.GetField(Tags.LastQty), CultureInfo.InvariantCulture);
                    decimal lastPrice = decimal.Parse(msg.GetField(Tags.LastPx), CultureInfo.InvariantCulture);

                    TradeParameters trdPrms = new TradeParameters(side, lastShares, lastPrice, this.SymbolCode);
                    this.Position.AddTrade(trdPrms);

                }
            }
        }

        private Position Position { get; }
        


    }
}
