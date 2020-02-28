using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Helper;
using BackOfficeEngine.GeneralBase;
using BackOfficeEngine.DB.SQLite;

namespace BackOfficeEngine.Model
{
    public class Position : BaseNotifier,IComparable,IDataBaseWritable
    {
        public Account account;
        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            private set
            {
                symbol = value;
                NotifyPropertyChanged(nameof(Symbol));
            }
        }
        private decimal averageCost;
        public decimal AverageCost
        {
            get { return averageCost; }
            private set 
            {
                averageCost = value;
                NotifyPropertyChanged(nameof(AverageCost));
            }
        }
        private decimal positionQty;
        public decimal PositionQty
        {
            get { return positionQty; }
            private set
            {
                positionQty = value;
                NotifyPropertyChanged(nameof(PositionQty));
            }
        }
        private decimal netprofit;
        public decimal Netprofit
        {
            get { return netprofit; }
            private set
            {
                netprofit = value;
                NotifyPropertyChanged(nameof(Netprofit));
            }
        }
        private Side side;
        public Side Side
        {
            get { return side; }
            private set
            {
                side = value;
                NotifyPropertyChanged(nameof(Side));
            }
        }

        public string TableName
        {
            get
            {
                return "Positions";
            }
        }


        public Dictionary<string,TableField> Fields { get; } = new Dictionary<string,TableField> {
            {nameof(account), new TableField(nameof(account),typeof(string),"",30) },
            {nameof(symbol), new TableField(nameof(symbol),typeof(string),"",30) },
            {nameof(averageCost), new TableField(nameof(averageCost),typeof(string),"",30) },
            {nameof(positionQty), new TableField(nameof(positionQty),typeof(string),"",30) },
            {nameof(netprofit), new TableField(nameof(netprofit),typeof(string),"",30) },
            {nameof(side), new TableField(nameof(side),typeof(string),"",30) },
            {nameof(DatabaseID), new TableField(nameof(DatabaseID),typeof(string),"PRIMARY KEY",48) }
        };

        public Dictionary<string,object> Values
        {
            get
            {
                return new Dictionary<string,object>
                {
                    {nameof(account),account },
                    {nameof(symbol),symbol },
                    {nameof(averageCost),averageCost },
                    {nameof(positionQty),positionQty },
                    {nameof(netprofit),netprofit },
                    {nameof(side),side },
                    {nameof(DatabaseID),DatabaseID }
                };
            }
        }

        public string DatabaseID
        {
            get
            {
                return account + symbol;
            }
        }

        public string DatabaseIDColumnName
        {
            get { return "ID"; }
        }

        private object tradeLock = new object();

        public event PropertyChangedEventHandler PropertyChanged;
        internal Position() { }

        internal Position(string symbol,Account account)
        {
            this.symbol = symbol;
            this.account = account;
        }

        internal void AddTrade(TradeParameters prms)
        {
            lock (tradeLock)
            {
                switch (prms.side)
                {
                    case Side.Buy:
                        prms.price = -prms.price;//struct is a value type. No worries changing struct field about affecting function caller scope.
                        break;
                }

                if (prms.side != side)
                {
                    if (prms.lastShares >= positionQty)
                    {
                        netprofit += positionQty * (prms.price + averageCost);
                        positionQty = prms.lastShares - positionQty;
                        averageCost = prms.price;
                        side = prms.side;
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
            }
        }
        public override string ToString()
        {
            return symbol + " " + side + " " + positionQty + " " + Util.RoundToDecimalPlaces(averageCost,4) + " " + Util.RoundToDecimalPlaces(netprofit,4);
        }
        #region IComparable implementation
        public int CompareTo(object obj)
        {
            if(obj.GetType() != typeof(Position))
            {
                throw new ArgumentException("Can't compare Position to object of type " + obj.GetType().ToString());
            }
            Position other = (Position)obj;
            return string.Compare(this.symbol,other.symbol,StringComparison.OrdinalIgnoreCase);
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
            return this.symbol.Equals(other.symbol,StringComparison.OrdinalIgnoreCase);
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
