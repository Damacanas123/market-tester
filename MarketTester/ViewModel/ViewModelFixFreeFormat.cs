﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Base;
using MarketTester.Model.FixFreeFormat;
using MarketTester.UI.Usercontrol;

using BackOfficeEngine.Helper;
using QuickFix.Fields;
using System.Globalization;

using BackOfficeEngine;
using MarketTester.Model;
using MarketTester.Helper;
using System.Threading;
using Microsoft.Win32;
using MarketTester.UI.Popup;
using System.IO;
using System.Windows;
using MarketTester.Connection;

namespace MarketTester.ViewModel
{
    public class ViewModelFixFreeFormat :BaseNotifier
    {
        
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
            CommandAddSchedule = new BaseCommand(CommandAddScheduleExecute, CommandAddScheduleCanExecute);
            CommandSaveFile = new BaseCommand(CommandSaveFileExecute, CommandSaveFileCanExecute);
            CommandLoadFile = new BaseCommand(CommandLoadFileExecute, CommandLoadFileCanExecute);
            CommandStartSchedule = new BaseCommand(CommandStartScheduleExecute, CommandStartScheduleCanExecute);
            CommandClearSchedule = new BaseCommand(CommandClearScheduleExecute, CommandClearScheduleCanExecute);
            CommandRemoveItemFromSchedule = new BaseCommand(CommandRemoveItemFromScheduleExecute, CommandRemoveItemFromScheduleCanExecute);
            CommandMoveMessageDown = new BaseCommand(CommandMoveMessageDownExecute, CommandMoveMessageDownCanExecute);
            CommandMoveMessageUp = new BaseCommand(CommandMoveMessageUpExecute, CommandMoveMessageUpCanExecute);
            CommandAddMessageSavedMessages = new BaseCommand(CommandAddMessageSavedMessagesExecute, CommandAddMessageSavedMessagesCanExecute);
            CommandDeleteSavedMessage = new BaseCommand(CommandDeleteSavedMessageExecute, CommandDeleteSavedMessageCanExecute);
            CommandDeleteSchedule = new BaseCommand(CommandDeleteScheduleExecute, CommandDeleteScheduleCanExecute);


            if (SavedMessage.SavedMessages.Count == 0)
            {
                SavedMessage.Load();
            }
            GetCurrentActiveChannels();
            Connection.Connector.ActiveChannels.CollectionChanged += OnActiveChannelsCollectionChanged;
            SelectedSchedule = new FreeFormatSchedule() { Name = "Schedule1" };
            Schedules.Add(SelectedSchedule);

            TagValuePairs.Add(new TagValuePair("35", "D"));
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
        private string savedMessageName;

        public string SavedMessageName
        {
            get { return savedMessageName; }
            set
            {
                savedMessageName = value;
                NotifyPropertyChanged(nameof(SavedMessageName));
            }
        }

        private SavedMessage selectedSavedMessage;

