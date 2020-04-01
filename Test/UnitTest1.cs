using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BackOfficeEngine.Model;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.MessageEnums;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        

        [TestMethod]
        public void TestAccount()
        {
            Account account1 = Account.GetInstance("1");
            account1.AddTrade(new TradeParameters(Side.Sell, 10, 15.67m, "sym1"));
            account1.AddTrade(new TradeParameters(Side.Buy, 10, 15.68m, "sym1"));
            account1.AddTrade(new TradeParameters(Side.Sell, 40, 15.65m, "sym1"));
            account1.AddTrade(new TradeParameters(Side.Buy, 35, 15.67m, "sym1"));
            Account account2 = Account.GetInstance("2");
            account2.AddTrade(new TradeParameters(Side.Sell, 10, 15.67m, "sym2"));
            account2.AddTrade(new TradeParameters(Side.Buy, 10, 15.68m, "sym2"));
            account2.AddTrade(new TradeParameters(Side.Sell, 40, 15.65m, "sym2"));
            account2.AddTrade(new TradeParameters(Side.Buy, 35, 15.67m, "sym2"));
            account2.AddTrade(new TradeParameters(Side.Buy, 5, 15.72m, "sym2"));
            Console.WriteLine("Account1");
            foreach(Position pos in account1.Positions)
            {
                Console.WriteLine(pos);
            }
            Console.WriteLine("Account2");
            foreach (Position pos in account2.Positions)
            {
                Console.WriteLine(pos);
            }
        }

        [TestMethod]
        public void TestFixStringBuilder()
        {
            (string, string)[] heyo = new (string, string)[] { ("35","D"),("55","GARAN") };
            string msg = BackOfficeEngine.Helper.Fix.GetFixString(BackOfficeEngine.ProtocolType.Fix42, heyo);
            Console.WriteLine(msg);
        }
    }
}
