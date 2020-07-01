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

        public delegate void OnFailureEvent(string resourceKey);
        public OnFailureEvent OnFailureEventHandler;


        

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
                sniffer.IsInitiator = value;
                NotifyPropertyChanged(nameof(IsInitiator));
            }
        }



        public FixDelayHandler()
        {
            sniffer = new FixSniffer();
            sniffer.onFailure += OnFailure;
        }

        private void OnFailure(string resourceKey)
        {
            OnFailureEventHandler.Invoke(resourceKey);
        }
        

        public void SetPorts(List<ushort> ports)
        {
            sniffer.SetPorts(ports);
        }

        public void SetDevice(LivePacketDevice device)
        {
            sniffer.SetDevice(device);
        }

        private static string BASE_PATH { get; } = MarketTesterUtil.APPLICATION_SAVE_DIR + MarketTesterUtil.FILE_PATH_DELIMITER + "sniffer" +
            MarketTesterUtil.FILE_PATH_DELIMITER;
        private static string MESSAGE_LOG_FILE_PATH { get; } = BASE_PATH + "message.log";

        private static string PACKET_LOG_FILE_PATH { get; } = BASE_PATH + "packet.log";
        public void Start()
        {
            if (IsRemoteRunning)
            {
                throw new ParallelRunException("Remote sniffer is already running while trying to start local sniffer");
            }
            sniffer.Start();
            new Thread(() =>
            {
                if (IsRunning)
                {
                    return;
                }
                IsRunning = true;
                while (IsRunning && sniffer.IsRunning)
                {
                    if(sniffer.MessageQueue.TryDequeue(out (byte[],DateTime) messageTuple))
                    {
                        string packet = DefaultEncoding.GetString(messageTuple.Item1);
                        messageStringBuffer += packet;
                        string timestampString = messageTuple.Item2.ToString(Util.DateFormatMicrosecondPrecision);
                        Util.AppendStringToFile(PACKET_LOG_FILE_PATH, timestampString + " : " + packet);
                        int messageStartIndex, messageLength;
                        
                        while(true)
                        {
                            (messageStartIndex, messageLength) = Fix.ExtractFixMessageIndexFromBuffer(messageStringBuffer);
                            if (messageStartIndex == -1 || messageLength == 0)
                            {
                                break;
                            }
                            string message = messageStringBuffer.Substring(messageStartIndex, messageLength);
                            Util.AppendStringToFile(MESSAGE_LOG_FILE_PATH,
                                timestampString + " : " + message);
                            messageStringBuffer = messageStringBuffer.Substring(messageStartIndex + messageLength,
                                messageStringBuffer.Length - (messageStartIndex + messageLength));
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
            
        }

        public void Stop()
        {
            IsRunning = false;
            IsRemoteRunning = false;
            sniffer.Stop();
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

        private class FixSniffer : IDisposable
        {
            private PacketDevice Device { get; set; }
            private PacketCommunicator Communicator { get; set; }
            private HashSet<ushort> Ports { get; set; } = new HashSet<ushort>();
            private List<IpV4Address> LocalHostIpAddresses { get; set; } = new List<IpV4Address>();
            public bool IsRunning { get; set; }
            public ConcurrentQueue<(byte[], DateTime)> MessageQueue { get; set; } = new ConcurrentQueue<(byte[], DateTime)>();
            public bool IsInitiator { get; set; }
            

            public delegate void OnFailure(string message);
            public OnFailure onFailure;
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


            public void SetPorts(List<ushort> ports)
            {
                Ports.Clear();
                foreach (ushort port in ports)
                {
                    Ports.Add(port);
                }
            }

            public void SetDevice(LivePacketDevice device)
            {
                if (device != null)
                {
                    Device = device;
                    Communicator = Device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
                    //using (BerkeleyPacketFilter filter = Communicator.CreateFilter("ip and tcp"))
                    //{
                    //    // Set the filter
                    //    Communicator.SetFilter(filter);

                    //}
                }

            }
            /// <summary>
            /// method for starting packet capturing for given ports on the local machine. Dont forget to call Stop() method when you are done with sniffer.
            /// </summary>
            public void Start()
            {
                if (IsRunning)
                {
                    return;
                }
                else
                {
                    IsRunning = true;
                    if (Device != null)
                    {
                        new Thread(() =>
                        {
                            try
                            {
                                while (IsRunning)
                                {
                                    Communicator.ReceivePacket(out Packet packet);
                                    if (packet != null)
                                        PacketHandler(packet);
                                }
                                //If code reaches here Stop function has been called
                            }
                            catch(Exception)
                            {
                                IsRunning = false;
                                onFailure.Invoke(ResourceKeys.StringInfoSnifferStopped);
                            }

                        }).Start();
                    }
                }
            }

            public void Stop()
            {
                IsRunning = false;
            }

            private bool IsSynchronized { get; set; } = false;
            private uint NextExpectedSeqNum {get;set;}
            //buffer for incoming out of order packets
            private List<Packet> PendingBuffer { get; set; } = new List<Packet>();
            private static string BASE_PATH { get; } = MarketTesterUtil.APPLICATION_SAVE_DIR + MarketTesterUtil.FILE_PATH_DELIMITER + "sniffer" +
            MarketTesterUtil.FILE_PATH_DELIMITER;
            private static string PACKET_DETAILED_LOG_FILE_PATH { get; } = BASE_PATH + "packet_detailed.log";

            private void PacketHandler(Packet packet)
            {                
                IpV4Datagram ip = packet.Ethernet.IpV4;                
                TcpDatagram tcp = ip.Tcp;
                if(tcp == null)
                {
                    return;
                }
                void HandlePacketOutgoing()
                {
                    if(tcp.PayloadLength > 0)
                    {
                        byte[] tcpData = tcp.ToArray().SubArray(tcp.HeaderLength, tcp.Length - tcp.HeaderLength);
                        MessageQueue.Enqueue((tcpData, packet.Timestamp));
                    }
                    
                }
                void HandlePendingBuffer()
                {
                    //reverse sort in order to reduce remove cost
                    PendingBuffer.Sort((o1, o2) => o2.TcpSeqCompare(o1));
                    while(PendingBuffer.Count > 0)
                    {
                        Packet pendingPacket = PendingBuffer[PendingBuffer.Count - 1];
                        IpV4Datagram ipBuffer = pendingPacket.Ethernet.IpV4;
                        TcpDatagram tcpBuffer = ip.Tcp;
                        if (tcpBuffer.SequenceNumber == NextExpectedSeqNum)
                        {
                            if(tcpBuffer.PayloadLength > 0)
                            {
                                byte[] tcpData = tcpBuffer.ToArray().SubArray(tcpBuffer.HeaderLength, tcpBuffer.Length - tcpBuffer.HeaderLength);
                                MessageQueue.Enqueue((tcpData, pendingPacket.Timestamp));
                            }                            
                            PendingBuffer.RemoveAt(PendingBuffer.Count - 1);
                            NextExpectedSeqNum = (uint)tcpBuffer.SequenceNumber + (uint)tcpBuffer.PayloadLength;
                        }
                        else if(tcpBuffer.SequenceNumber < NextExpectedSeqNum)
                        {
                            PendingBuffer.RemoveAt(PendingBuffer.Count - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                void HandlePacketIncoming()
                {
                    if (IsSynchronized && tcp.SequenceNumber != NextExpectedSeqNum)
                    {
                        if(tcp.SequenceNumber > NextExpectedSeqNum)
                        {
                            
                            PendingBuffer.Add(packet);
                        }                        
                        return;                        
                    }
                    if (tcp.IsSynchronize)
                    {
                        NextExpectedSeqNum = (uint)tcp.SequenceNumber + 1;
                        IsSynchronized = true;
                    }
                    else
                    {
                        NextExpectedSeqNum = (uint)tcp.SequenceNumber + (uint)tcp.PayloadLength;
                        IsSynchronized = true;
                    }
                    if (tcp.IsReset)
                    {
                        IsSynchronized = false;
                    }
                    
                    
                    if (tcp.PayloadLength > 0)
                    {                        
                        byte[] tcpData = tcp.ToArray().SubArray(tcp.HeaderLength, tcp.Length - tcp.HeaderLength);
                        MessageQueue.Enqueue((tcpData, packet.Timestamp));                        
                    }
                    HandlePendingBuffer();
                }
                
                if (IsInitiator)
                {
                    if (LocalHostIpAddresses.Contains(ip.Source))
                    {

                        if (Ports.Contains(tcp.DestinationPort))
                        {
                            HandlePacketOutgoing();
                        }
                    }
                    else
                    {
                        if (Ports.Contains(tcp.SourcePort))
                        {
                            HandlePacketIncoming();
                        }
                    }
                }
                else
                {
                    if (LocalHostIpAddresses.Contains(ip.Source))
                    {

                        if (Ports.Contains(tcp.SourcePort))
                        {
                            HandlePacketOutgoing();
                        }
                    }
                    else
                    {
                        if (Ports.Contains(tcp.DestinationPort))
                        {
                            HandlePacketIncoming();
                        }
                    }
                }
                
                
                

            }
            public void Dispose()
            {
                Stop();
            }
        }
    }
}
