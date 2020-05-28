using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BackOfficeEngine.Model;
using MarketTester.Model.OrderHistoryFix;

using MarketTester.Worksheet;
using MarketTester.Helper;

using QuickFix.Fields;
using System.Globalization;
using MarketTester.Base;
using System.Threading;
using System.IO;
using FixHelper;
using System.Windows;
using MarketTester.Model.Scheduler;

namespace MarketTester.ViewModel
{
    public class ViewModelOrderHistoryFix : BaseNotifier
    {
        public ViewModelOrderHistoryFix()
        {
            CommandExportToCsv = new BaseCommand(CommandExportToCsvExecute, CommandExportToCsvCanExecute);
            CommandAppendToCsv = new BaseCommand(CommandAppendToCsvExecute, CommandAppendToCsvCanExecute);
            CommandExportToXLSX = new BaseCommand(CommandExportToXLSXExecute, CommandExportToXLSXCanExecute);
            CommandAppendToXLSX = new BaseCommand(CommandAppendToXLSXExecute, CommandAppendToXLSXCanExecute);
            CommandScheduleConsolidate = new BaseCommand(CommandScheduleConsolidateExecute, CommandScheduleConsolidateCanExecute);
            Settings.GetInstance().LanguageChangedEventHandler += OnLanguageChanged;
            IsConsolidateOpen = false;
        }

        public void OnLanguageChanged()
        {
            if(InfoTextResourceKey != null)
            {
                InfoText = App.Current.Resources[InfoTextResourceKey].ToString();
            }
        }
        private List<string> rejectedClOrdIDs = new List<string>();
        private Dictionary<string, int> clOrdIdMap = new Dictionary<string, int>();
        private int clOrdIdCounter = 0;
        private Order order;
        /// <summary>
        /// for use with single order history
        /// </summary>
        public Order Order
        {
            get { return order; }
            set
            {
                //can only be set once
                if(order == null)
                {
                    order = value;
                    //register to collection update event when order is set
                    order.Messages.CollectionChanged += MessageCollectionChanged;
                    lock (order.MessagesLock)
                    {
                        for (int i = 0; i < order.Messages.Count; i++)
                        {
                            AddMessage(order.Messages[i]);                            
                        }
                    }
                    ScheduleConsolidateEnabled = Scheduler.ScheduleGroupedMapMessages.ContainsKey(order.NonProtocolID);
                    NotifyPropertyChanged(nameof(Order));
                }                
            }
        }

        private bool scheduleConsolidateEnabled;

        public bool ScheduleConsolidateEnabled
        {
            get { return scheduleConsolidateEnabled; }
            set
            {
                scheduleConsolidateEnabled = value;
                NotifyPropertyChanged(nameof(ScheduleConsolidateEnabled));
            }
        }



        private List<Order> Orders { get; set; } = new List<Order>();
        /// <summary>
        /// for use with schedule grouped orders
        /// </summary>
        /// <param name="order"></param>
        public void AddOrder(Order order)
        {
            Orders.Add(order);
            order.Messages.CollectionChanged += MessageCollectionChanged;
        }

        /// <summary>
        /// onlye for use with schedule grouped orders
        /// </summary>
        /// <param name="msgs"></param>
        public void SetMessages(List<IMessage> msgs)
        {
            HistoryItems.Clear();
            foreach(IMessage msg in msgs)
            {
                AddMessage(msg);
            }
        }

        private string sheetName;

        public string SheetName
        {
            get { return sheetName; }
            set
            {
                sheetName = value;
                NotifyPropertyChanged(nameof(SheetName));
            }
        }

        private string consoliDateClickString;

        public string ConsolidateClickString
        {
            get { return consoliDateClickString; }
            set
            {
                consoliDateClickString = value;
                NotifyPropertyChanged(nameof(ConsolidateClickString));
            }
        }


