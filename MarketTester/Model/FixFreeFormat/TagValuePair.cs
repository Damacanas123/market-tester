using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Base;

namespace MarketTester.Model.FixFreeFormat
{
    public class TagValuePair :BaseNotifier
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
        private bool isSelected = true;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        public TagValuePair(string tag,string value)
        {
            Tag = tag;
            Value = value;
        }

        public TagValuePair(string tag, string value,bool isSelected)
        {
            Tag = tag;
            Value = value;
            IsSelected = isSelected;
        }

        public TagValuePair(TagValuePair other)
        {
            Tag = other.Tag;
            Value = other.Value;
            IsSelected = other.IsSelected;
        }
    }
}