        public SavedMessage SelectedSavedMessage
        {
            get { return selectedSavedMessage; }
            set
            {
                selectedSavedMessage = value;
                TagValuePairs.Clear();
                if(selectedSavedMessage != null)
                {
                    foreach (TagValuePair pair in selectedSavedMessage.GetTagValuePairs())
                    {
                        TagValuePairs.Add(new TagValuePair(pair));
                    }
                }                
                NotifyPropertyChanged(nameof(SelectedSavedMessage));
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
                textTag = Util.RemoveNonNumeric(value);                
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
                textDelay = Util.RemoveNonNumeric(value);
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

        private void GetCurrentActiveChannels()
        {
            foreach (Channel channel in Connector.ActiveChannels)
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


        #region commands 


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

            if(TagValuePairs.FirstOrDefault((o) => o.Tag == TextTag) == null)
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
            if(SelectedTagValuePairIndex < TagValuePairs.Count && SelectedTagValuePairIndex > -1)
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
                FreeFormatScheduleItem item = new FreeFormatScheduleItem(delay, Message, SelectedChannel.Name);
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
                Dictionary<int,string> tagValuePairs = MarketTesterUtil.GetTagValuePairs(SelectedScheduleItem.Message);
                foreach(KeyValuePair<int,string> pair in tagValuePairs)
                {
                    if(!Fix.CantBeEditedTags.Contains(pair.Key.ToString()))
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
            Schedules.Add(new FreeFormatSchedule() { Name = "Schedule" + (Schedules.Count + 1).ToString(CultureInfo.InvariantCulture) });
        }
        public bool CommandAddScheduleCanExecute()
        {
            return true;
        }
        #endregion


        private static string SaveDelimiter = $"dsdfprtotoelxlvmbd{Environment.NewLine}";
        private const string FreeFormatSaveFileExtension = "ffsf";
        #region CommandSaveFile
        public BaseCommand CommandSaveFile { get; set; }
        public void CommandSaveFileExecute(object param)
        {
            string filePath = UIUtil.SaveFileDialog(new string[] { FreeFormatSaveFileExtension },MarketTesterUtil.APPLICATION_FREEFORMATSCHEDULE_DIR);
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                new Thread(() =>
                {
                    lock (Schedules)
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        foreach (FreeFormatSchedule schedule in Schedules)
                        {                            
                            Util.AppendStringToFile(filePath, SaveDelimiter + schedule.SaveString);
                        }
                    }
                }).Start();
            }
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
            string filePath = "";            
            try
            {
                filePath = UIUtil.OpenFileDialog(new string[] { FreeFormatSaveFileExtension }, MarketTesterUtil.APPLICATION_FREEFORMATSCHEDULE_DIR);
                List<FreeFormatSchedule> tempList = new List<FreeFormatSchedule>(10);
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    new Thread(() =>
                    {
                        try
                        {
                            string content = MarketTesterUtil.ReadFile(filePath);
                            string[] splits = content.Split(new string[] { SaveDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string saveString in splits)
                            {
                                FreeFormatSchedule schedule = FreeFormatSchedule.Load(saveString);
                                tempList.Add(schedule);
                            }
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                Schedules.Clear();
                                foreach (FreeFormatSchedule schedule in tempList)
                                {
                                    Schedules.Add(schedule);
                                }
                                SelectedSchedule = Schedules[0];
                            });
                        }
                        catch
                        {
                            InfoTextResourceKey = ResourceKeys.StringUnknownErrorOccured;
                        }
                        
                    }).Start();
                }
                
            }
            catch (FormatException ex)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    UserControlErrorPopup popup = new UserControlErrorPopup(ResourceKeys.StringCannotParseDelayFromFile);
                    popup.SetExtraText(Environment.NewLine + filePath);
                    PopupManager.OpenErrorPopup(popup);
                });
            }
            catch(Exception ex)
            {
                UserControlErrorPopup popup = new UserControlErrorPopup();
                popup.SetExtraText(filePath);
                PopupManager.OpenErrorPopup(popup);
            }
        }
        public bool CommandLoadFileCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandStartSchedule
        public BaseCommand CommandStartSchedule { get; set; }
        public void CommandStartScheduleExecute(object param)
        {
            Connector connector = Connector.GetInstance();
            foreach (FreeFormatScheduleItem item in SelectedSchedule.Items)
            {
                if (item.IsSelected)
                {
                    if (!connector.CheckChannelConnection(item.Channel))
                    {
                        InfoTextResourceKey = ResourceKeys.StringChannelNotConnected;
                        return;
                    }
                }
            }
            InfoTextResourceKey = ResourceKeys.StringStartedSchedule;
            Util.ThreadStart(() =>
            {
                try
                {

                    foreach (FreeFormatScheduleItem item in SelectedSchedule.Items)
                    {
                        if (item.IsSelected)
                        {
                            Thread.Sleep(item.Delay);
                            connector.SendMessage(item.Channel, item.Message, !OverrideSessionTags);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Util.LogDebugError(ex);
                    App.Invoke(() =>
                    {
                        InfoTextResourceKey = ResourceKeys.StringCouldntFinishSchedule;
                    });
                    
                }
                App.Invoke(() =>
                {
                    InfoTextResourceKey = ResourceKeys.StringFinishedSchedule;
                });
            });
        }
        public bool CommandStartScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandClearSchedule
        public BaseCommand CommandClearSchedule { get; set; }
        public void CommandClearScheduleExecute(object param)
        {
            SelectedSchedule.Items.Clear();
        }
        public bool CommandClearScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandRemoveItemFromSchedule
        public BaseCommand CommandRemoveItemFromSchedule { get; set; }
        public void CommandRemoveItemFromScheduleExecute(object param)
        {
            SelectedSchedule.Items.Remove(SelectedScheduleItem);
        }
        public bool CommandRemoveItemFromScheduleCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandMoveMessageUp
        public BaseCommand CommandMoveMessageUp { get; set; }
        public void CommandMoveMessageUpExecute(object param)
        {
            if(SelectedScheduleItemIndex > 0)
            {
                FreeFormatScheduleItem temp = SelectedSchedule.Items[SelectedScheduleItemIndex];
                SelectedSchedule.Items[SelectedScheduleItemIndex] = SelectedSchedule.Items[SelectedScheduleItemIndex - 1];
                SelectedSchedule.Items[SelectedScheduleItemIndex - 1] = temp;
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
            if (SelectedScheduleItemIndex < SelectedSchedule.Items.Count - 1)
            {
                FreeFormatScheduleItem temp = SelectedSchedule.Items[SelectedScheduleItemIndex];
                SelectedSchedule.Items[SelectedScheduleItemIndex] = SelectedSchedule.Items[SelectedScheduleItemIndex + 1];
                SelectedSchedule.Items[SelectedScheduleItemIndex + 1] = temp;
            }
        }
        public bool CommandMoveMessageDownCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandAddMessageSavedMessages
        public BaseCommand CommandAddMessageSavedMessages { get; set; }
        public void CommandAddMessageSavedMessagesExecute(object param)
        {
            if (string.IsNullOrWhiteSpace(SavedMessageName))
            {
                InfoTextResourceKey = ResourceKeys.StringEnterAMessageName;
                return;
            }
            if(TagValuePairs.Count == 0)
            {
                InfoTextResourceKey = ResourceKeys.StringNoTagValuePairSet;
                return;
            }
            SavedMessage m = new SavedMessage();
            m.Name = SavedMessageName;
            foreach(TagValuePair pair in TagValuePairs)
            {
                m.AddTagValuePair(new TagValuePair(pair));
            }
            SavedMessage.SavedMessages.Add(m);
            SavedMessage.Save();
        }
        public bool CommandAddMessageSavedMessagesCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandDeleteSavedMessage
        public BaseCommand CommandDeleteSavedMessage { get; set; }
        public void CommandDeleteSavedMessageExecute(object param)
        {
            if(SelectedSavedMessage != null)
            {
                SavedMessage.SavedMessages.Remove(SelectedSavedMessage);
                SavedMessage.Save();
            }
        }
        public bool CommandDeleteSavedMessageCanExecute()
        {
            return true;
        }
        #endregion



        #endregion
    }
}
