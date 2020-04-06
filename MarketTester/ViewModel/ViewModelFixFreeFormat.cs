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
            CommandAddMessageToSchedule = new BaseCommand(CommandAddMessageToScheduleExecute, CommandAddMessageToScheduleCanExecute);
            CommandAddTagValuePair = new BaseCommand(CommandAddTagValuePairExecute, CommandAddTagValuePairCanExecute);
            CommandClearTagValuePairs = new BaseCommand(CommandClearTagValuePairsExecute, CommandClearTagValuePairsCanExecute);
            CommandDeleteTagValuePair = new BaseCommand(CommandDeleteTagValuePairExecute, CommandDeleteTagValuePairCanExecute);
            CommandSelectMessageFromSchedule = new BaseCommand(CommandSelectMessageFromScheduleExecute, CommandSelectMessageFromScheduleCanExecute);
            CommandSendMessage = new BaseCommand(CommandSendMessageExecute, CommandSendMessageCanExecute);
            Connection.Connector.ActiveChannels.CollectionChanged += OnActiveChannelsCollectionChanged;
            SelectedSchedule = new FreeFormatSchedule() { Name = "Scheduler1" };
            Schedules.Add(SelectedSchedule);
        }
        public ObservableCollection<FreeFormatSchedule> Schedules { get; set; } = new ObservableCollection<FreeFormatSchedule>();
        public ObservableCollection<TagValuePair> TagValuePairs { get; set; } = new ObservableCollection<TagValuePair>();
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();

        private bool overrideSessionTags;

        public bool OverrideSessionTags
        {
            get { return overrideSessionTags; }
            set
            {
                overrideSessionTags = value;
                NotifyPropertyChanged(nameof(OverrideSessionTags));
            }
        }

        private ProtocolType protocolType;
        public ProtocolType ProtocolType
        {
            get { return protocolType; }
            set 
            { 
                protocolType = value;
                NotifyPropertyChanged(nameof(ProtocolType));
                Dummy.Add(5);
            }
        }

        public List<ProtocolType> AvailableProtocols { get; set; } = new List<ProtocolType>()
        {
            ProtocolType.Fix50sp2,ProtocolType.Fix44,ProtocolType.Fix42,ProtocolType.Fix40,ProtocolType.Fix41,ProtocolType.Fix43,ProtocolType.Fix50
        };

        public ObservableCollection<int> Dummy { get; set; } = new ObservableCollection<int>();

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

        private FreeFormatSchedule selectedSchedule;

        public FreeFormatSchedule SelectedSchedule
        {
            get { return selectedSchedule; }
            set
            {
                selectedSchedule = value;
                NotifyPropertyChanged(nameof(SelectedSchedule));
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

        private void OnActiveChannelsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
             UpdateChannelsCollection();
        }

        private void UpdateChannelsCollection()
        {
            if (App.Current != null)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Channels.Clear();
                    foreach (Channel channel in Connection.Connector.ActiveChannels)
                    {
                        if (channel.IsConnected)
                        {
                            if (!Channels.Contains(channel))
                                Channels.Add(channel);
                        }
                    }
                });
            }
        }


        #region commands 
        #region CommandAddTagValuePair
        public BaseCommand CommandAddTagValuePair { get; set; }
        public void CommandAddTagValuePairExecute(object param)
        {
            if (string.IsNullOrWhiteSpace(TextTag))
            {
                InfoTextResourceKey = ResourceKeys.StringTagEmpty;
                return;
            }
            if (string.IsNullOrWhiteSpace(TextValue))
            {
                InfoTextResourceKey = ResourceKeys.StringValueEmpty;
                return;
            }
            if (CantBeEditedTags.Contains(TextTag))
            {
                InfoTextResourceKey = ResourceKeys.StringCantEditTag;
                return;
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
            if(SelectedChannel == null)
            {
                InfoTextResourceKey = ResourceKeys.StringPleaseSelectAChannel;
                return;
            }
            if (!SelectedChannel.IsConnected)
            {
                InfoTextResourceKey = ResourceKeys.StringChannelNotConnected;
                return;
            }
            //invert the boolean value because function names makes sense then in Engine and IConnectors only inverting this boolean is unreasonable.
            Connection.Connector.GetInstance().SendMessage(SelectedChannel.ConnectorName, Message, !OverrideSessionTags);
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
            if (SelectedChannel != null)
            {
                int delay = 0;
                if (!string.IsNullOrWhiteSpace(TextDelay))
                {
                    delay = int.Parse(TextDelay, CultureInfo.InvariantCulture);
                }                
                FreeFormatScheduleItem item = new FreeFormatScheduleItem(delay, Message, SelectedChannel);
                SelectedSchedule.Items.Add(item);
            }
            else
            {
                InfoTextResourceKey = ResourceKeys.StringPleaseSelectAChannel;
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


        #region CommandAddSchedule
        public BaseCommand CommandAddSchedule { get; set; }
        public void CommandAddScheduleExecute(object param)
        {

        }
        public bool CommandAddScheduleCanExecute()
        {
            return true;
        }
        #endregion

        #endregion
    }
}
