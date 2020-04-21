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

namespace MarketTester.Model.Sniffer
{
    public class FixDelayHandler
    {
        private static string Tag8 = "8=";
        private static string Tag10 = "\u000110=";
        private string lastMessage { get; set; }
        private bool IsRunning { get; set; }
        private bool TagFound8 { get; set; }
        private bool TagFound10 { get; set; }
        private FixSniffer sniffer { get; set; }
        private static List<IPAddress> HostIpAddresses { get; } = Dns.GetHostAddresses(Dns.GetHostName()).ToList();
        public FixDelayHandler()
        {
            sniffer = new FixSniffer();
        }
        public FixDelayHandler(LivePacketDevice device, List<ushort> ports)
        {
            sniffer = new FixSniffer(device, ports, HostIpAddresses);
        }

        public void SetDevice(LivePacketDevice device)
        {
            sniffer.SetDevice(device);
        }
        
        public void Start()
        {
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
            }
            if (FixHelper.FixValues.MsgTypesOrderEntryOutbound.ContainsKey(msgType))
            {
                item.Request = message;
                item.RequestTime = timeStamp;
            }
            else
            {
                item.Response = message;
                item.ResponseTime = timeStamp;
            }
            DiffItems.SupressNotification = true;
            DiffItems.Add(item);
            DiffItems.SupressNotification = false;
        }

        private class FixSniffer : IDisposable
        {
            private PacketDevice Device { get; set; }
            private PacketCommunicator Communicator { get; set; }
            private List<ushort> Ports { get; set; } = new List<ushort>();
            private List<IpV4Address> HostIpAddresses { get; set; } = new List<IpV4Address>();
            private bool IsRunning { get; set; }
            public ConcurrentQueue<(byte[], DateTime)> MessageQueue { get; set; } = new ConcurrentQueue<(byte[], DateTime)>();
            public FixSniffer() { }
            public FixSniffer(PacketDevice device, List<ushort> ports, List<IPAddress> hostIpAddresses)
            {
                Device = device;
                Ports = ports;
                foreach (IPAddress address in hostIpAddresses)
                {
                    if (IpV4Address.TryParse(address.ToString(), out IpV4Address ip4address))
                    {
                        HostIpAddresses.Add(ip4address);
                    }
                }
                if (Device != null)
                {
                    Communicator = Device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
                    using (BerkeleyPacketFilter filter = Communicator.CreateFilter("ip and tcp"))
                    {
                        // Set the filter
                        Communicator.SetFilter(filter);
                    }
                }
                else
                {
                    throw new DeviceCantBeNull("Given device is null");
                }

            }

            public void SetDevice(LivePacketDevice device)
            {
                if (Device != null)
                {
                    Communicator = Device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
                    using (BerkeleyPacketFilter filter = Communicator.CreateFilter("ip and tcp"))
                    {
                        // Set the filter
                        Communicator.SetFilter(filter);
                    }
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
                            while (IsRunning)
                            {
                                Communicator.ReceivePacket(out Packet packet);
                                PacketHandler(packet);
                            }
                            //If code reaches here Stop function has been called
                        }).Start();
                    }
                }
            }

            public void Stop()
            {
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
                if (HostIpAddresses.Contains(ip.Source))
                {
                    if (Ports.Contains(tcp.SourcePort))
                    {
                        HandlePacket();
                    }
                }
                else
                {
                    if (Ports.Contains(tcp.DestinationPort))
                    {
                        HandlePacket();
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
