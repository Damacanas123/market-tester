using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuickFix
{
    public class Helper
    {
        public const string FixDelimiter = "\u0001";
        public const string UTCTimestampFormat = "yyyyMMdd-HH:mm:ss.fff";
        public static string CorrectBodyLengthSendingTimeCheckSum(string msg)
        {
            int sendingTimeStartIndex = msg.IndexOf(FixDelimiter + "52=");
            if (sendingTimeStartIndex != -1)
            {
                sendingTimeStartIndex += 4;
                int sendingTimeEndIndex = msg.IndexOf(FixDelimiter, sendingTimeStartIndex);
                msg = msg.Substring(0, sendingTimeStartIndex) + DateTime.Now.ToString(UTCTimestampFormat) + msg.Substring(sendingTimeEndIndex, msg.Length - sendingTimeEndIndex);
            }
            int bodyLengthTagStart = msg.IndexOf(FixDelimiter + "9=") + 3;

            int bodyLengthTagEnd = msg.IndexOf(FixDelimiter, bodyLengthTagStart);
            int bodyStartIndex = msg.IndexOf(FixDelimiter, bodyLengthTagStart) + 1 ;
            int bodyEndIndex = msg.IndexOf(FixDelimiter + "10=") + 1;
            string bodyLength = (bodyEndIndex - bodyStartIndex).ToString(CultureInfo.InvariantCulture);
            msg = msg.Substring(0, bodyLengthTagStart) + bodyLength + msg.Substring(bodyLengthTagEnd, msg.Length - bodyLengthTagEnd);
            uint checksumTotal = 0;
            for (int i = 0; i < bodyEndIndex; checksumTotal += msg[i++]);
            string checksum = (checksumTotal % 256).ToString(CultureInfo.InvariantCulture);
            int checkSumStartIndex = msg.IndexOf(FixDelimiter + "10=") + 4;
            msg = msg.Substring(0, checkSumStartIndex) + checksum + FixDelimiter;
            return msg;
        }
    }
}
