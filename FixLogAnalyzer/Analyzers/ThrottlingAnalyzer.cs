using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace FixLogAnalyzer
{
    public class ThrottlingAnalyzer : ILogAnalyzer
    {
        private IFixLineParser parser;
        private string inFilePath;
        private string outFilePath;
        private int expectedThrottling;
        private const string outDateFormat = "yyyyMMdd-HH:mm:ss";
        private const string hourMinuteSecondDateFormat = "HH:mm:ss";
        private const string sendingTimeFormat = "yyyyMMdd-HH:mm:ss.fff";
        private string messageTypes = "D-G-F-i";//default is given in order for easy use
        private HashSet<string> MessageTypesSet = new HashSet<string>();


        private void ConstructorCommonPostWork()
        {
            SetMessageTypes(this.messageTypes);
        }
        public override string ToString()
        {
            return "Throttling Analyzer";
        }
        public ThrottlingAnalyzer() 
        {
            ConstructorCommonPostWork();
        }

        public ThrottlingAnalyzer(string inFilePath,string outFilePath,IFixLineParser parser,int expectedThrottling,string messageTypes)
        {
            this.inFilePath = inFilePath;
            this.outFilePath = outFilePath;
            this.parser = parser;
            this.expectedThrottling = expectedThrottling;
            this.messageTypes = messageTypes;
            ConstructorCommonPostWork();

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

        public void SetExpectedThrottling(int expectedThrottling)
        {
            this.expectedThrottling = expectedThrottling;
        }

        public void SetMessageTypes(string messageTypes)
        {
            MessageTypesSet.Clear();
            this.messageTypes = messageTypes;
            string[] msgTypesArray = this.messageTypes.Split('-');
            foreach (string msgTypes in msgTypesArray)
            {
                MessageTypesSet.Add(msgTypes);
            }
        }

        public void Start()
        {
            List<FixMessage> msgs = GetFixMessages();
            Dictionary<long, int[]> allDatesDic = CountMessageNumInFrames(msgs);
            Dictionary<long, int[]> allDatesDicSliding = CreateSlidingWindowCounts(allDatesDic);
            List<long> keys = allDatesDicSliding.Keys.ToList();
            keys.Sort();
            string csvHeader = "Time(Second Precision),Time Difference,Frame 0,Frame 1,Frame 2,Frame 3,Frame 4,Frame 5,Frame 6,Frame 7,Frame 8,Frame 9,Under Throttling," +
                "OverThrottling";
            using(StreamWriter writer = new StreamWriter(outFilePath))
            {
                writer.WriteLine(csvHeader);
                long prevKey = (keys.Count > 0 ? keys[0] : 0);
                foreach(long key in keys)
                {
                    int [] slidingFrames = allDatesDicSliding[key];
                    int[] frames = allDatesDic[key];
                    bool isUnderThrottling = false;
                    bool isOverThrottling = false;
                    string line = new DateTime(key * 10000000).ToString(outDateFormat) + ",";
                    line += new DateTime((key - prevKey) * 10000000).ToString(hourMinuteSecondDateFormat) + ",";
                    for (int i = 0; i < slidingFrames.Length; i++)
                    {
                        int frameConsolidated = slidingFrames[i];
                        line += frameConsolidated + "|" + frames[i] +  ",";
                        if (!isUnderThrottling)
                        {
                            isUnderThrottling = frameConsolidated < expectedThrottling;
                        }
                        if (!isOverThrottling)
                        {
                            isOverThrottling = frameConsolidated > expectedThrottling;
                        }
                    }
                    line += isUnderThrottling + "," + isOverThrottling;
                    writer.WriteLine(line);
                    prevKey = key;
                }
            }
            

        }

        private List<FixMessage> GetFixMessages()
        {
            List<FixMessage> messages = new List<FixMessage>();
            FileStream stream = new FileStream(inFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            string line = Util.ReadLine(stream);
            while(!string.IsNullOrWhiteSpace(line)) 
            {
                string msg;
                DateTime logtime;
                (logtime,msg) = parser.Parse(line);
                FixMessage msgO = new FixMessage(msg, logtime);
                if (msgO.IsSet(35) && MessageTypesSet.Contains(msgO.GetField(35)))
                {
                    messages.Add(msgO);
                }
                line = Util.ReadLine(stream);
            }
            stream.Dispose();
            return messages;
        }

        private Dictionary<long,int[]> CountMessageNumInFrames(List<FixMessage> msgs)
        {
            Dictionary<long, int[]> allDatesDic = new Dictionary<long, int[]>();
            foreach (FixMessage msg in msgs)
            {
                int msgWeight = 1;
                //commented out because mass quote should also weigh as 1 message.
                //if(msg.GetField(35) == "i")
                //{
                //    msgWeight = msg.GetTotalMassQuoteNum();
                //}
                DateTime sendTime = DateTime.ParseExact(msg.GetField(52),sendingTimeFormat,CultureInfo.InvariantCulture);
                long frameInSecond = sendTime.Millisecond / 100L;
                long totalSeconds = sendTime.Ticks / 10000000;
                if (allDatesDic.TryGetValue(totalSeconds, out int[] frames))
                {
                    frames[frameInSecond] += msgWeight;
                }
                else
                {
                    allDatesDic[totalSeconds] = new int[10];
                    allDatesDic[totalSeconds][frameInSecond] += msgWeight;
                }             
            }
            return allDatesDic;
        }

        private Dictionary<long,int[]> CreateSlidingWindowCounts(Dictionary<long, int[]> allDatesDic)
        {
            Dictionary<long, int[]> allDatesDicSliding = new Dictionary<long, int[]>();
            foreach(KeyValuePair<long,int[]> pair in allDatesDic)
            {
                int[] currSecond = new int[10];
                for(int i = 0; i < 10; i++)
                {
                    int count = GetTotalCountInASecondOfAFrame(allDatesDic, pair.Key, i);
                    currSecond[i] = count;
                }
                allDatesDicSliding[pair.Key] = currSecond;
            }
            return allDatesDicSliding;
        }

        private int GetTotalCountInASecondOfAFrame(Dictionary<long, int[]> allDatesDic,long date,int frame) 
        {
            int count = 0;
            int[] currentSecond = allDatesDic[date];
            for (int i = frame; i < 10; i++)
            {
                count += currentSecond[i];
            }
            if (allDatesDic.TryGetValue(date + 1,out int[] nextSecond))
            {
                for(int i = 0; i < frame; i++)
                {
                    count += nextSecond[i];
                }
            }
            return count;
        }
    }
}
