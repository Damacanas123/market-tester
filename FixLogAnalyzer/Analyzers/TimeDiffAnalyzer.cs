using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FixLogAnalyzer
{
    public class RequestAckPair
    {
        public LogMessage request;
        public LogMessage acknowledge;
        public TimeSpan diff;
        public long diffMicro
        {
            get
            {
                return diff.Ticks / 10;
            }
        }
    }
    public class LogMessage
    {
        public DateTime logtime;
        public string msg;
        public string clOrdIDClippedFromRight;
        public LogMessage(DateTime logtime,string msg)
        {
            this.logtime = logtime;
            this.msg = msg;
            clOrdIDClippedFromRight = Util.GetTag(msg, "11");
            if(clOrdIDClippedFromRight.Length > 15)
            {
                clOrdIDClippedFromRight = clOrdIDClippedFromRight.Substring(clOrdIDClippedFromRight.Length - 15, 15);
            }
        }
    }
    public class TimeDiffAnalyzer : ILogAnalyzer
    {
        private string inFilePath;
        private string outFilePath;
        private IFixLineParser parser;
        private static string dateFormat = "yyyyMMdd-HH:mm:ss.ffffff";

        private List<string> requestMsgTypes = new List<string>() { "D", "G", "F" };
        private List<string> ackExecTypes = new List<string>() { "0", "4", "5","8" };

        public override string ToString()
        {
            return "Time Diff Analyzer";
        }
        public TimeDiffAnalyzer() { }
        public TimeDiffAnalyzer(string inFilePath,string outFilePath,IFixLineParser parser)
        {
            this.inFilePath = inFilePath;
            this.outFilePath = outFilePath; 
            this.parser = parser;
        }

        public List<RequestAckPair> StartInMemory()
        {
            FileStream stream = new FileStream(inFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            string line = Util.ReadLine(stream);
            List<LogMessage> unhandledRequests = new List<LogMessage>();
            List<RequestAckPair> pairs = new List<RequestAckPair>();
            while (!string.IsNullOrEmpty(line))
            {
                DateTime logtime;
                string fixMessage;
                (logtime, fixMessage) = parser.Parse(line);
                string msgType = Util.GetTag(fixMessage, "35");
                if (requestMsgTypes.Contains(msgType))
                {
                    unhandledRequests.Add(new LogMessage(logtime,fixMessage));
                }
                else if (msgType == "8")
                {
                    string execType = Util.GetTag(fixMessage, "150");
                    if (ackExecTypes.Contains(execType))
                    {
                        string clOrdId = Util.GetTag(fixMessage, "11");
                        foreach (LogMessage request in unhandledRequests)
                        {
                            if (clOrdId == Util.GetTag(request.msg, "11"))
                            {
                                pairs.Add(new RequestAckPair
                                {
                                    request = request,
                                    acknowledge = new LogMessage(logtime, fixMessage),
                                    diff = logtime - request.logtime
                                });
                                unhandledRequests.Remove(request);
                                break;
                            }
                        }
                    }
                }
                line = Util.ReadLine(stream);
            }
            stream.Dispose();
            return pairs;
        }

        public void Start()
        {
            FileStream stream = new FileStream(inFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            string line = Util.ReadLine(stream);
            List<LogMessage> unhandledRequests = new List<LogMessage>();
            using(StreamWriter writer = new StreamWriter(outFilePath))
            {
                writer.WriteLine("MsgType,Diff Micro,Symbol,OrderQty,Side,Request Time,Acknowledge Time,Request,Acknowledge");
                while (!string.IsNullOrEmpty(line))
                {
                    DateTime logtime;
                    string fixMessage;
                    (logtime, fixMessage) = parser.Parse(line);
                    string msgType = Util.GetTag(fixMessage, "35");
                    if (requestMsgTypes.Contains(msgType))
                    {
                        unhandledRequests.Add(new LogMessage(logtime, fixMessage));
                    }
                    else if (msgType == "8")
                    {
                        string execType = Util.GetTag(fixMessage, "150");
                        if (ackExecTypes.Contains(execType))
                        {
                            string clOrdId = Util.GetTag(fixMessage, "11");
                            foreach (LogMessage request in unhandledRequests)
                            {
                                if (clOrdId == Util.GetTag(request.msg, "11"))
                                {
                                    LogMessage acknowledge = new LogMessage(logtime, fixMessage);
                                    TimeSpan diff = logtime - request.logtime;
                                    string s = Util.GetTag(request.msg, "35") + "," + (diff.Ticks / 10).ToString() + "," + Util.GetTag(request.msg, "55") + "," + Util.GetTag(request.msg, "38") + "," + Util.GetSidePretty(request.msg) + "," +
                                        ConvertDate(request.logtime) + "," + ConvertDate(acknowledge.logtime) +  "," +
                                        request.msg + "," + acknowledge.msg;
                                    
                                    writer.WriteLine(s);
                                    unhandledRequests.Remove(request);
                                    break;
                                }
                            }
                        }
                    }
                    else if(msgType == "9")
                    {
                        string clOrdId = Util.GetTag(fixMessage, "11");
                        foreach (LogMessage request in unhandledRequests)
                        {
                            if (clOrdId == Util.GetTag(request.msg, "11"))
                            {
                                LogMessage acknowledge = new LogMessage(logtime, fixMessage);
                                TimeSpan diff = logtime - request.logtime;
                                string s = Util.GetTag(request.msg, "35") + "," + (diff.Ticks / 10).ToString() + "," + Util.GetTag(request.msg, "55") + "," + Util.GetTag(request.msg, "38") + "," + Util.GetSidePretty(request.msg) + "," +
                                        ConvertDate(request.logtime) + "," + ConvertDate(acknowledge.logtime) + "," +
                                        request.msg + "," + acknowledge.msg;
                                writer.WriteLine(s);
                                unhandledRequests.Remove(request);
                                break;
                            }
                        }
                    }
                    line = Util.ReadLine(stream);
                }
            }
            stream.Dispose();
            
        }
        private string ConvertDate(DateTime dt)
        {
            return dt.ToString(dateFormat);
        }

        public void SetInFilePath(string inFilePath)
        {
            this.inFilePath = inFilePath;
        }

        public void SetOutFilePath(string outFilePath)
        {
            this.outFilePath = outFilePath;
        }

        public void SetLogParser(IFixLineParser parser)
        {
            this.parser = parser;
        }
    }
}
