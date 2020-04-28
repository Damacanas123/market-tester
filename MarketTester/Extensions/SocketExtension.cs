using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace MarketTester.Extensions
{
    public static class SocketExtension
    {
        static readonly ConditionalWeakTable<Socket, object> SendLocks = new ConditionalWeakTable<Socket, object>();
        static readonly ConditionalWeakTable<Socket, object> ReceiveLocks = new ConditionalWeakTable<Socket, object>();
        public static object GetSendLock(this Socket socket)
        {
            return SendLocks.GetOrCreateValue(socket);
        }
        public static object GetReceiveLock(this Socket socket)
        {
            return ReceiveLocks.GetOrCreateValue(socket);
        }

        public static void Send(this Socket socket, string s)
        {
            byte[] length = BitConverter.GetBytes(s.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(length);
            }
            byte[] data = Encoding.UTF8.GetBytes(s);
            lock (socket.GetSendLock())
            {
                socket.Send(length.Concat(data).ToArray());
            }
        }


        public static string ReadMessage(this Socket socket)
        {
            byte[] lengthBytes = new byte[2];
            int bytesReceived;

            lock (socket.GetReceiveLock())
            {
                bytesReceived = socket.Receive(lengthBytes);
            }
            if (bytesReceived != 2)
            {
                return null;
            }
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
            }
            ushort length = BitConverter.ToUInt16(lengthBytes,0);
            if(length == 0)
            {
                return null;
            }
            byte[] dataBytes = new byte[length];
            int dataLengthReceived = 0;
            ushort tryCount = 0;
            while (dataLengthReceived != length && tryCount < 4)
            {
                lock (socket.GetReceiveLock())
                {
                    dataLengthReceived += socket.Receive(dataBytes, dataLengthReceived, length - dataLengthReceived, SocketFlags.None);
                }
                Thread.Sleep(10);
                tryCount++;
            }
            return Encoding.UTF8.GetString(dataBytes);
        }
    }
}
