using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Base;
using MarketTester.Model.FixFreeFormat;

using BackOfficeEngine.Helper;
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
        public string Message
        {
            get
            {
                List<(string, string)> tagValuesArr = new List<(string, string)>();
                
                foreach(TagValuePair pair in TagValuePairs)
                {
                    if (pair.IsSelected)
                    {
                        tagValuesArr.Add((pair.Tag, pair.Value));
                    }                    
                }
                return Fix.GetFixString(ProtocolType,tagValuesArr);
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
