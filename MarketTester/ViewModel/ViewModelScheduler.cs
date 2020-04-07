using MarketTester.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackOfficeEngine.MessageEnums;
using MarketTester.Model;
using System.Collections.ObjectModel;
using MarketTester.Model.Scheduler;
using MarketTester.Model.FixFreeFormat;
using BackOfficeEngine.Helper;
using MarketTester.Helper;
using System.Globalization;
using System.Threading;

namespace MarketTester.ViewModel
{
    public class ViewModelScheduler : BaseNotifier
    {
        public ViewModelScheduler()
        {
            CommandAddMessageToSchedule = new BaseCommand(CommandAddMessageToScheduleExecute, CommandAddMessageToScheduleCanExecute);
            CommandAddSchedule = new BaseCommand(CommandAddScheduleExecute, CommandAddScheduleCanExecute);
            CommandAddTagValuePair = new BaseCommand(CommandAddTagValuePairExecute, CommandAddTagValuePairCanExecute);
            CommandClearTagValuePairs = new BaseCommand(CommandClearTagValuePairsExecute, CommandClearTagValuePairsCanExecute);
            CommandDeleteTagValuePair = new BaseCommand(CommandDeleteTagValuePairExecute, CommandDeleteTagValuePairCanExecute);
            CommandSelectMessageFromSchedule = new BaseCommand(CommandSelectMessageFromScheduleExecute, CommandSelectMessageFromScheduleCanExecute);
            CommandClearSchedule = new BaseCommand(CommandClearScheduleExecute, CommandClearScheduleCanExecute);
            CommandDeleteSchedule = new BaseCommand(CommandDeleteScheduleExecute, CommandDeleteScheduleCanExecute);
            CommandRemoveItemFromSchedule = new BaseCommand(CommandRemoveItemFromScheduleExecute, CommandRemoveItemFromScheduleCanExecute);
            CommandStartSchedule = new BaseCommand(CommandStartScheduleExecute, CommandStartScheduleCanExecute);

            Schedules.Add(new Scheduler("Schedule1"));
            SelectedSchedule = Schedules[0];
        }
        private string textAllocID;

        public string TextAllocID
        {
            get { return textAllocID; }
            set
            {
                textAllocID = value;
                NotifyPropertyChanged(nameof(TextAllocID));
            }
        }

        private string textTag;
        public string TextTag
        {
            get { return textTag; }
            set
            {
                textTag = Util.RemoveNonNumeric(value);
                NotifyPropertyChanged(nameof(TextTag));
            }
        }

        private int selectedScheduleItemIndex;

