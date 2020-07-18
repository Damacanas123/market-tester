using BackOfficeEngine.Helper;
using MarketTester.Helper;
using PcapDotNet.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuickFix.Fields;
using FixHelper;
using MarketTester.Exceptions;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Transport;
using MarketTester.Extensions;
using BackOfficeEngine.Model;
using MarketTester.Base;
using System.Globalization;
using System.Net.Sockets;
using QuickFix;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;
using Microsoft.Office.Interop.Excel;
using PcapDotNet.Core.Extensions;
using System.Windows.Media;
using System.IO;

namespace MarketTester.Model.Sniffer
{
    public class FixDelayHandler :BaseNotifier
    {
        private static string Tag8 = "8=";
        private static string Tag10 = "\u000110=";
        public static Encoding DefaultEncoding { get; private set; } = Encoding.GetEncoding("iso-8859-1");
        private string messageStringBuffer { get; set; }
        private bool IsRunning { get; set; }
        private bool TagFound8 { get; set; }
        private bool TagFound10 { get; set; }
        private FixSniffer sniffer { get; set; }
        private ConcurrentQueue<(string, DateTime)> MessageQueue { get; set; } = new ConcurrentQueue<(string, DateTime)>();

        public delegate void OnFailureEvent(string resourceKey);
        public event OnFailureEvent OnFailureEventHandler;

        private static string GetShortDeviceName(PacketDevice device)
        {
            string name = (device.Name.Length > 10 ? device.Name.Substring(0, 10) : device.Name);  
            name += (device.Description.Length > 10 ? device.Description.Substring(0, 10) : device.Description);
            name = Util.RemoveFileNameInvalidChars(name);
            return name;
        }
        

        private string textAverageDelay;
        public string TextAverageDelay
        {
            get { return textAverageDelay; }
            set
            {
                textAverageDelay = value;
                NotifyPropertyChanged(nameof(TextAverageDelay));
            }
        }

        private string textTotalRequests;

        public string TextTotalRequest
        {
            get { return textTotalRequests; }
            set
            {
                textTotalRequests = value;
                NotifyPropertyChanged(nameof(TextTotalRequest));
            }
        }

        private string textTotalAcknowledgements;

        public string TextTotalAcknowledgements
        {
            get { return textTotalAcknowledgements; }
            set
            {
                textTotalAcknowledgements = value;
                NotifyPropertyChanged(nameof(TextTotalAcknowledgements));
            }
        }


        private int TotalPairs { get; set; }
        private decimal averageDelay;
        private decimal AverageDelay
        {
            get
            {
                return averageDelay;
            }
            set
            {
                averageDelay = value;
                TextAverageDelay = ((int)averageDelay/1000m).ToString(CultureInfo.InvariantCulture);
            }
        }

        private int totalRequests;

        public int TotalRequests
        {
            get { return totalRequests; }
            set
            {
                totalRequests = value;
                TextTotalRequest = totalRequests.ToString(CultureInfo.InvariantCulture);
            }
        }

        private int totalAcknowledgements;

        public int TotalAcknowledgements
        {
            get { return totalAcknowledgements; }
            set
            {
                totalAcknowledgements = value;
                TextTotalAcknowledgements = totalAcknowledgements.ToString(CultureInfo.InvariantCulture);
            }
        }

        private bool isInitiator;

        public bool IsInitiator
        {
            get { return isInitiator; }
            set
            {
                isInitiator = value;
                NotifyPropertyChanged(nameof(IsInitiator));
            }
        }

        private PacketDevice UsedDevice { get; set; }



        public FixDelayHandler()
        {
            
        }

        private void OnFailure(string resourceKey)
        {
            OnFailureEventHandler?.Invoke(resourceKey);
            Stop();
        }

        
        

        private (bool,string) Subscribe(PacketDevice device,List<ushort> ports)
        {
            Console.WriteLine("Inside Handler subscribe");
            if (UsedDevice == null)
            {
                FixSniffer sniffer = FixSniffer.GetInstance(device);
                this.sniffer = sniffer;
                (bool result,string message) = sniffer.Subscribe(this, ports, IsInitiator);
                if (!result)
                {
                    return (result, message);
                }
                sniffer.onFailure += OnFailure;
                UsedDevice = device;
                MESSAGE_LOG_FILE_PATH = BASE_PATH + $"{GetShortDeviceName(device)}_message.log";
                PACKET_LOG_FILE_PATH = BASE_PATH + $"{GetShortDeviceName(device)}_packet_ordered.log";
                return (true, "");
            }
            else
            {
                Util.Debug("UsedDevice is not null when initiating sniffer. This is a bug that should not happen");
                return (false, App.Current.Resources[ResourceKeys.StringUnknownErrorOccured].ToString());
            }
        }

