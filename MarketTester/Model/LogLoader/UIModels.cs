using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixHelper;
using FixLogAnalyzer;
using MarketTester.Base;

namespace MarketTester.Model.LogLoader
{
    public class MsgTypeUI :BaseNotifier
    {

        public MsgTypeUI(string msgType)
        {
            MsgType = msgType;
            Explanation = AllFixTags.GetInstance().GetValueExplanation(Tags.MsgType, msgType);
        }
        private string msgType;

        public string MsgType
        {
            get { return msgType; }
            set
            {
                msgType = value;
                NotifyPropertyChanged(nameof(MsgType));
            }
        }

        private string explanation;

        public string Explanation
        {
            get { return explanation; }
            set
            {
                explanation = value;
                NotifyPropertyChanged(nameof(Explanation));
            }
        }

       

    }
    public class TagUI : BaseNotifier
    {
        public TagUI(int tag,int occurenceNum)
        {
            Tag = tag;
            Explanation = AllFixTags.GetInstance().GetTagExplanation(tag);
            OccurenceNum = occurenceNum;
        }
        public TagUI(string tagString,int occurenceNum) : this(int.Parse(tagString, CultureInfo.InvariantCulture),occurenceNum)
        {

        }

        private int occurenceNum;

        public int OccurenceNum
        {
            get { return occurenceNum; }
            set
            {
                occurenceNum = value;
                NotifyPropertyChanged(nameof(OccurenceNum));
            }
        }

        private int tag;

        public int Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                NotifyPropertyChanged(nameof(Tag));
            }
        }

        private string explanation;

        public string Explanation
        {
            get { return explanation; }
            set
            {
                explanation = value;
                NotifyPropertyChanged(nameof(Explanation));
            }
        }
    }

    public class ValueUI : BaseNotifier
    {

        public ValueUI(int tag,string value,int occurenceNum)
        {
            Value = value;
            OccurenceNum = occurenceNum;
            Explanation = AllFixTags.GetInstance().GetValueExplanation(tag, value);
        }
        private string value;

        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }

        private string explanation;

        public string Explanation
        {
            get { return explanation; }
            set
            {
                explanation = value;
                NotifyPropertyChanged(nameof(Explanation));
            }
        }

        private int occurenceNum;

        public int OccurenceNum
        {
            get { return occurenceNum; }
            set
            {
                occurenceNum = value;
                NotifyPropertyChanged(nameof(OccurenceNum));
            }
        }



    }
}
