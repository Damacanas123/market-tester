using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using QuickFix;
namespace BackOfficeEngine.Extensions
{
    public static class QuickFixMessageExtension
    {
        //ConditionalWeakTable is available in .NET 4.0+
        //if you use an older .NET, you have to create your own CWT implementation (good luck with that!)
        static readonly ConditionalWeakTable<Message, DateTimeObject> SendTime = new ConditionalWeakTable<Message, DateTimeObject>();
        static readonly ConditionalWeakTable<Message, DateTimeObject> ReceiveTime = new ConditionalWeakTable<Message, DateTimeObject>();

        public static DateTime GetSendTime(this Message message) { return SendTime.GetOrCreateValue(message).Value; }

        public static void SetSendTime(this Message message, DateTime newDatetime) { SendTime.GetOrCreateValue(message).Value = newDatetime; }

        public static DateTime GetReceiveTime(this Message message) { return ReceiveTime.GetOrCreateValue(message).Value; }

        public static void SetReceiveTime(this Message message, DateTime newDatetime) { ReceiveTime.GetOrCreateValue(message).Value = newDatetime; }

        class DateTimeObject
        {
            public DateTime Value;
        }
    }
}