        private void Unsubscribe()
        {
            FixSniffer.GetInstance(UsedDevice).Unsubscribe(this);
            sniffer.onFailure -= OnFailure;
        }

        private string BASE_PATH { get; } = MarketTesterUtil.APPLICATION_SAVE_DIR + MarketTesterUtil.FILE_PATH_DELIMITER + "sniffer" +
            MarketTesterUtil.FILE_PATH_DELIMITER;
        private string MESSAGE_LOG_FILE_PATH { get; set; }

        private string PACKET_LOG_FILE_PATH { get; set; }
        public (bool,string) Start(PacketDevice device, List<ushort> ports)
        {
            Console.WriteLine("Inside Handler start");
            (bool result,string errorMessage) = Subscribe(device, ports);
            if (!result)
            {
                return (result, errorMessage);
            }
            if (UsedDevice == null)
            {
                return (false,App.Current.Resources[ResourceKeys.StringUnknownErrorOccured].ToString());
            }
            if (IsRunning)
            {
                return (false, App.Current.Resources[ResourceKeys.StringSnifferAlreadyRunning].ToString());
            }
            
            if (IsRemoteRunning)
            {
                throw new ParallelRunException("Remote sniffer is already running while trying to start local sniffer");
            }
            IsRunning = true;
            new Thread(() =>
            {
                while (IsRunning)
                {
                    if(MessageQueue.TryDequeue(out (string,DateTime) messageTuple))
                    {
                        string packet = messageTuple.Item1;
                        messageStringBuffer += packet;
                        string timestampString = messageTuple.Item2.ToString(Util.DateFormatMicrosecondPrecision);
                        Util.AppendStringToFile(PACKET_LOG_FILE_PATH,$"{DateTime.Now.ToString(Util.DateFormatMicrosecondPrecision)}({timestampString})" +
                             " : " + packet);
                        int messageStartIndex, messageLength;
                        
                        while(true)
                        {
                            (messageStartIndex, messageLength) = Fix.ExtractFixMessageIndexFromBuffer(messageStringBuffer);
                            
                            if (messageStartIndex == -1 || messageLength == 0)
                            {
                                break;
                            }
                            string message = messageStringBuffer.Substring(messageStartIndex, messageLength);
                            Util.AppendStringToFile(MESSAGE_LOG_FILE_PATH, $"{DateTime.Now.ToString(Util.DateFormatMicrosecondPrecision)}({timestampString})" +
                            " : " + message);
                            int nextMessageStart = messageStartIndex + messageLength;
                            if (nextMessageStart == messageStringBuffer.Length)
                            {
                                messageStringBuffer = "";
                            }
                            else
                            {
                                int length = messageStringBuffer.Length - nextMessageStart;
                                messageStringBuffer = messageStringBuffer.Substring(nextMessageStart,length);
                                
                            }

                            string msgType = Fix.GetTag(message, Tags.MsgType);
                            if (FixValues.MsgTypesOrderEntry.ContainsKey(msgType))
                            {
                                AddItem(message, messageTuple.Item2);
                                Util.AppendStringToFile(MESSAGE_LOG_FILE_PATH,"Added preceding message to list");
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();
            return (true, "");
        }

        public void Stop()
        {
            if (IsRunning)
            {
                Unsubscribe();
            }   
            IsRunning = false;
            IsRemoteRunning = false;
            
            UsedDevice = null;
            sniffer = null;
            
        }


        public ObservableCollectionEx<DiffItem> DiffItems { get; set; } = new ObservableCollectionEx<DiffItem>();
        private Dictionary<string, DiffItem> ClOrdIdMap { get; set; } = new Dictionary<string, DiffItem>();

        public void AddItem(string message, DateTime timeStamp)
        {
            string msgType = Fix.GetTag(message, Tags.MsgType);
            string clOrdID = BackOfficeEngine.Helper.Fix.GetTag(message, Tags.ClOrdID);
            if (ClOrdIdMap.TryGetValue(clOrdID, out DiffItem item))
            {

            }
            else
            {
                if (FixValues.MsgTypesOrderEntryOutbound.ContainsKey(msgType))
                {
                    item = new DiffItem();
                    item.RowIndex = DiffItems.Count + 1;
                    ClOrdIdMap[clOrdID] = item;
                    App.Invoke(() =>
                    {
                        DiffItems.Add(item);
                    });
                }
                else
                {
                    return;
                }
                
            }
            //means that a new acknowledgement arrived
            //discard the previous acknowledgement
            if (item.Request != null && item.Response != null)
            {
                if(TotalPairs == 1)
                {
                    AverageDelay = 0;
                    TotalPairs = 0;
                }
                else
                {
                    AverageDelay = (AverageDelay * TotalPairs - item.Delay.GetTotalMicroSeconds()) / (--TotalPairs);
                }
                
            }
            if (FixHelper.FixValues.MsgTypesOrderEntryOutbound.ContainsKey(msgType))
            {
                item.Request = message;
                item.RequestTime = timeStamp;
                TotalRequests += 1;
            }
            else if(FixValues.MsgTypesOrderEntryInbound.ContainsKey(msgType))
            {
                if(msgType == MsgType.EXECUTIONREPORT)
                {
                    string execType = Fix.GetTag(message, Tags.ExecType.ToString());
                    if (FixValues.IncomingExecTypesOrderEntry.ContainsKey(execType))
                    {
                        item.Response = message;
                        item.ResponseTime = timeStamp;
                        TotalAcknowledgements += 1;
                    }
                }
                else
                {
                    item.Response = message;
                    item.ResponseTime = timeStamp;
                    TotalAcknowledgements += 1;
                }
                
            }
            
            if(item.Request != null && item.Response != null)
            {
                AverageDelay = (AverageDelay * TotalPairs + item.Delay.GetTotalMicroSeconds()) / (++TotalPairs);
            }
            
        }

        private IPAddress RemoteIPAddress { get; set; }
        private ushort RemotePort { get; set; }
        private IPEndPoint RemoteEndPoint { get; set; }
        private bool IsRemoteRunning { get; set; }

        private const string REQUEST_RESPONSE = "Req-Res";
        private const string REQUEST = "Req";
        private const string RESPONSE = "Res";
        private const string HEARTBEAT = "Hrtbt";



        public void SetParameters(IPAddress ipAddress, ushort port)
        {
            RemoteIPAddress = ipAddress;
            RemotePort = port;
            RemoteEndPoint = new IPEndPoint(RemoteIPAddress, RemotePort);
        }

        public delegate void OnUnMatureSocketClose();
        
        /// <summary>
        /// Tries to start a connection and listen for incoming diff item from remote sniffer. If an error occurs at initilization returns false and a ResourceKey
        /// indicating what the error was.
        /// </summary>
        /// <returns></returns>
        public (bool, string) StartRemote(OnUnMatureSocketClose onSocketClose)
        {
            if (IsRunning)
            {
                throw new ParallelRunException("Local sniffer is already running while trying to start remote sniffer");
            }
            Socket sender;
            try
            {
                sender = new Socket(RemoteIPAddress.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(RemoteEndPoint);
            }
            catch (SocketException ex)
            {
                Util.LogDebugError(ex);
                return (false, ResourceKeys.StringCantConnectToRemoteServer);
            }
            catch (Exception ex)
            {
                Util.LogError(ex);
                return (false, ResourceKeys.StringUnknownErrorOccured);
            }
            IsRemoteRunning = true;
            sender.Send(REQUEST_RESPONSE);

            Util.ThreadStart(() =>
            {
                while (IsRemoteRunning)
                {
                    string s = null;
                    try
                    {
                        s = sender.ReadMessage();
                    }
                    catch(SocketException ex)
                    {
                        IsRemoteRunning = false;
                        onSocketClose();
                    }
                    if(s == null)
                    {
                        continue;
                    }
                    if(s == HEARTBEAT)
                    {
                        try
                        {

                            sender.Send(HEARTBEAT);
                            continue;
                        }
                        catch (SocketException ex)
                        {
                            IsRemoteRunning = false;
                            onSocketClose();
                        }
                    }
                    string[] values = s.Split('|');
                    DateTime timeStamp = DateTime.ParseExact(values[2], MarketTesterUtil.DateFormatMicrosecondPrecision, CultureInfo.InvariantCulture);
                    AddItem(values[1], timeStamp);
                }
                sender.Close();
            });
            return (true, "");
        }





        public void StopRemote()
        {
            IsRemoteRunning = false;
        }

        private class FixSniffer
        {
            private PacketDevice Device { get; set; }
            private PacketCommunicator Communicator { get; set; }
            private List<IpV4Address> LocalHostIpAddresses { get; set; } = new List<IpV4Address>();
            public bool IsRunning { get; set; }
            private static ConcurrentDictionary<string, FixSniffer> Instances = new ConcurrentDictionary<string, FixSniffer>();
            private HashSet<ushort> ActiveInitiatorPorts = new HashSet<ushort>();
            private HashSet<ushort> ActiveAcceptorPorts = new HashSet<ushort>();

            private class Subscriber
            {
                public HashSet<ushort> Ports { get; set; } = new HashSet<ushort>();
                public bool IsInitiator { get; set; }
                public FixDelayHandler Handler { get; set; }
                public bool IsSynchronized { get; set; } = false;
                public uint NextExpectedSeqNum { get; set; }
                public List<Packet> PendingBuffer = new List<Packet>();
            }
            private List<Subscriber> Subscribers { get; set; } = new List<Subscriber>();

            public (bool,string) Subscribe(FixDelayHandler handler,List<ushort> ports,bool isInitiator)
            {
                Subscriber subscriber = new Subscriber();
                
                string usedPortMessage = App.Current.Resources[ResourceKeys.StringPortIsAlreadyListened].ToString();
                bool duplicatePort = false;
                if (isInitiator)
                {
                    lock (ActiveInitiatorPorts)
                    {
                        foreach (ushort port in ports)
                        {
                            if (ActiveInitiatorPorts.Contains(port))
                            {
                                usedPortMessage += port + ",";
                                duplicatePort = true;
                            }
                            else
                            {
                                if (!duplicatePort)
                                    ActiveInitiatorPorts.Add(port);
                            }
                        }                    
                    }
                }
                else
                {
                    lock (ActiveAcceptorPorts)
                    {
                        foreach(ushort port in ports) 
                        {
                            if (ActiveAcceptorPorts.Contains(port))
                            {
                                usedPortMessage += port + ",";
                                duplicatePort = true;
                            }
                            else
                            {
                                if(!duplicatePort)
                                    ActiveAcceptorPorts.Add(port);
                            }
                        }
                    }
                }
                if (duplicatePort)
                {
                    return (false, usedPortMessage.Substring(0, usedPortMessage.Length - 1));
                }
                foreach (ushort port in ports)
                {
                    subscriber.Ports.Add(port);
                }
                subscriber.IsInitiator = isInitiator;
                subscriber.Handler = handler;
                lock (Subscribers)
                {
                    
                    Subscribers.Add(subscriber);
                    Console.WriteLine("Handler subscribed");
                    if (Subscribers.Count == 1)
                    {
                        Start();
                        Console.WriteLine("Started sniffer");
                    }
                }
                return (true, "");
            }

            public void Unsubscribe(FixDelayHandler handler)
            {
                if(handler != null)
                {
                    lock (Subscribers)
                    {
                        int index = Subscribers.FindIndex((o) => o.Handler == handler);
                        if (index >= 0 && index < Subscribers.Count)
                        {
                            Subscriber subscriber = Subscribers[index];
                            if (subscriber.IsInitiator)
                            {
                                foreach(ushort port in subscriber.Ports)
                                {
                                    ActiveInitiatorPorts.Remove(port);
                                    Console.WriteLine("removed port : " + port);
                                }                                
                            }
                            else
                            {
                                foreach (ushort port in subscriber.Ports)
                                {
                                    ActiveAcceptorPorts.Remove(port);
                                    Console.WriteLine("removed port : " + port);
                                }
                            }
                            Subscribers.RemoveAt(index);
                            Console.WriteLine("Handler unsubscribed");
                            if (Subscribers.Count == 0)
                            {
                                Stop();
                                Console.WriteLine("Stopped sniffer");
                            }
                        }                        
                    }
                }
            }

            public static FixSniffer GetInstance(PacketDevice device)
            {
                if(Instances.TryGetValue(device.Name,out FixSniffer sniffer))
                {
                    
                }
                else
                {
                    sniffer = new FixSniffer();
                    sniffer.Device = device;
                    
                    sniffer.PACKET_DETAILED_LOG_FILE_PATH = BASE_PATH + $"{FixDelayHandler.GetShortDeviceName(sniffer.Device)}_packet_unordered.log";
                    Instances[device.Name] = sniffer;
                }
                return sniffer;
            }
            

            public delegate void OnFailure(string message);
            public event OnFailure onFailure;
            public FixSniffer()
            {
                foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()).ToList())
                {
                    if (IpV4Address.TryParse(address.ToString(), out IpV4Address ip4address))
                    {
                        LocalHostIpAddresses.Add(ip4address);
                    }
                }
                
            }

            /// <summary>
            /// method for starting packet capturing for given ports on the local machine. This is called automatically when first subscriber subscribes to sniffer
            /// </summary>
            private void Start()
            {
                if (IsRunning)
                {
                    return;
                }
                else
                {
                    
                    if (Device != null)
                    {
                        IsRunning = true;
                        new Thread(() =>
                        {
                            Communicator = Device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
                            //there is a bug in berkeley packet filter when it is activated only outgoing packets are captured.
                            //using (BerkeleyPacketFilter filter = sniffer.Communicator.CreateFilter("ip and tcp"))
                            //{
                            //    // Set the filter
                            //    sniffer.Communicator.SetFilter(filter);
                            //}
                            try
                            {
                                while (IsRunning)
                                {
                                    Communicator.ReceivePacket(out Packet packet);
                                    if (packet != null)
                                        PacketHandler(packet);
                                }
                                Communicator.Dispose();
                                //If code reaches here Stop function has been called
                            }
                            catch(Exception ex)
                            {
                                Util.LogDebugError(ex);
                                Communicator.Dispose();
                                IsRunning = false;
                                lock (Subscribers)
                                {
                                    Subscribers.Clear();
                                    ActiveAcceptorPorts.Clear();
                                    ActiveInitiatorPorts.Clear();
                                }
                                onFailure?.Invoke(ResourceKeys.StringInfoSnifferStopped);
                            }

                        }).Start();
                    }
                }
            }

            private void Stop()
            {
                IsRunning = false;
            }

            
            //buffer for incoming out of order packets
            
            private static string BASE_PATH { get; } = MarketTesterUtil.APPLICATION_SAVE_DIR + MarketTesterUtil.FILE_PATH_DELIMITER + "sniffer" +
            MarketTesterUtil.FILE_PATH_DELIMITER;
            private string PACKET_DETAILED_LOG_FILE_PATH { get; set; }

            

            private void PacketHandler(Packet packet)
            {                
                IpV4Datagram ip = packet.Ethernet.IpV4;                
                TcpDatagram tcp = ip.Tcp;
                if(tcp == null)
                {
                    return;
                }

                void HandlePacketOutgoing(Subscriber subscriber)
                {
                    byte[] tcpData = tcp.ToArray().SubArray(tcp.HeaderLength, tcp.Length - tcp.HeaderLength);
                    string packetS = DefaultEncoding.GetString(tcpData);
                    //log whole packet
                    Util.AppendStringToFile(PACKET_DETAILED_LOG_FILE_PATH,
                        $"{DateTime.Now.ToString(Util.DateFormatMicrosecondPrecision)}({packet.Timestamp.ToString(Util.DateFormatMicrosecondPrecision)})" +
                        $" {ip.Source}:{tcp.SourcePort} -> {ip.Destination}:{tcp.DestinationPort} TCP SEQ({tcp.SequenceNumber})" +
                        $" ACK({tcp.AcknowledgmentNumber}) Data Length({tcp.PayloadLength})"
                        + Environment.NewLine + packetS);
                    
                    if (tcp.PayloadLength > 0)
                    {
                        subscriber.Handler.MessageQueue.Enqueue((packetS, packet.Timestamp));
                    }                    
                }

                void HandlePendingBuffer(Subscriber subscriber)
                {
                    Console.WriteLine($"Function start. Pending buffer length {subscriber.PendingBuffer.Count}");
                    //reverse sort in order to reduce remove cost
                    subscriber.PendingBuffer.Sort((o1, o2) => o2.TcpSeqCompare(o1));
                    while(subscriber.PendingBuffer.Count > 0)
                    {
                        Packet pendingPacket = subscriber.PendingBuffer[subscriber.PendingBuffer.Count - 1];
                        IpV4Datagram ipBuffer = pendingPacket.Ethernet.IpV4;
                        TcpDatagram tcpBuffer = ip.Tcp;
                        if (tcpBuffer.SequenceNumber == subscriber.NextExpectedSeqNum)
                        {
                            if(tcpBuffer.PayloadLength > 0)
                            {
                                byte[] tcpData = tcpBuffer.ToArray().SubArray(tcpBuffer.HeaderLength, tcpBuffer.Length - tcpBuffer.HeaderLength);
                                subscriber.Handler.MessageQueue.Enqueue((DefaultEncoding.GetString(tcpData), pendingPacket.Timestamp));
                            }
                            subscriber.PendingBuffer.RemoveAt(subscriber.PendingBuffer.Count - 1);
                            subscriber.NextExpectedSeqNum = (uint)tcpBuffer.SequenceNumber + (uint)tcpBuffer.PayloadLength;
                        }
                        else if(tcpBuffer.SequenceNumber < subscriber.NextExpectedSeqNum)
                        {
                            subscriber.PendingBuffer.RemoveAt(subscriber.PendingBuffer.Count - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    Console.WriteLine($"Function end. Pending buffer length {subscriber.PendingBuffer.Count}");
                }

                void HandlePacketIncoming(Subscriber subscriber)
                {
                    byte[] tcpData = tcp.ToArray().SubArray(tcp.HeaderLength, tcp.Length - tcp.HeaderLength);
                    string packetS = DefaultEncoding.GetString(tcpData);
                    //log whole packet
                    Util.AppendStringToFile(PACKET_DETAILED_LOG_FILE_PATH,
                        $"{DateTime.Now.ToString(Util.DateFormatMicrosecondPrecision)}({packet.Timestamp.ToString(Util.DateFormatMicrosecondPrecision)})" +
                        $" {ip.Source}:{tcp.SourcePort} -> {ip.Destination}:{tcp.DestinationPort} TCP SEQ({tcp.SequenceNumber})" +
                        $" ACK({tcp.AcknowledgmentNumber}) Data Length({tcp.PayloadLength})"
                        + Environment.NewLine + packetS);
                    if (subscriber.IsSynchronized && tcp.SequenceNumber != subscriber.NextExpectedSeqNum)
                    {
                        if(tcp.SequenceNumber > subscriber.NextExpectedSeqNum)
                        {
                            Console.WriteLine($"Added packet to pending buffer : {packetS}");
                            subscriber.PendingBuffer.Add(packet);
                        }                        
                        return;                        
                    }
                    if (tcp.IsSynchronize)
                    {
                        subscriber.NextExpectedSeqNum = (uint)tcp.SequenceNumber + 1;
                        subscriber.IsSynchronized = true;
                    }
                    else
                    {
                        subscriber.NextExpectedSeqNum = (uint)tcp.SequenceNumber + (uint)tcp.PayloadLength;
                        subscriber.IsSynchronized = true;
                    }
                    if (tcp.IsReset)
                    {
                        subscriber.IsSynchronized = false;
                    }
                    
                    
                    if (tcp.PayloadLength > 0)
                    {
                        subscriber.Handler.MessageQueue.Enqueue((packetS, packet.Timestamp));
                    }
                    HandlePendingBuffer(subscriber);
                }

                foreach (Subscriber subscriber in Subscribers)
                {
                    if (subscriber.IsInitiator)
                    {
                        if (LocalHostIpAddresses.Contains(ip.Source))
                        {
                            
                            if (subscriber.Ports.Contains(tcp.DestinationPort))
                            {
                                HandlePacketOutgoing(subscriber);
                                break;
                            }
                        }
                        else
                        {
                            if (subscriber.Ports.Contains(tcp.SourcePort))
                            {
                                HandlePacketIncoming(subscriber);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (LocalHostIpAddresses.Contains(ip.Source))
                        {

                            if (subscriber.Ports.Contains(tcp.SourcePort))
                            {
                                HandlePacketOutgoing(subscriber);
                                break;
                            }
                        }
                        else
                        {
                            if (subscriber.Ports.Contains(tcp.DestinationPort))
                            {
                                HandlePacketIncoming(subscriber);
                                break;
                            }
                        }
                    }
                }
                
                
                
                

            }
        }
    }
}
