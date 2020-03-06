using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BackOfficeEngine.ParamPacker;

namespace BackOfficeEngine.Model
{
    //extended singleton
    public class Account : IComparable
    {
        private static ConcurrentDictionary<string, Account> instances = new ConcurrentDictionary<string, Account>();
        
        internal static Account GetInstance(string accountString)
        {
            if(instances.TryGetValue(accountString, out Account account))
            {
                
            }
            else
            {
                account = new Account(accountString);
                instances[accountString] = account;
                Accounts.Add(account);
            }
            return account;
        }

        public static Account GetAccount(string accountName)
        {
            return instances[accountName];
        }

        
        public static ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();


        public string name
        {
            get; private set;
        }
        private ConcurrentDictionary<string, Position> positionsDic = new ConcurrentDictionary<string, Position>();

        public ObservableCollection<Position> Positions { get; } = new ObservableCollection<Position>();
        private Account(string name) 
        {
            this.name = name;
        }

        internal void AddTrade(TradeParameters prms)
        {
            if(positionsDic.TryGetValue(prms.symbol,out Position position))
            {
                
            }
            else
            {
                position = new Position(prms.symbol,this);
                positionsDic[prms.symbol] = position;
                Positions.Add(position);
            }
            position.AddTrade(prms);
        }

        internal void LoadPosition(Position position)
        {
            positionsDic[position.Symbol] = position;
            Positions.Add(position);
        }

        public override string ToString()
        {
            return this.name;
        }


        #region IComparable implementation
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Account))
            {
                throw new ArgumentException("Can't compare Account to object of type " + obj.GetType().ToString());
            }
            Account other = (Account)obj;
            return string.Compare(this.name, other.name, StringComparison.OrdinalIgnoreCase);
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

            Account other = (Account)obj;
            return this.name.Equals(other.name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public static bool operator ==(Account left, Account right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Account left, Account right)
        {
            return !(left == right);
        }

        public static bool operator <(Account left, Account right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Account left, Account right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Account left, Account right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Account left, Account right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
        #endregion
    }
}
