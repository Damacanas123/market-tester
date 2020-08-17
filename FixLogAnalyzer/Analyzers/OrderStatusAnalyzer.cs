using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FixLogAnalyzer.Exceptions;
using QuickFix.Fields;

namespace FixLogAnalyzer
{
    public class OrderStatusAnalyzer : ILogAnalyzer
    {
        private string inFilePath;
        private string outFilePath;
        IFixLineParser parser;

        public override string ToString()
        {
            return "Order Status Analyzer";
        }
        public OrderStatusAnalyzer() { }
        public OrderStatusAnalyzer(string inFilePath,string outFilePath,IFixLineParser parser)
        {
            this.inFilePath = inFilePath;
            this.outFilePath = outFilePath;
            this.parser = parser;
        }

        public void SetInFilePath(string inFilePath)
        {
            this.inFilePath = inFilePath;
        }

        public void SetLogParser(IFixLineParser parser)
        {
            this.parser = parser;
        }

        public void SetOutFilePath(string outFilePath)
        {
            this.outFilePath = outFilePath;
        }

        public void Start()
        {
            Dictionary <string,Order> clOrdIDMap = new Dictionary<string, Order>();
            List<Order> orderList = new List<Order>();
            FileStream stream = new FileStream(inFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            string line;
            using (StreamReader reader = new StreamReader(inFilePath))
            {
                while(true)
                {
                    line = Util.ReadLine(stream);
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }
                    Console.WriteLine(line);
                    string msg;
                    DateTime logtime;
                    (logtime, msg) = parser.Parse(line);
                    FixMessage fixMsg = new FixMessage(msg, logtime);
                    string msgType = fixMsg.GetField(Tags.MsgType);
                    if (msgType == "D")
                    {
                        Order order;
                        try
                        {
                            order = new Order(fixMsg);
                        }
                        catch(FixMessageNullValueException ex)
                        {
                            continue;
                        }
                        clOrdIDMap[order.clOrdIds[0]] = order;
                        orderList.Add(order);
                    }
                    if (msgType == "8")
                    {
                        string clOrdID = fixMsg.GetField(Tags.ClOrdID);
                        if (string.IsNullOrWhiteSpace(clOrdID))
                        {
                            continue;
                        }
                        string execType = fixMsg.GetField(Tags.ExecType);
                        if (string.IsNullOrWhiteSpace(execType))
                        {
                            continue;
                        }
                        Order order = null;
                        if (execType == "0")
                        {
                            if (!clOrdIDMap.ContainsKey(clOrdID))
                            {
                                continue;
                            }
                            order = clOrdIDMap[fixMsg.GetField(Tags.ClOrdID)];
                            string orderId = fixMsg.GetField(Tags.OrderID);
                            if(orderId == null)
                            {
                                continue;
                            }
                            clOrdIDMap[orderId] = order;
                        }
                        else if (execType == "8")
                        {
                            if (!clOrdIDMap.ContainsKey(clOrdID))
                            {
                                continue;
                            }
                            order = clOrdIDMap[clOrdID];
                        }
                        else if (execType == "5" || execType == "4")
                        {
                            if (fixMsg.IsSet(Tags.OrigClOrdID))
                            {
                                string origClOrdID = fixMsg.GetField(Tags.OrigClOrdID);
                                if (!clOrdIDMap.ContainsKey(origClOrdID))
                                {
                                    continue;
                                }
                                order = clOrdIDMap[origClOrdID];
                                clOrdIDMap[clOrdID] = order;
                            }
                            else
                            {
                                string orderID = fixMsg.GetField(Tags.OrderID);
                                if(orderID == null)
                                {
                                    continue;
                                }
                                if (!clOrdIDMap.ContainsKey(orderID))
                                {
                                    continue;
                                }
                                order = clOrdIDMap[orderID];
                            }
                        }
                        else if (execType == "F")
                        {
                            if (!clOrdIDMap.ContainsKey(clOrdID))
                            {
                                continue;
                            }
                            order = clOrdIDMap[clOrdID];
                        }
                        if (order != null)
                        {
                            order.AddExecutionReport(fixMsg);
                        }
                    }

                }

            }
                stream.Dispose();
            WriteResultsToFile(outFilePath,orderList);
        }

        private void WriteResultsToFile(string filePath,List<Order> orderList)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(Order.csvHeader);
                foreach (Order order in orderList)
                {
                    writer.WriteLine(order.GetCsvRepr());
                }
            }
        }
    }
}
