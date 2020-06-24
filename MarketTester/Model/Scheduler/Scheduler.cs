using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using System.Threading;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Model;
using MarketTester.Helper;
using BackOfficeEngine;
using BackOfficeEngine.ParamPacker;
using System.CodeDom.Compiler;
using MarketTester.Connection;
using BackOfficeEngine.GeneralBase;
using MarketTester.Model.FixFreeFormat;
using System.Reflection;
using BackOfficeEngine.Helper;
using MarketTester.Exceptions;
using QuickFix;
using BackOfficeEngine.Exceptions;
using FixLogAnalyzer;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace MarketTester.Model.Scheduler
{
    public class Scheduler : BaseNotifier
    {
        private const string MapMessages = "map_messages";
        private const string MapNonProtocolID = "map_non_protocol_id";
        private static string ScheduleGroupsSavePath { get; set; } = MarketTesterUtil.APPLICATION_SAVE_DIR + "schedule_group_orders.json";
        public static void ClearMaps()
        {
            ScheduleGroupedMapMessages.Clear();
            ScheduleGroupedMapNonProtocolIDs.Clear();
            SaveScheduleGroups();
        }
        public static void LoadScheduleGroups()
        {
            try
            {
                if (!File.Exists(ScheduleGroupsSavePath))
                {
                    return;
                }
                string json = MarketTesterUtil.ReadFile(ScheduleGroupsSavePath);
                JObject topLevelObject = (JObject)JsonConvert.DeserializeObject(json);
                JObject mapMessages = (JObject)topLevelObject[MapMessages];
                foreach (var item in mapMessages)
                {
                    List<IMessage> list = new List<IMessage>();
                    ScheduleGroupedMapMessages[item.Key] = list;
                    JArray array = (JArray)item.Value;
                    foreach (string s in array)
                    {
                        IMessage msg = Util.ParseLineRepr(s);
                        list.Add(msg);
                    }
                }
                JObject mapNonProtocolID = (JObject)topLevelObject[MapNonProtocolID];
                foreach (var item in mapNonProtocolID)
                {
                    HashSet<string> set = new HashSet<string>();
                    ScheduleGroupedMapNonProtocolIDs[item.Key] = set;
                    JArray array = (JArray)item.Value;
                    foreach (string s in array)
                    {
                        set.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogDebugError(ex, "Couldn't parse scheduler maps");
            }

            Util.ThreadStart(() =>
            {
                while (true)
                {
                    SaveScheduleGroups();
                    Thread.Sleep(5000);
                }                    
            });
        }
        private static object saveLock { get; set; } = new object();
        private static void SaveScheduleGroups()
        {
            lock (saveLock)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic[MapNonProtocolID] = ScheduleGroupedMapNonProtocolIDs;

                Dictionary<string, List<string>> protocolIDCopy = new Dictionary<string, List<string>>();
                foreach (KeyValuePair<string, List<IMessage>> pair in ScheduleGroupedMapMessages)
                {
                    List<string> heyo = new List<string>();
                    protocolIDCopy[pair.Key] = heyo;
                    foreach (IMessage msg in pair.Value)
                    {
                        heyo.Add(Util.GetLineRepr(msg));
                    }
                }
                dic[MapMessages] = protocolIDCopy;
                string serialized = JsonConvert.SerializeObject(dic);
                MarketTesterUtil.OverwriteToFile(ScheduleGroupsSavePath, serialized);
            }            
        }
        public static Dictionary<string, List<IMessage>> ScheduleGroupedMapMessages { get; set; } = new Dictionary<string, List<IMessage>>();
        public static Dictionary<string, HashSet<string>> ScheduleGroupedMapNonProtocolIDs { get; set; } = new Dictionary<string, HashSet<string>>();
        //do not modify the following collection from outside of this class. it is made public in order for use with bindings on xaml.
        public ObservableCollection<SchedulerRawItem> scheduleRaw { get; set; } = new ObservableCollection<SchedulerRawItem>();
        //(message,connector name,delay)
        private List<(Message,string,int)> schedulePrepared = new List<(Message, string, int)>();
        //map between scheduler order id and backoffice order id
        private Dictionary<string, string> OrderIdMap { get; set; }
        private int schedulerOrderId = 1;

        public bool PreEvaluateReplaceQuantities { get; set; }
        public int[] selectedIndices = new int[0];

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }


        public Scheduler(string name)
        {
            OrderIdMap = new Dictionary<string, string>();
            this.Name = name;
            Engine.GetInstance().OnMessageEvent += Scheduler_OnMessageEvent;
        }
        //ClOrdID of the first order
        private string MainNonProtocolOrderId{ get; set; }
        private HashSet<string> CurrentScheduleClOrdIDs { get; set; } = new HashSet<string>();
        /// <summary>
        /// in order to consolidate schedule messages these event is triggered by Engine
        /// </summary>
        private void Scheduler_OnMessageEvent(object sender, BackOfficeEngine.Events.OnMessageEventArgs args)
        {
            string clOrdID = Fix.GetTag(args.msg, Tags.ClOrdID);
            if (CurrentScheduleClOrdIDs.Contains(clOrdID))
            {
                if(!ScheduleGroupedMapMessages.TryGetValue(MainNonProtocolOrderId,out List<IMessage> msgs))
                {
                    msgs = new List<IMessage>();
                    ScheduleGroupedMapMessages[MainNonProtocolOrderId] = msgs;
                }
                IMessage msg = new QuickFixMessage(args.msg);
                msg.TimeStamp = args.timeStamp;
                msgs.Add(msg);
            }
        }

        public SchedulerRawItem PrepareScheduleItem(
            MsgType msgType,
            string account,
            BackOfficeEngine.MessageEnums.Side side,
            OrdType orderType,
            TimeInForce timeInForce,
            string orderQty,
            string symbol,
            DateTime expireDate,
            string price,
            string allocId,
            string schedulerOrderId,
            string connectorName,
            string delay)
        {
            
            SchedulerRawItem item = new SchedulerRawItem()
            {
                MsgType = msgType,
                Account = account,
                Side = side,
                OrdType = orderType,
                TimeInForce = timeInForce,
                Symbol = symbol,                
                AllocID = allocId,
                SchedulerOrderID = !string.IsNullOrWhiteSpace(schedulerOrderId) ? schedulerOrderId : (this.schedulerOrderId++).ToString(),
                ConnectorName = connectorName,
                Delay = int.Parse(delay, CultureInfo.InvariantCulture),
                ExpireDate = expireDate
            };
            if (!string.IsNullOrWhiteSpace(orderQty))
                item.OrderQty = Decimal.Parse(orderQty, CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(price))
                item.Price = Decimal.Parse(price, CultureInfo.InvariantCulture);
            return item;
        }

        public void AddItem(SchedulerRawItem item)
        {
            SchedulerRawItem prevItem = GetLastItemBySchedulerOrderID(item.SchedulerOrderID);
            if (prevItem != null)
            {
                if (item.MsgType == MsgType.Cancel)
                {
                    item.OrderQty = prevItem.OrderQty;
                    item.Price = prevItem.Price;
                    item.Side = prevItem.Side;
                }
                else if (item.MsgType == MsgType.Replace)
                {
                    item.Side = prevItem.Side;
                }
            }
            scheduleRaw.Add(item);
        }

        public SchedulerRawItem GetLastItemBySchedulerOrderID(string schedulerOrderID)
        {
            return scheduleRaw.LastOrDefault((o) => o.SchedulerOrderID == schedulerOrderID);
        }

        public SchedulerRawItem GetItem(int index)
        {
            return scheduleRaw[index];
        }

        public void DeleteItem(int index)
        {
            scheduleRaw.RemoveAt(index);
        }

        

        public bool SwapItem(int index1, int index2)
        {
            if (index1 < scheduleRaw.Count ||
               index1 >= 0 ||
               index2 < scheduleRaw.Count ||
               index2 >= 0)
            {
                SchedulerRawItem upItem;
                SchedulerRawItem downItem;
                if (index1 == index2)
                {
                    return false;
                }
                if(index1 > index2)
                {
                    upItem = scheduleRaw[index2];
                    downItem = scheduleRaw[index1];
                }
                else
                {
                    upItem = scheduleRaw[index1];
                    downItem = scheduleRaw[index2];
                }
                if(upItem.SchedulerOrderID == downItem.SchedulerOrderID)
                {
                    switch (upItem.MsgType)
                    {
                        case MsgType.New:
                            if(downItem.MsgType == MsgType.Replace || downItem.MsgType == MsgType.Cancel)
                            {
                                return false;
                            }
                            break;
                        case MsgType.Replace:
                            if(downItem.MsgType == MsgType.Cancel)
                            {
                                return false;
                            }
                            break;
                    }
                }
                SchedulerRawItem item1 = scheduleRaw[index1];
                scheduleRaw[index1] = scheduleRaw[index2];
                scheduleRaw[index2] = item1;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void StartSchedule()
        {
            Engine engine = Engine.GetInstance();
            foreach ((Message,string,int) item in schedulePrepared)
            {
                Message m; string connectorName;int delay;
                (m, connectorName, delay) = item;
                Thread.Sleep(delay);
                engine.SendMessage(m,connectorName);
            }
            selectedIndices = new int[0];
        }
        /// <summary>
        /// Prepares the schedule and returns the first New Message's associated order's NonProtocolId(basically returns the id of the order)
        /// </summary>
        /// <param name="priceOffset"></param>
        /// <param name="quantityMultiplier"></param>
        /// <param name="extraTagValuePairs"></param>
        /// <returns></returns>
        public string PrepareSchedule(decimal priceOffset, decimal quantityMultiplier,List<TagValuePair> extraTagValuePairs,Channel defaultChannel = null)
        {
            //note that replace requests and cancel requests have to be based on ClOrdId and OrigClOrdId in case of preprocessing
            //and schedule is always preproccesed
            schedulePrepared.Clear();

            
            
            string firstNewMessageNonProtocolId = null;
            Channel channel = defaultChannel;
            Matcher matcher = new Matcher();
            Engine engine = Engine.GetInstance();
            CurrentScheduleClOrdIDs.Clear();
            foreach (SchedulerRawItem item in scheduleRaw)
            {
                if (!item.IsSelected)
                {
                    continue;
                }
                if (item.MsgType != MsgType.New && !OrderIdMap.ContainsKey(item.SchedulerOrderID))
                {
                    continue;
                }
                item.Price = item.Price + priceOffset;
                item.OrderQty = item.OrderQty * quantityMultiplier;
                IMessage m;string nonProtocolOrderId;
                if (channel == null)
                    channel = Connector.ActiveChannels.FirstOrDefault((o) => o.Name == item.ConnectorName);
                if(channel == null)
                {
                    throw new ConnectorNotPresentException();
                }
                if (item.MsgType == MsgType.New)
                {
                    if(item.Price != -1)
                    {
                        (m,nonProtocolOrderId) = engine.PrepareMessageNew(new NewMessageParameters(channel.ProtocolType, item.Account, item.Symbol,
                                                                       item.OrderQty, item.Side, item.TimeInForce, item.OrdType, item.Price),channel.Name);
                    }                    
                    else
                    {
                        (m,nonProtocolOrderId) = engine.PrepareMessageNew(new NewMessageParameters(channel.ProtocolType, item.Account, item.Symbol,
                                                                       item.OrderQty, item.Side, item.TimeInForce, item.OrdType),channel.Name);
                    }
                    if(firstNewMessageNonProtocolId == null)
                    {
                        firstNewMessageNonProtocolId = nonProtocolOrderId;
                        MainNonProtocolOrderId = firstNewMessageNonProtocolId;
                        ScheduleGroupedMapNonProtocolIDs[MainNonProtocolOrderId] = new HashSet<string>();
                    }
                    if(item.ExpireDate != DateTime.MinValue)
                    {
                        m.SetGenericField(432, item.ExpireDate.ToString(MarketTesterUtil.LocalMktDateFormat));
                    }
                    OrderIdMap[item.SchedulerOrderID] = nonProtocolOrderId;
                }
                else if (item.MsgType == MsgType.Replace)
                {
                    if(item.Price != -1 && item.OrderQty != -1)
                    {
                        m = engine.PrepareMessageReplace(new ReplaceMessageParameters(OrderIdMap[item.SchedulerOrderID], item.OrderQty, item.Price));
                    }
                    else if(item.Price != -1)
                    {
                        m = engine.PrepareMessageReplace(new ReplaceMessageParameters(item.Price, OrderIdMap[item.SchedulerOrderID]));
                    }
                    else
                    {
                        m = engine.PrepareMessageReplace(new ReplaceMessageParameters(OrderIdMap[item.SchedulerOrderID],item.OrderQty));
                    }
                    if (PreEvaluateReplaceQuantities)
                    {
                        m.SetOrderQty(item.OrderQty + matcher.GetMatchedQuantity(item));
                    }
                }
                else if (item.MsgType == MsgType.Cancel)
                {
                    m = engine.PrepareMessageCancel(new CancelMessageParameters(OrderIdMap[item.SchedulerOrderID]));
                }
                else
                {
                    throw new Exception("Unsupported message type in Scheduler");
                }
                if (PreEvaluateReplaceQuantities)
                {
                    matcher.AddMessage(item);
                }                
                foreach(TagValuePair pair in extraTagValuePairs)
                {
                    m.SetGenericField(int.Parse(pair.Tag, CultureInfo.InvariantCulture), pair.Value);
                }
                Message msgString = engine.PrepareMessage(m);
                CurrentScheduleClOrdIDs.Add(m.GetClOrdID());
                //add non protocol order ids to current ones exclude main order
                if (OrderIdMap[item.SchedulerOrderID] != MainNonProtocolOrderId)
                    ScheduleGroupedMapNonProtocolIDs[MainNonProtocolOrderId].Add(OrderIdMap[item.SchedulerOrderID]);
                schedulePrepared.Add((msgString, channel.Name,item.Delay));
                item.Price = item.Price - priceOffset;
                item.OrderQty = item.OrderQty / quantityMultiplier;
            }
            return firstNewMessageNonProtocolId;
        }


        

        public string SaveSchedule()
        {
            string s = name + Environment.NewLine;
            foreach (SchedulerRawItem item in scheduleRaw)
            {
                s += item.ToString() + Environment.NewLine;
            }
            s = s.Substring(0, s.Length - Environment.NewLine.Length);
            return s;

        }



        public bool LoadSchedule(string[] lines)
        {
            try
            {
                int maxId = 0;
                scheduleRaw.Clear();
                name = lines[0];
                for (int i = 1; i < lines.Length; i++)
                {
                    SchedulerRawItem item = new SchedulerRawItem(lines[i]);
                    scheduleRaw.Add(item);
                    int currentId = int.Parse(item.SchedulerOrderID, CultureInfo.InvariantCulture);
                    if (currentId > maxId)
                    {
                        maxId = currentId;
                    }
                }
                schedulerOrderId = maxId + 1;
                return true;
            }
            catch (Exception ex)
            {
                Util.LogError(ex);
                return false;
            }

        }

        public void ClearSchedule()
        {
            scheduleRaw.Clear();
            schedulePrepared.Clear();
        }


    }
}
