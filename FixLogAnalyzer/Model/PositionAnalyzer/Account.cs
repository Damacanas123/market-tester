using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix.Fields;

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

        internal static void HandleMessageStatic(FixMessage msg)
        {
            string msgType = msg.GetField(Tags.MsgType);
            if(msgType == null)
            {
                return;
            }

        }

        internal void HandleMessage(FixMessage msg)
        {
            
        }
    }
}
