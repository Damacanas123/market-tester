using ControlzEx.Standard;
using FixHelper;
using FixLogAnalyzer;
using MarketTester.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.LogLoader
{
    public class RequestResponses
    {
        public static Dictionary<string,Dictionary<string,List<EchoBackTagValuePair>>> EchoBackTags { get; set; } = new Dictionary<string, Dictionary<string, List<EchoBackTagValuePair>>>();
        //first message that contains a new clOrdId will be assumed as a request
        public ExtendedLogMessage Request { get; set; }
        //all messages with the same ClOrdID that comes after the request message will be assumed as responses
        public List<ExtendedLogMessage> Responses { get; set; } = new List<ExtendedLogMessage>();
        public void Add(ExtendedLogMessage msg)
        {
            if( Request == null)
            {
                Request = msg;
            }
            else
            {
                string requestMsgType = Request.GetField(Tags.MsgType);
                if (!EchoBackTags.TryGetValue(requestMsgType, out Dictionary<string,List<EchoBackTagValuePair>> responseMsgMap))
                {
                    responseMsgMap = new Dictionary<string, List<EchoBackTagValuePair>>();
                    EchoBackTags[requestMsgType] = responseMsgMap;
                }
                string msgType = msg.GetField(Tags.MsgType);
                Responses.Add(msg);
                if(!responseMsgMap.TryGetValue(msgType ,out List<EchoBackTagValuePair> tags))
                {
                    tags = new List<EchoBackTagValuePair>();
                    responseMsgMap[msgType] = tags;
                }
                foreach (KeyValuePair<int, string> pair in msg.TagValuePairs)
                {
                    if (Request.TagValuePairs.TryGetValue(pair.Key, out string value) && value == pair.Value)
                    {
                        EchoBackTagValuePair echoPair = new EchoBackTagValuePair(pair.Key, pair.Value,Request.LineNum,msg.LineNum);
                        EchoBackTagValuePair inList = responseMsgMap[msgType].FirstOrDefault((o) => o.Tag == echoPair.Tag && o.Value == echoPair.Value);
                        if (inList == null)
                        {
                            responseMsgMap[msgType].Add(echoPair);
                            inList = echoPair;
                        }
                        else
                        {
                            inList.OccurrenceNum++;
                            inList.OccurenceLines.Add((Request.LineNum, msg.LineNum));
                            if (msg.GetField(Tags.ClOrdID) == "4m9VGASFfJ1")
                            {

                            }
                        }
                            
                    }
                }
            }
        }

        public static List<string> GetResponseMsgTypes(string requestMsgType)
        {
            if(EchoBackTags.TryGetValue(requestMsgType,out Dictionary<string,List<EchoBackTagValuePair>> responseMsgTypes))
            {
                return responseMsgTypes.Keys.ToList();
            }
            else
            {
                return new List<string>();
            }
        }



        

    }

    public class EchoBackTagValuePair : BaseNotifier,IComparable
    {
        private int tag;

        public int Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                TagDescription = AllFixTags.GetInstance().GetTagExplanation(tag);
                NotifyPropertyChanged(nameof(Tag));
            }
        }

        private string tagDescription;

        public string TagDescription
        {
            get { return tagDescription; }
            set
            {
                tagDescription = value;
                NotifyPropertyChanged(nameof(TagDescription));
            }
        }


        private string value;

        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                ValueDescription = AllFixTags.GetInstance().GetValueExplanation(Tag, this.value);
                NotifyPropertyChanged(nameof(Value));
            }
        }

        private string  valueDescription;

        public string  ValueDescription
        {
            get { return valueDescription; }
            set
            {
                valueDescription = value;
                NotifyPropertyChanged(nameof(ValueDescription));
            }
        }


        private int occurrenceNum = 1;

        public int OccurrenceNum
        {
            get { return occurrenceNum; }
            set
            {
                occurrenceNum = value;
                NotifyPropertyChanged(nameof(OccurrenceNum));
            }
        }

        public List<(int, int)> OccurenceLines { get; set; } = new List<(int, int)>();


        public EchoBackTagValuePair(int tag, string value, int requestLine, int responseLine)
        {
            Tag = tag;
            Value = value;
            OccurenceLines.Add((requestLine, responseLine));
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            EchoBackTagValuePair other = obj as EchoBackTagValuePair;
            return (this.Tag.CompareTo(other.Tag) << 1) + this.Value.CompareTo(other.Value);
        }
    }
}
