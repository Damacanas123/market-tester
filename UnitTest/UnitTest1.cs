using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BackOfficeEngine.Helper;
using MarketTester.Helper;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string filePath = @"C:\MATRIKS_OMS\MarketTester\FIXLog\Primary\FIXT.1.1-FC221_FE343-BI_TEST2-Primary.messages.current.log";
            int startIndex = "20200410-13:47:24.131 : ".Length;

            foreach (string msg in MarketTester.Helper.MarketTesterUtil.ReadLines(filePath))
            {
                    Assert.AreEqual(true, Fix.CheckMessageValidity(msg.Substring(startIndex, msg.Length - startIndex)));
            }            
        }

        


    }
}