        public int SelectedScheduleItemIndex
        {
            get { return selectedScheduleItemIndex; }
            set
            {
                selectedScheduleItemIndex = value;
                NotifyPropertyChanged(nameof(SelectedScheduleItemIndex));
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

        private string textSymbol;

        public string TextSymbol
        {
            get { return textSymbol; }
            set
            {
                textSymbol = value;
                NotifyPropertyChanged(nameof(TextSymbol));
            }
        }

        private string textPrice;

        public string TextPrice
        {
            get { return textPrice; }
            set
            {
                textPrice = value;
                NotifyPropertyChanged(nameof(TextPrice));
            }
        }

        private string textQuantity;

        public string TextQuantity
        {
            get { return textQuantity; }
            set
            {
                textQuantity = value;
                NotifyPropertyChanged(nameof(TextQuantity));
            }
        }

        private string textAccount;

        public string TextAccount
        {
            get { return textAccount; }
            set
            {
                textAccount = value;
                NotifyPropertyChanged(nameof(TextAccount));
            }
        }

        private string textDelay = "50";

        public string TextDelay
        {
            get { return textDelay; }
            set
            {
                textDelay = Util.RemoveNonNumeric(value);
                NotifyPropertyChanged(nameof(TextDelay));
            }
        }

        private string textExpireDate;

        public string TextExpireDate
        {
            get { return textExpireDate; }
            set
            {
                textExpireDate = value;
                NotifyPropertyChanged(nameof(TextExpireDate));
            }
        }


        private MsgType msgType = MsgType.New;

        public MsgType SelectedMsgType
        {
            get { return msgType; }
            set
            {
                msgType = value;
                NotifyPropertyChanged(nameof(SelectedMsgType));
            }
        }

        private TimeInForce timeInForce = TimeInForce.Day;

        public TimeInForce SelectedTimeInForce
        {
            get { return timeInForce; }
            set
            {
                timeInForce = value;
                NotifyPropertyChanged(nameof(SelectedTimeInForce));
            }
        }

        private OrdType ordType = OrdType.Limit;

        public OrdType SelectedOrdType
        {
            get { return ordType; }
            set
            {
                ordType = value;
                NotifyPropertyChanged(nameof(SelectedOrdType));
            }
        }

        private Side side = Side.Buy;

        public Side SelectedSide
        {
            get { return side; }
            set
            {
                side = value;
                NotifyPropertyChanged(nameof(SelectedSide));
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

        private Scheduler selectedSchedule;

        public Scheduler SelectedSchedule
        {
            get { return selectedSchedule; }
            set
            {
                selectedSchedule = value;
                NotifyPropertyChanged(nameof(SelectedSchedule));
            }
        }

        private int selectedTagValuePairIndex;

        public int SelectedTagValuePairIndex
        {
            get { return selectedTagValuePairIndex; }
            set
            {
                selectedTagValuePairIndex = value;
                NotifyPropertyChanged(nameof(SelectedTagValuePairIndex));
            }
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

        private SchedulerRawItem selectedScheduleItem;

        public SchedulerRawItem SelectedScheduleItem
        {
            get { return selectedScheduleItem; }
            set
            {
                selectedScheduleItem = value;
                NotifyPropertyChanged(nameof(SelectedScheduleItem));
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

        private bool scheduleNotRunning = true;

        public bool ScheduleNotRunning
        {
            get { return scheduleNotRunning; }
            set
            {
                scheduleNotRunning = value;
                NotifyPropertyChanged(nameof(ScheduleNotRunning));
            }
        }


        public ObservableCollection<Scheduler> Schedules { get; set; } = new ObservableCollection<Scheduler>();
        public ObservableCollection<TagValuePair> TagValuePairs { get; set; } = new ObservableCollection<TagValuePair>();
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();

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
            if (Fix.CantBeEditedTags.Contains(TextTag))
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


        #region CommandAddMessageToSchedule
        public BaseCommand CommandAddMessageToSchedule { get; set; }
        public void CommandAddMessageToScheduleExecute(object param)
        {
            SchedulerRawItem item = SelectedSchedule.PrepareScheduleItem(SelectedMsgType,
                TextAccount,
                SelectedSide,
                SelectedOrdType,
                SelectedTimeInForce,
                TextQuantity,
                TextSymbol,
                (SelectedTimeInForce == TimeInForce.GoodTillDate ? TextExpireDate : null),
                TextPrice,
                TextAllocID,
                (SelectedScheduleItem != null && SelectedMsgType == MsgType.Replace ? SelectedScheduleItem.SchedulerOrderID : null),
                SelectedChannel.Name,
                TextDelay
                );
            SelectedSchedule.AddItem(item);
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
            SchedulerRawItem item = SelectedScheduleItem;
            SelectedMsgType = item.MsgType;
            TextAccount = item.Account;
            SelectedSide = item.Side;
            SelectedOrdType = item.OrdType;
            SelectedTimeInForce = item.TimeInForce;
            TextQuantity = item.OrderQty.ToString(CultureInfo.InvariantCulture);
            TextSymbol = item.Symbol;
            SelectedTimeInForce = item.TimeInForce;
            TextPrice = item.Price.ToString(CultureInfo.InvariantCulture);
            TextAllocID = item.AllocID;
            SelectedChannel = Connection.Connector.ActiveChannels.FirstOrDefault((o) => o.Name == item.ConnectorName);
            TextDelay = item.Delay.ToString(CultureInfo.InvariantCulture);
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
            Schedules.Add(new Scheduler("Schedule" + (Schedules.Count + 1).ToString(CultureInfo.InvariantCulture)));
        }
        public bool CommandAddScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandRemoveItemFromSchedule
        public BaseCommand CommandRemoveItemFromSchedule { get; set; }
        public void CommandRemoveItemFromScheduleExecute(object param)
        {
            if(SelectedScheduleItemIndex > -1 && SelectedScheduleItemIndex < SelectedSchedule.scheduleRaw.Count)
            {
                SelectedSchedule.DeleteItem(SelectedScheduleItemIndex);
            }            
        }
        public bool CommandRemoveItemFromScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandClearSchedule
        public BaseCommand CommandClearSchedule { get; set; }
        public void CommandClearScheduleExecute(object param)
        {
            SelectedSchedule.ClearSchedule();
        }
        public bool CommandClearScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandStartSchedule
        public BaseCommand CommandStartSchedule { get; set; }
        public void CommandStartScheduleExecute(object param)
        {
            ScheduleNotRunning = false;
            SelectedSchedule.PrepareSchedule(0, 1);
            new Thread(() =>
            {
                
                SelectedSchedule.StartSchedule();
                App.Current.Dispatcher.Invoke(() =>
                {
                    ScheduleNotRunning = true;
                });
            }).Start();

        }
        public bool CommandStartScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandDeleteSchedule
        public BaseCommand CommandDeleteSchedule { get; set; }
        public void CommandDeleteScheduleExecute(object param)
        {
            Schedules.Remove(SelectedSchedule);
        }
        public bool CommandDeleteScheduleCanExecute()
        {
            return true;
        }
        #endregion

        #endregion
    }
}