        private bool isConsolidateOpen;
        private bool IsConsolidateOpen
        {
            get
            {
                return isConsolidateOpen;
            }
            set
            {
                isConsolidateOpen = value;
                if (!value)
                {
                    ConsolidateClickString = App.Current.Resources[ResourceKeys.StringConsolidateScheduleOrders].ToString();
                }
                else
                {
                    ConsolidateClickString = App.Current.Resources[ResourceKeys.StringBackToNormalView].ToString();
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

        private HistoryItem selectedHistoryItem;

        public HistoryItem SelectedHistoryItem
        {
            get { return selectedHistoryItem; }
            set
            {
                selectedHistoryItem = value;
                NotifyPropertyChanged(nameof(SelectedHistoryItem));
                if(selectedHistoryItem == null)
                {
                    return;
                }
                Dictionary<int,string> tagValuePairs = MarketTesterUtil.GetTagValuePairs(selectedHistoryItem.MessageString);
                TagValues.Clear();
                foreach(KeyValuePair<int,string> pair in tagValuePairs)
                {
                    int tag = pair.Key;
                    string Value = pair.Value;
                    if(tag != 9 && tag != 10)
                    {
                        string tagDescription = "";
                        if(AllFixTags.GetInstance().allTagToObjectMap.TryGetValue(tag,out AllFixTags.Tag tagStruct))
                        {
                            tagDescription = tagStruct.Name + " - (" + tagStruct.Type + ")";
                        }
                        string valueDescription = "";
                        Dictionary<string, string> valueMap = new Dictionary<string, string>();
                        if (AllFixTags.GetInstance().msgValueMap.TryGetValue(tag,out valueMap))
                        {
                            
                            if(valueMap.TryGetValue(Value,out valueDescription))
                            {
                                
                            }
                        }
                        TagValues.Add(new TagValueDescription(tag.ToString(), Value, tagDescription, valueDescription));
                    }
                }
            }
        }

        public ObservableCollection<TagValueDescription> TagValues { get; } = new ObservableCollection<TagValueDescription>();



        public ObservableCollectionEx<HistoryItem> HistoryItems { get; set; } = new ObservableCollectionEx<HistoryItem>();
       


        #region commands

        #region CommandExportToCsv
        public BaseCommand CommandExportToCsv { get; set; }
        public void CommandExportToCsvExecute(object param)
        {
            string filePath = UIUtil.SaveFileDialog(new string[] { "csv" }, MarketTesterUtil.APPLICATION_CSV_DIR);
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                new Thread(() =>
                {
                    try
                    {
                        Csv.CreateCsvFromHistoryItems(filePath, HistoryItems.ToList());
                    }
                    catch(IOException ex)
                    {
                        App.Invoke(() =>
                        {
                            InfoTextResourceKey = ResourceKeys.StringFileIsBeingUsed;
                        });                
                    }
                    catch(Exception ex)
                    {
                        App.Invoke(() =>
                        {
                            InfoTextResourceKey = ResourceKeys.StringCouldntExportCsv;
                        });                        
                    }
                    
                }).Start();                
            }
        }
        public bool CommandExportToCsvCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandAppendToCsv
        public BaseCommand CommandAppendToCsv { get; set; }
        public void CommandAppendToCsvExecute(object param)
        {
            string filePath = UIUtil.OpenFileDialog(new string[] { "csv" }, MarketTesterUtil.APPLICATION_CSV_DIR);
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                new Thread(() =>
                {
                    try
                    {
                        Csv.AppendCsvFromHistoryItems(filePath, HistoryItems.ToList());
                    }
                    catch (IOException ex)
                    {
                        App.Invoke(() =>
                        {
                            InfoTextResourceKey = ResourceKeys.StringFileIsBeingUsed;
                        });
                    }
                    catch (Exception ex)
                    {
                        App.Invoke(() =>
                        {
                            InfoTextResourceKey = ResourceKeys.StringCouldntExportCsv;
                        });
                    }

                }).Start();
            }
        }
        public bool CommandAppendToCsvCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandExportToXLSX
        public BaseCommand CommandExportToXLSX { get; set; }
        public void CommandExportToXLSXExecute(object param)
        {
            if (String.IsNullOrEmpty(SheetName))
            {
                InfoTextResourceKey = ResourceKeys.StringEnterASheetName;
                return;
            }
            string filePath = UIUtil.SaveFileDialog(new string[] { "xlsx" }, MarketTesterUtil.APPLICATION_XLSX_DIR);
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                string fixHistory = "";
                foreach (HistoryItem m in HistoryItems)
                {
                    fixHistory += m.ToString() + Environment.NewLine;
                }
                InfoTextResourceKey = ResourceKeys.StringStartingToWriteToXLSXFile;
                bool result; string resultMessageKey;
                new Thread(() =>
                {
                    (result, resultMessageKey) = Excel.SaveXLSXFromDataTable(HistoryItems.ToList(), filePath, fixHistory, SheetName);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        InfoTextResourceKey = resultMessageKey;
                    });
                }).Start();
                
                
            }
        }
        public bool CommandExportToXLSXCanExecute()
        {
            return true;
        }
        #endregion

        #region CommandAppendToXLSX
        public BaseCommand CommandAppendToXLSX { get; set; }
        public void CommandAppendToXLSXExecute(object param)
        {
            if (String.IsNullOrEmpty(SheetName))
            {
                InfoTextResourceKey = ResourceKeys.StringEnterASheetName;
                return;
            }
            string filePath = UIUtil.OpenFileDialog(new string[] { "xlsx" }, MarketTesterUtil.APPLICATION_XLSX_DIR);
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                InfoTextResourceKey = ResourceKeys.StringStartingToWriteToXLSXFile;
                Thread t = new Thread(() => 
                {
                    string fixHistory = "";
                    foreach (HistoryItem m in HistoryItems)
                    {
                        fixHistory += m.ToString() + Environment.NewLine;
                    }
                    bool result; string resultMessageKey;
                    (result, resultMessageKey) = Excel.AppendSheetSaveXLSXFromDataTable(HistoryItems.ToList(), filePath, fixHistory, SheetName);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        InfoTextResourceKey = resultMessageKey;
                    });
                });
                t.Start();
            }
        }
        public bool CommandAppendToXLSXCanExecute()
        {
            return true;
        }
        #endregion


        #region CommandScheduleConsolidate
        public BaseCommand CommandScheduleConsolidate { get; set; }
        public void CommandScheduleConsolidateExecute(object param)
        {
            if (!IsConsolidateOpen)
            {
                List<IMessage> msgs = Scheduler.ScheduleGroupedMapMessages[Order.NonProtocolID];
                SetMessages(msgs);
                HashSet<string> NonProtocolIDs = Scheduler.ScheduleGroupedMapNonProtocolIDs[Order.NonProtocolID];
                foreach(string nonProtocolId in NonProtocolIDs)
                {
                    Order order = Order.GetOrderByNonProtocolId(nonProtocolId);
                    if (order != null)
                    {
                        AddOrder(order);
                    }
                }
                IsConsolidateOpen = true;
            }
            else
            {
                List<IMessage> msgs = Order.Messages.ToList();
                SetMessages(msgs);
                foreach(Order order in Orders)
                {
                    order.Messages.CollectionChanged -= MessageCollectionChanged;
                }
                IsConsolidateOpen = false;
            }
        }
        public bool CommandScheduleConsolidateCanExecute()
        {
            return true;
        }
        #endregion
        #endregion


        private void MessageCollectionChanged(object sender,NotifyCollectionChangedEventArgs args)
        {
            App.Invoke(() =>
            {
                HistoryItems.SupressNotification = true;
                foreach (IMessage msg in args.NewItems)
                {
                    AddMessage(msg);
                }
                HistoryItems.SupressNotification = false;
            });
        }

        private object HistoryItemLock { get; set; } = new object();
        private void AddMessage(IMessage msg)
        {
            string oldClOrdId = MarketTesterUtil.GetField(msg, Tags.ClOrdID);
            string newClOrdId = "";
            string oldOrigClOrdId = MarketTesterUtil.GetField(msg, Tags.OrigClOrdID);
            string newOrigClOrdId = "";
            if (!oldClOrdId.Equals(""))
            {
                if (clOrdIdMap.ContainsKey(oldClOrdId))
                {
                    newClOrdId = clOrdIdMap[oldClOrdId].ToString();
                }
                else
                {
                    newClOrdId = clOrdIdCounter.ToString();
                    clOrdIdMap[oldClOrdId] = clOrdIdCounter;
                    clOrdIdCounter++;
                }
            }
            if (!oldOrigClOrdId.Equals(""))
            {
                if (clOrdIdMap.ContainsKey(oldOrigClOrdId))
                {
                    newOrigClOrdId = clOrdIdMap[oldOrigClOrdId].ToString();
                }
                else
                {
                    newOrigClOrdId = clOrdIdCounter.ToString();
                    clOrdIdMap[oldOrigClOrdId] = clOrdIdCounter;
                    clOrdIdCounter++;
                }
            }
            HistoryItem item = new HistoryItem(msg, newClOrdId, newOrigClOrdId);

            if (msg.GetGenericField(Tags.MsgType) == MsgType.ORDERCANCELREJECT)
            {
                rejectedClOrdIDs.Add(newClOrdId);
            }
            if (rejectedClOrdIDs.Contains(newOrigClOrdId))
            {
                rejectedClOrdIDs.Add(newClOrdId);
                item.IsPreGenerated = true;
            }
            item.TimeStamp = msg.TimeStamp;
            if(HistoryItems.Count == 0)
            {
                item.NormalizedTimeStamp = new TimeSpan(0);
            }
            else
            {
                item.NormalizedTimeStamp = msg.TimeStamp - HistoryItems[0].TimeStamp;
            }
            lock (HistoryItemLock)
            {
                HistoryItems.Add(item);
            }            
        }

        

        


    }
}
