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
    internal class Account
    {
        private string accountCode;

        public string AccountCode
        {
            get { return accountCode; }
            private set
            {
                accountCode = value;                
            }
        }

        private static Dictionary<string,Account> Instances { get; set; }

        internal static List<Account> Accounts
        {
            get
            {
                List<Account> accounts = new List<Account>();
                foreach(KeyValuePair<string,Account> acc in Instances)
                {
                    accounts.Add(acc.Value);
                }
                return accounts;
            }
        }

        internal static Account GetInstance(string accountCode)
        {
            if(!Instances.TryGetValue(accountCode,out Account account))
            {
                account = new Account(accountCode);
                Instances[accountCode] = account;
            }
            return account;
        }

        private Account(string accountCode)
        {
            this.accountCode = accountCode;
        }

        private static Dictionary<string, FixMessage> UnhandledRequests { get; set; } = new Dictionary<string, FixMessage>();
        
        private static List<(FixMessage, string)> FaultyMessages { get; set; } = new List<(FixMessage, string)>();

        internal static void HandleMessageStatic(FixMessage msg)
        {
            string msgType = msg.GetField(Tags.MsgType);
            if (msgType == null)
            {
                FaultyMessages.Add((msg,InfoMessages.MsgTypeNotSet));
                return;
            }
            string clOrdID = msg.GetField(Tags.ClOrdID);
            
            if(msgType == MsgType.EXECUTIONREPORT)
            {
                string execType = msg.GetField(Tags.MsgType);
                if(execType == null)
                {
                    FaultyMessages.Add((msg, InfoMessages.ExecTypeNotSetOnExecutionReport));
                }
            }
            if (FixValues.MsgTypesOrderEntry.ContainsKey(msgType))
            {
                if (clOrdID == null)
                {
                    FaultyMessages.Add((msg, InfoMessages.ClOrdIDNotSet));
                    return;
                }
                string accountCode = msg.GetField(Tags.Account);
                if (accountCode == null)
                {
                    accountCode = msg.GetField(Tags.TargetSubID);
                }
                if (accountCode == null)
                {
                    FaultyMessages.Add((msg, InfoMessages.NoAccountOrTargetSubIdSetOnTradeReport));
                    return;
                }
                Account account = Account.GetInstance(accountCode);
                account.HandleMessage(msg);
            }
            
            if (FixValues.MsgTypesOrderEntryOutbound.ContainsKey(msgType))
            {
                
                if (msgType != MsgType.NEWORDERSINGLE && !(msg.IsSet(Tags.OrigClOrdID) || msg.IsSet(Tags.OrderID)))
                {
                    FaultyMessages.Add((msg, InfoMessages.BothOrigClOrdOrderIdNotSet));
                    return;
                }
                UnhandledRequests[clOrdID] = msg;
            }
            else if (FixValues.MsgTypesOrderEntryInbound.ContainsKey(msgType))
            {
                if (!UnhandledRequests.ContainsKey(clOrdID))
                {
                    FaultyMessages.Add((msg, InfoMessages.ResponseArrivedWhenThereisNoRequest));
                    return;
                }

                UnhandledRequests.Remove(clOrdID);
            }
            

        }

        
        private Dictionary<string, Symbol> Symbols = new Dictionary<string, Symbol>();

        internal void HandleMessage(FixMessage msg)
        {
            string symbolCode = msg.GetField(Tags.Symbol);
            if (!Symbols.TryGetValue(symbolCode, out Symbol symbol))
            {
                symbol = new Symbol(symbolCode);
            }
            symbol.HandleMessage(msg);

        }
    }
}
