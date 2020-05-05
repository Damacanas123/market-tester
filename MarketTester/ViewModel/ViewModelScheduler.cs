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
using BackOfficeEngine.Model;
using Microsoft.Win32;
using MarketTester.Connection;
using BackOfficeEngine.ParamPacker;
using System.IO;
using BackOfficeEngine;

namespace MarketTester.ViewModel
{
    public class ViewModelScheduler : BaseNotifier
    {
        private static HashSet<string> IDTags { get; set; } = new HashSet<string>()
        {
            "11","41","37"
        };
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
            CommandSaveFile = new BaseCommand(CommandSaveFileExecute, CommandSaveFileCanExecute);
            CommandLoadFile = new BaseCommand(CommandLoadFileExecute, CommandLoadFileCanExecute);
            CommandSendMessage = new BaseCommand(CommandSendMessageExecute, CommandSendMessageCanExecute);
            CommandMoveMessageUp = new BaseCommand(CommandMoveMessageUpExecute, CommandMoveMessageUpCanExecute);
            CommandMoveMessageDown = new BaseCommand(CommandMoveMessageDownExecute, CommandMoveMessageDownCanExecute);


            Settings.GetInstance().LanguageChangedEventHandler += OnLanguageChange;

            Schedules.Add(new Scheduler("Schedule1"));
            SelectedSchedule = Schedules[0];
            GetCurrentActiveChannels();
            Connector.ActiveChannels.CollectionChanged += OnActiveChannelsCollectionChanged;
            
        }

