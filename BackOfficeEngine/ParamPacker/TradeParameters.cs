using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.MessageEnums;

namespace BackOfficeEngine.ParamPacker
{
    internal class TradeParameters
    {
        internal Side side;
        internal decimal lastShares;
        internal decimal price;
        internal string symbol;
        internal TradeParameters(
            Side side,
            decimal lastShares,
            decimal price,
            string symbol)
        {
            this.side = side;
            this.lastShares = lastShares;
            this.price = price;
            this.symbol = symbol;
        }

        public override string ToString()
        {
            return $"Side : {side}\nLastShares : {lastShares}\nPrice : {price}\nSymbol : {symbol}";
        }
    }
}
