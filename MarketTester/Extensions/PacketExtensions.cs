using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Transport;

namespace MarketTester.Extensions
{
    public static class PacketExtensions
    {
        /// <summary>
        /// compare two TCP packets. Packet with a lower sequence num has a lower order and if the sequence nums are 
        /// equal packet with more data has lower order in order to prevent data misses because of retransmission of data. 
        /// Packets may be merged together when retransmitted.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int TcpSeqCompare(this Packet first, Packet second)
        {
            if (ReferenceEquals(first, second)) return 0;
            if ((first == null) || (second == null)) return -1;
            //Compare two object's class, return false if they are difference
            if (first.GetType() != second.GetType()) return -1;
            TcpDatagram tcp1 = first.Ethernet.IpV4.Tcp;
            TcpDatagram tcp2 = second.Ethernet.IpV4.Tcp;
            if (tcp1.SequenceNumber < tcp2.SequenceNumber)
            {
                return -1;
            }
            else if(tcp1.SequenceNumber == tcp2.SequenceNumber)
            {
                if(tcp1.PayloadLength > tcp2.PayloadLength)
                {
                    return -1;
                }
                else if( tcp1.PayloadLength == tcp2.PayloadLength)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
    }
}