        private void GetCurrentActiveChannels()
        {
            foreach(Channel channel in Connector.ActiveChannels)
            {
                if (!Channels.Contains(channel))
                {
                    Channels.Add(channel);
                }
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
                    lock (Channels)
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
                    }
                });
            }
        }

        public void OnLanguageChange()
        {
            if(InfoTextResourceKey != null)
                InfoText = App.Current.Resources[InfoTextResourceKey].ToString();
        }

        private bool preEvaluateQuantities;

        public bool PreEvaluateQuantities
        {
            get { return preEvaluateQuantities; }
            set
            {
                if(SelectedSchedule != null)
                {
                    SelectedSchedule.PreEvaluateReplaceQuantities = value;
                }
                preEvaluateQuantities = value;
                NotifyPropertyChanged(nameof(PreEvaluateQuantities));
            }
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
                textPrice = Util.RemoveNonNumericKeepDot(value);
                NotifyPropertyChanged(nameof(TextPrice));
            }
        }

        private string textQuantity;

        public string TextQuantity
        {
            get { return textQuantity; }
            set
            {
                textQuantity = Util.RemoveNonNumeric(value);
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

       

        private DateTime expireDate;

        public DateTime ExpireDate
        {
            get { return expireDate; }
            set
            {
                expireDate = value;
                NotifyPropertyChanged(nameof(ExpireDate));
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

        private string textPriceOffset = "0.00";

        public string TextPriceOffset
        {
            get { return textPriceOffset; }
            set
            {
                textPriceOffset = Util.RemoveNonNumericKeepDotAndDash(value);
                NotifyPropertyChanged(nameof(TextPriceOffset));
            }
        }

        private string textQuantityMultiplier = "1";

        public string TextQuantityMultiplier
        {
            get { return textQuantityMultiplier; }
            set
            {
                textQuantityMultiplier = Util.RemoveNonNumeric(value);
                NotifyPropertyChanged(nameof(TextQuantityMultiplier));
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
            if (IDTags.Contains(TextTag))
            {
                InfoTextResourceKey = ResourceKeys.StringCantEditIdTags;
                return;
            }

            if (TagValuePairs.FirstOrDefault((o) => o.Tag == TextTag) == null)
            {
                TagValuePairs.Add(new TagValuePair(TextTag, TextValue));
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


        #region CommandAddMessageToSchedule
        public BaseCommand CommandAddMessageToSchedule { get; set; }
        public void CommandAddMessageToScheduleExecute(object param)
        {
            if (!CheckMandatoryFieldsOnMessageAdd())
                return;
            SchedulerRawItem item = SelectedSchedule.PrepareScheduleItem(SelectedMsgType,
                TextAccount,
                SelectedSide,
                SelectedOrdType,
                SelectedTimeInForce,
                TextQuantity,
                TextSymbol,
                (SelectedTimeInForce == TimeInForce.GoodTillDate ? ExpireDate : DateTime.MinValue),
                (SelectedOrdType == OrdType.Limit ? TextPrice : null),
                TextAllocID,
                (SelectedScheduleItem != null && (SelectedMsgType == MsgType.Replace || SelectedMsgType == MsgType.Cancel) ? SelectedScheduleItem.SchedulerOrderID : null),
                SelectedChannel.Name,
                TextDelay
                );
            SelectedSchedule.AddItem(item);
        }

        private bool CheckMandatoryFieldsOnMessageAdd()
        {
            if (SelectedChannel == null)
            {
                InfoTextResourceKey = ResourceKeys.StringPleaseSelectAChannel;
                return false;
            }
            if (string.IsNullOrWhiteSpace(TextAccount))
            {
                InfoTextResourceKey = ResourceKeys.StringAccountCantBeEmpty;
                return false;
            }
            if (string.IsNullOrWhiteSpace(TextSymbol))
            {
                InfoTextResourceKey = ResourceKeys.StringSymbolCantBeEmpty;
                return false;
            }
            if (string.IsNullOrWhiteSpace(TextQuantity))
            {
                InfoTextResourceKey = ResourceKeys.StringQuantityCantBeEmpty;
                return false;
            }
            if(SelectedOrdType == OrdType.Limit)
            {
                if (string.IsNullOrWhiteSpace(TextPrice))
                {
                    InfoTextResourceKey = ResourceKeys.StringPriceCantBeEmptyOnLimit;
                    return false;
                }
            }
            return true;
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
            if (string.IsNullOrWhiteSpace(TextPriceOffset))
            {
                InfoTextResourceKey = ResourceKeys.StringPriceOffsetEmpty;
                return;
            }
            if (string.IsNullOrWhiteSpace(TextQuantityMultiplier))
            {
                InfoTextResourceKey = ResourceKeys.StringQuantityMultiplierEmpty;
                return;
            }
            ScheduleNotRunning = false;
            Order.Orders.SupressNotification = true;
            new Thread(() =>
            {                
                try
                {
                    SelectedSchedule.PrepareSchedule(decimal.Parse(TextPriceOffset, CultureInfo.InvariantCulture),
                    decimal.Parse(TextQuantityMultiplier, CultureInfo.InvariantCulture), TagValuePairs.ToList());
                    App.Invoke(() =>
                    {
                        InfoTextResourceKey = ResourceKeys.StringStartedSchedule;
                    });
                    SelectedSchedule.StartSchedule(!OverrideSessionTags);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ScheduleNotRunning = true;
                        Order.Orders.SupressNotification = false;
                        InfoTextResourceKey = ResourceKeys.StringFinishedSchedule;
                    });
                }
                catch(BackOfficeEngine.Exceptions.ConnectorNotPresentException ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ScheduleNotRunning = true;
                        Order.Orders.SupressNotification = false;
                        InfoTextResourceKey = ResourceKeys.StringConnectionNotActive;
                    });
                }
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

        private const string FileFormat = "sche";
        private static string ScheduleSaveFileDelimeter = $"sdfedbsfgdwrgs{Environment.NewLine}";
        #region CommandSaveFile
        public BaseCommand CommandSaveFile { get; set; }
        public void CommandSaveFileExecute(object param)
        {
            string filePath = UIUtil.SaveFileDialog(new string[] { FileFormat },MarketTesterUtil.APPLICATION_NONFREEFORMATSCHEDULE_DIR);
            new Thread(() =>
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    foreach (Scheduler schedule in Schedules)
                    {
                        Util.AppendStringToFile(filePath, ScheduleSaveFileDelimeter + schedule.SaveSchedule());
                    }
                }
            }).Start();
        }
        public bool CommandSaveFileCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandLoadFile
        public BaseCommand CommandLoadFile { get; set; }
        public void CommandLoadFileExecute(object param)
        {
            
            string filePath = UIUtil.OpenFileDialog(new string[] { FileFormat }, MarketTesterUtil.APPLICATION_NONFREEFORMATSCHEDULE_DIR);
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                Schedules.Clear();
                new Thread(() =>
                {
                    string content = MarketTesterUtil.ReadFile(filePath);
                    string[] scheduleStrings = content.Split(new string[] { ScheduleSaveFileDelimeter },StringSplitOptions.RemoveEmptyEntries);
                    foreach(string scheduleString in scheduleStrings)
                    {
                        Scheduler scheduler = new Scheduler("dummy");
                        scheduler.LoadSchedule(scheduleString.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries));
                        App.Current.Dispatcher.Invoke(() =>
                        {
                             Schedules.Add(scheduler);
                        });
                    }
                    App.Invoke(() =>
                    {
                        if(Schedules.Count > 0)
                            SelectedSchedule = Schedules[0];
                    });
                }).Start();
            }
        }
        public bool CommandLoadFileCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandSendMessage
        public BaseCommand CommandSendMessage { get; set; }
        public void CommandSendMessageExecute(object param)
        {
            if (!CheckMandatoryFieldsOnMessageAdd())
                return;
            if (SelectedMsgType != MsgType.New)
            {
                InfoTextResourceKey = ResourceKeys.StringOnlyNewMessage;
                return;
            }
            Connector connector = Connector.GetInstance();
            if(SelectedOrdType == OrdType.Limit)
            {
                connector.SendMessageNew(SelectedChannel, new NewMessageParameters(SelectedChannel.ProtocolType, TextAccount, TextSymbol,
                    decimal.Parse(TextQuantity, CultureInfo.InvariantCulture), SelectedSide, SelectedTimeInForce, SelectedOrdType,
                    decimal.Parse(textPrice, CultureInfo.InvariantCulture)));
            }
            else
            {
                connector.SendMessageNew(SelectedChannel, new NewMessageParameters(SelectedChannel.ProtocolType, TextAccount, TextSymbol,
                    decimal.Parse(TextQuantity, CultureInfo.InvariantCulture), SelectedSide, SelectedTimeInForce, SelectedOrdType));
            }
        }
        public bool CommandSendMessageCanExecute()
        {
            return true;
        }
        #endregion

        #region CommandMoveMessageUp
        public BaseCommand CommandMoveMessageUp { get; set; }
        public void CommandMoveMessageUpExecute(object param)
        {
            if (SelectedScheduleItemIndex < SelectedSchedule.scheduleRaw.Count && SelectedScheduleItemIndex > 0)
            {
                if(SelectedSchedule.SwapItem(SelectedScheduleItemIndex, SelectedScheduleItemIndex - 1))
                    SelectedScheduleItemIndex--;
            }
        }
        public bool CommandMoveMessageUpCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandMoveMessageDown
        public BaseCommand CommandMoveMessageDown { get; set; }
        public void CommandMoveMessageDownExecute(object param)
        {
            if (SelectedScheduleItemIndex < SelectedSchedule.scheduleRaw.Count - 1 && SelectedScheduleItemIndex > -1)
            {
                if(SelectedSchedule.SwapItem(SelectedScheduleItemIndex, SelectedScheduleItemIndex + 1))
                    SelectedScheduleItemIndex++;
            }
        }
        public bool CommandMoveMessageDownCanExecute()
        {
            return true;
        }
        #endregion

        #endregion
    }
}
