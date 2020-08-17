using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixLogAnalyzer;
using QuickFix.Fields;

namespace FixLogAnalyzer.Model
{


    //isnt exactly the same struct as in BackOfficeEngine. And classes are copied to prevent cross dependency between projects
    internal struct TradeParameters
    {
        private char sideBacking;
        internal char side 
        { 
            get
            {
                return sideBacking;
            }
            set
            {
                sideBacking = value;
                if(value == Side.SELL || value == Side.SELL_SHORT)
                {
                    simpleSide = Side.SELL;
                }
                else
                {
                    simpleSide = Side.BUY;
                }
            }
        }
        //simple side is either buy or sell
        internal char simpleSide { get; set; }
        internal decimal lastShares { get; set; }
        internal decimal price { get; set; }
        internal string symbol { get; set; }
        internal TradeParameters(
            char side,
            decimal lastShares,
            decimal price,
            string symbol)
        {            
            this.lastShares = lastShares;
            this.price = price;
            this.symbol = symbol;
            //dummy assignments because of C# compiler
            this.sideBacking = '0';
            this.simpleSide = '0';
            //end of dummy assignments
            this.side = side;
        }

        public override string ToString()
        {
            return $"Side : {side}{Environment.NewLine}LastShares : {lastShares}{Environment.NewLine}Price : {price}{Environment.NewLine}Symbol : {symbol}";
        }
    }
    

    internal class ConsolidatedTransaction
    {
        private decimal totalQuantity;

        public decimal TotalQuantity
        {
            get { return totalQuantity; }
            set
            {
                totalQuantity = value;
            }
        }

        private decimal averagePrice;

        public decimal AveragePrice
        {
            get { return averagePrice; }
            set
            {
                averagePrice = value;
            }
        }


    }

    internal class Position : IComparable
    {
        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            private set
            {
                symbol = value;
            }
        }
        private decimal averageCost;
        public decimal AverageCost
        {
            get { return averageCost; }
            private set
            {
                averageCost = value;
            }
        }
        private decimal positionQty;
        public decimal PositionQty
        {
            get { return positionQty; }
            private set
            {
                positionQty = value;
            }
        }
        private decimal netprofit;
        public decimal Netprofit
        {
            get { return netprofit; }
            private set
            {
                netprofit = value;
            }
        }
        private char side;
        public char Side
        {
            get { return side; }
            private set
            {
                side = value;
            }
        }

        private ConsolidatedTransaction SellShortTransactions { get; set; }
        private ConsolidatedTransaction SellTransactions { get; set; }
        private ConsolidatedTransaction BuyTransactions { get; set; }

        
        internal Position(string symbol) 
        {
            this.side = QuickFix.Fields.Side.BUY;
            this.symbol = symbol;
        }

        

        

        internal void AddTrade(TradeParameters prms)
        {
            switch (prms.simpleSide)
            {
                case QuickFix.Fields.Side.BUY:
                    prms.price = -prms.price;//struct is a value type. No worries changing struct field about affecting function caller scope.
                    break;
            }

            if (prms.simpleSide != side)
            {
                if (prms.lastShares >= positionQty)
                {
                    netprofit += positionQty * (prms.price + averageCost);
                    positionQty = prms.lastShares - positionQty;
                    averageCost = prms.price;
                    side = prms.simpleSide;
                }
                else
                {
                    netprofit += prms.lastShares * (prms.price + averageCost);
                    positionQty -= prms.lastShares;
                }
            }
            else
            {
                averageCost = ((averageCost * positionQty) + (prms.price * prms.lastShares)) / (positionQty + prms.lastShares);
                positionQty += prms.lastShares;
            }

            void HandleConsolidated(ConsolidatedTransaction ct)
            {
                decimal currTotal = ct.AveragePrice * ct.TotalQuantity + prms.price * prms.lastShares;
                ct.TotalQuantity += prms.lastShares;
                ct.AveragePrice = currTotal / ct.TotalQuantity;
            }
            switch (prms.side)
            {
                case QuickFix.Fields.Side.BUY:
                    HandleConsolidated(BuyTransactions);
                    break;
                case QuickFix.Fields.Side.SELL:
                    HandleConsolidated(SellTransactions);
                    break;
                case QuickFix.Fields.Side.SELL_SHORT:
                    HandleConsolidated(SellShortTransactions);
                    break;
            }
        }
        public override string ToString()
        {
            return symbol + " " + side + " " + positionQty + " " + Util.RoundToDecimalPlaces(averageCost, 4) + " " + Util.RoundToDecimalPlaces(netprofit, 4);
        }
        #region IComparable implementation
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Position))
            {
                throw new ArgumentException("Can't compare Position to object of type " + obj.GetType().ToString());
            }
            Position other = (Position)obj;
            return string.Compare(this.symbol, other.symbol, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            Position other = (Position)obj;
            return this.symbol.Equals(other.symbol, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.symbol.GetHashCode();
        }

        public static bool operator ==(Position left, Position right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public static bool operator <(Position left, Position right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Position left, Position right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Position left, Position right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Position left, Position right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
        #endregion
    }
}
