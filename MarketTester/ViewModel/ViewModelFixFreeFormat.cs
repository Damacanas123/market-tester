using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Base;
using MarketTester.Model.FixFreeFormat;

using BackOfficeEngine.Model;
using QuickFix.Fields;
using System.Globalization;

using BackOfficeEngine;

namespace MarketTester.ViewModel
{
    public class ViewModelFixFreeFormat :BaseNotifier
    {
        public ViewModelFixFreeFormat()
        {
            ProtocolType = ProtocolType.Fix50sp2;
        }
        public ObservableCollection<TagValuePair> TagValuePairs { get; set; } = new ObservableCollection<TagValuePair>();
        private string ProtocolString = "";
        private ProtocolType protocolType;
        public ProtocolType ProtocolType
        {
            get { return protocolType; }
            set 
            { 
                protocolType = value;
                //switch (protocolType)
                //{
                //    case ProtocolType.Fix50sp2:
                //        ProtocolString = "FIXT.1.1"
                //}
                NotifyPropertyChanged(nameof(ProtocolType));
            }
        }

        private string textTag;
        public string TextTag
        {
            get { return textTag; }
            set
            {
                textTag = value;
                NotifyPropertyChanged(nameof(TextTag));
            }
        }
        private string textValue;
        public string TextValue
        {
            get { return textValue; }
            set
            {
                textValue = value;
                NotifyPropertyChanged(nameof(TextValue));
            }
        }
        public int SelectedTagValuePairIndex { get; set; }
        public QuickFixMessage Message
        {
            get
            {
                QuickFixMessage msg = new QuickFixMessage();
                //msg.SetField(new StringField(8,))
                foreach(TagValuePair pair in TagValuePairs)
                {
                    msg.SetField(new StringField(int.Parse(pair.Tag, CultureInfo.InvariantCulture), pair.Value));
                }
                return msg;
            }
        }

        #region commands 
        #region CommandAddTagValuePair
        public BaseCommand CommandAddTagValuePair { get; set; }
        public void CommandAddTagValuePairExecute(object param)
        {
            if(!string.IsNullOrWhiteSpace(TextTag) && !string.IsNullOrWhiteSpace(TextValue))
            {
                TagValuePairs.Add(new TagValuePair(TextTag,TextValue));
            }
        }
        public bool CommandAddTagValuePairCanExecute()
        {
            return true;
        }

        #endregion

        #region CommandDeleteTagValuePair
        public BaseCommand CommandDeleteTagValuePair { get; set; }
        public void CommandDeleteTagValuePairExecute(object param)
        {
            TagValuePairs.RemoveAt(SelectedTagValuePairIndex);
        }
        public bool CommandDeleteTagValuePairCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandClearTagValuePairs
        public BaseCommand CommandClearTagValuePairs { get; set; }
        public void CommandClearTagValuePairsExecute(object param)
        {
            TagValuePairs.Clear();
        }
        public bool CommandClearTagValuePairsCanExecute()
        {
            return true;
        }
        #endregion

        #endregion
    }
}
