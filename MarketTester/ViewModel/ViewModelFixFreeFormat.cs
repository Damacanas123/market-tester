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
using MarketTester.Model;
using MarketTester.Helper;

namespace MarketTester.ViewModel
{
    public class ViewModelFixFreeFormat :BaseNotifier
    {
        private HashSet<string> CantBeEditedTags { get; set; } = new HashSet<string>()
        {
            "8","9","10"
        };
        public ViewModelFixFreeFormat()
        {
            ProtocolType = ProtocolType.Fix50sp2;
            Settings.GetInstance().LanguageChangedEventHandler += OnLanguageChanged;
        }
        public ObservableCollection<TagValuePair> TagValuePairs { get; set; } = new ObservableCollection<TagValuePair>();
        public ObservableCollection<FreeFormatScheduleItem> ScheduleItems { get; set; } = new ObservableCollection<FreeFormatScheduleItem>();

        private ProtocolType protocolType;
        public ProtocolType ProtocolType
        {
            get { return protocolType; }
            set 
            { 
                protocolType = value;
                NotifyPropertyChanged(nameof(ProtocolType));
            }
        }

        private Channel selectedChannel;

        public Channel SelectedChannel
        {
            get { return selectedChannel; }
            set 
            { 
                selectedChannel = value;
                NotifyPropertyChanged(nameof(SelectedChannel));
            }
        }

        private FreeFormatScheduleItem selectedScheduleItem;

        public FreeFormatScheduleItem SelectedScheduleItem
        {
            get { return selectedScheduleItem; }
            set
            {
                selectedScheduleItem = value;
                NotifyPropertyChanged(nameof(SelectedScheduleItem));
            }
        }



        private void OnLanguageChanged()
        {
            if (InfoTextResourceKey != null)
                InfoText = App.Current.Resources[InfoTextResourceKey].ToString();
        }
        private string infoTextResourceKey;

        public string InfoTextResourceKey
        {
            get { return infoTextResourceKey; }
            set
            {
                if (App.Current.Resources.Contains(value))
                {

                    infoTextResourceKey = value;
                    InfoText = App.Current.Resources[value].ToString();
                    NotifyPropertyChanged(nameof(InfoTextResourceKey));
                }
            }
        }


        private string infoText;

        public string InfoText
        {
            get { return infoText; }
            set
            {
                infoText = value;
                NotifyPropertyChanged(nameof(InfoText));
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
        private string textDelay;

        public string TextDelay
        {
            get { return textDelay; }
            set
            {
                textDelay = value;
                NotifyPropertyChanged(nameof(TextDelay));
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
             && !string.IsNullOrWhiteSpace(TextValue) && !CantBeEditedTags
            if (string.IsNullOrWhiteSpace(TextTag))
            {
                InfoTextResourceKey = 
            }
            TagValuePairs.Add(new TagValuePair(TextTag, TextValue));
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


        #region CommandSendMessage
        public BaseCommand CommandSendMessage { get; set; }
        public void CommandSendMessageExecute(object param)
        {

        }
        public bool CommandSendMessageCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandAddMessageToSchedule
        public BaseCommand CommandAddMessageToSchedule { get; set; }
        public void CommandAddMessageToScheduleExecute(object param)
        {
            if (!string.IsNullOrWhiteSpace(TextTag) && !string.IsNullOrWhiteSpace(TextValue) && SelectedChannel != null)
            {
                
                int delay = int.Parse(TextDelay, CultureInfo.InvariantCulture);
                FreeFormatScheduleItem item = new FreeFormatScheduleItem(delay, Message, SelectedChannel);
                ScheduleItems.Add(item);
            }
        }
        public bool CommandAddMessageToScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandSelectMessageFromSchedule
        public BaseCommand CommandSelectMessageFromSchedule { get; set; }
        public void CommandSelectMessageFromScheduleExecute(object param)
        {
            if(SelectedScheduleItem != null)
            {

                TagValuePairs.Clear();
                Dictionary<int,string> tagValuePairs = Util.GetTagValuePairs(SelectedScheduleItem.Message);
                foreach(KeyValuePair<int,string> pair in tagValuePairs)
                {
                    if(!CantBeEditedTags.Contains(pair.Key.ToString()))
                        TagValuePairs.Add(new TagValuePair(pair.Key.ToString(), pair.Value));
                }
            }
        }
        public bool CommandSelectMessageFromScheduleCanExecute()
        {
            return true;
        }
        #endregion

        #endregion
    }
}
