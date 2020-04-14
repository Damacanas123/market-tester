using MarketTester.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.OrderHistoryFix
{
    public class TagValueDescription : BaseNotifier
    {
        private string tag;

        public string Tag
        {
            get { return tag; }
            set
            {
                tag = value;
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
                NotifyPropertyChanged(nameof(Value));
            }
        }


        private string valueDescription;

        public string ValueDescription
        {
            get { return valueDescription; }
            set
            {
                valueDescription = value;
                NotifyPropertyChanged(nameof(ValueDescription));
            }
        }

        public TagValueDescription(string tag,string value,string tagDescription,string valueDescription)
        {
            Tag = tag;
            Value = value;
            TagDescription = tagDescription;
            ValueDescription = valueDescription;
        }
    }
}
