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

namespace MarketTester.Model.Sniffer
{
    public class FixDelayHandler :BaseNotifier
    {
        private static string Tag8 = "8=";
        private static string Tag10 = "\u000110=";
        private string lastMessage { get; set; }
        private bool IsRunning { get; set; }
        private bool TagFound8 { get; set; }
        private bool TagFound10 { get; set; }
        private FixSniffer sniffer { get; set; }
        

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


        public FixDelayHandler()
        {
            sniffer = new FixSniffer();
        }
        

        public void SetPorts(List<ushort> ports)
        {
            sniffer.SetPorts(ports);
        }

        public void SetDevice(LivePacketDevice device)
        {
            sniffer.SetDevice(device);
        }
        
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
                while (IsRunning)
                {
                    if(sniffer.MessageQueue.TryDequeue(out (byte[],DateTime) messageTuple))
                    {
                        string nextString = Encoding.UTF8.GetString(messageTuple.Item1);
                        if (string.IsNullOrWhiteSpace(nextString))
                        {
                            continue;
                        }
                        MarketTesterUtil.ConsoleDebug("Dequeued a packet : " + nextString);
                        int index8 = nextString.IndexOf(Tag8);
                        int index10 = nextString.IndexOf(Tag10);
                        if (index8 != -1 && index10 != -1)
                        {
                            if (Fix.CheckMessageValidity(nextString))
                            {
                                string msgType = Fix.GetTag(nextString, Tags.MsgType);
                                if(FixValues.MsgTypesOrderEntry.ContainsKey(msgType))
                                {
                                    AddItem(nextString, messageTuple.Item2);
                                }
                            }
                        }
                        else if(index8 == -1 && index10 != -1)
                        {
                            if (TagFound8)
                            {
                                lastMessage += nextString;
                                if (Fix.CheckMessageValidity(nextString))
                                {
                                    string msgType = Fix.GetTag(nextString, Tags.MsgType);
                                    if (FixValues.MsgTypesOrderEntry.ContainsKey(msgType))
                                    {
                                        AddItem(lastMessage, messageTuple.Item2);
                                    }
                                }
                                lastMessage = "";
                                TagFound8 = false;

                            }
                            else
                            {
                                throw new ShouldNotHappenException("A message that involves 10 arrived when there is no message in dump that contains 8 tag");
                            }
                        }
                        else if(index8 != -1 && index10 == -1)
                        {
                            lastMessage = nextString;
                        }
                        else if(index8 == -1 && index10 == -1)
                        {
                            lastMessage += nextString;
                        }
                        
                    }
                }
            }).Start();
            
        }

        public void Stop()
        {
            IsRunning = false;
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
                item = new DiffItem();
                ClOrdIdMap[clOrdID] = item;
                App.Invoke(() =>
                {
                    DiffItems.Add(item);
                });
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
                MarketTesterUtil.ConsoleDebug("Average delay : " + AverageDelay);
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
            private bool IsRunning { get; set; }
            public ConcurrentQueue<(byte[], DateTime)> MessageQueue { get; set; } = new ConcurrentQueue<(byte[], DateTime)>();
            public FixSniffer() 
            {                
                foreach(IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()).ToList())
                {
                    MarketTesterUtil.ConsoleDebug(address.ToString());
                    if (IpV4Address.TryParse(address.ToString(), out IpV4Address ip4address))
                    {
                        LocalHostIpAddresses.Add(ip4address);
                        MarketTesterUtil.ConsoleDebug("--------" + ip4address.ToString());
                    }
                }
            }
           

            public void SetPorts(List<ushort> ports)
            {
                Ports.Clear();
                foreach (ushort port in ports)
                {
                    Ports.Add(port);
                    MarketTesterUtil.ConsoleDebug(port.ToString());
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
                    MarketTesterUtil.ConsoleDebug("Instantiated communicator : " + Communicator.ToString());
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
                            MarketTesterUtil.ConsoleDebug("Started sniffer");
                            while (IsRunning)
                            {
                                Communicator.ReceivePacket(out Packet packet);
                                if(packet != null)
                                    PacketHandler(packet);
                            }
                            //If code reaches here Stop function has been called
                        }).Start();
                    }
                }
            }

            public void Stop()
            {
                string ports = "";
                foreach(ushort port in Ports)
                {
                    ports += port.ToString(CultureInfo.InvariantCulture) + ", ";
                }
                MarketTesterUtil.ConsoleDebug("Stopped Sniffing on ports : " + ports);
                IsRunning = false;
            }

            private void PacketHandler(Packet packet)
            {
                IpV4Datagram ip = packet.Ethernet.IpV4;
                TcpDatagram tcp = ip.Tcp;
                void HandlePacket()
                {
                    byte[] tcpData = tcp.ToArray().SubArray(tcp.HeaderLength, tcp.Length - tcp.HeaderLength);
                    MessageQueue.Enqueue((tcpData, packet.Timestamp));
                }
                if (Ports.Contains(tcp.DestinationPort) || Ports.Contains(tcp.SourcePort))
                {
                    HandlePacket();
                }
                //if (LocalHostIpAddresses.Contains(ip.Source))
                //{
                //    if (Ports.Contains(tcp.SourcePort))
                //    {
                //        HandlePacket();
                //    }
                //}
                //else
                //{
                //    if (Ports.Contains(tcp.DestinationPort))
                //    {
                //        HandlePacket();
                //    }
                //}

            }
            public void Dispose()
            {
                Stop();
            }
        }
    }
}
