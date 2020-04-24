using BackOfficeEngine.MessageEnums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Helper
{
    public class Fix
    {
        
        public static HashSet<string> FixProtocolStrings { get; } = new HashSet<string>()
        {
            "FIXT.1.1","FIX.4.0","FIX.4.1","FIX.4.2","FIX.4.3","FIX.4.4"
        };
        public static HashSet<string> CantBeEditedTags { get; } = new HashSet<string>()
        {
            "8","9","10"
        };

        public static HashSet<MsgType> OrderEntryOutboundMessageTypes { get; } = new HashSet<MsgType>()
        {
            MsgType.New,MsgType.Replace,MsgType.Cancel
        };
        public const char FixDelimiter = (char)1;
        public static string GetFixString(ProtocolType protocolType,List<(string,string)> tagValuePairs)
        {
            string msg = "8=";
            switch (protocolType)
            {
                case ProtocolType.Fix50sp2:
                    msg += "FIXT.1.1";
                    
                    break;
                case ProtocolType.Fix50:
                    msg += "FIXT.1.1";
                    break;
                case ProtocolType.Fix40:
                    msg += "FIX.4.0";
                    break;
                case ProtocolType.Fix41:
                    msg += "FIX.4.1";
                    break;
                case ProtocolType.Fix42:
                    msg += "FIX.4.2";
                    break;
                case ProtocolType.Fix43:
                    msg += "FIX.4.3";
                    break;
                case ProtocolType.Fix44:
                    msg += "FIX.4.4";
                    break;
            }
            int initialLen = msg.Length;
            foreach((string,string) pair in tagValuePairs)
            {
                msg += FixDelimiter + pair.Item1 + "=" + pair.Item2;
            }
            
            short msgLength = (short)(msg.Length - initialLen);
            msg = msg.Insert(initialLen,FixDelimiter + "9=" + msgLength );
            uint checksumTotal = 0;
            msg += FixDelimiter;
            for (int i = 0; i < msg.Length; checksumTotal += msg[i++]) ;
            string checksum = (checksumTotal % 256).ToString(CultureInfo.InvariantCulture);
            msg += "10=" + checksum + FixDelimiter;
            return msg;
        }

        /// <summary>
        /// This functions checks validity of 9 and 10 tags. It also checks if 8 tag exists
        /// </summary>
        /// <returns></returns>
        public static bool CheckMessageValidity(string message)
        {
            if(message[0] != '8')
            {
                return false;
            }
            int firstDelimeter = message.IndexOf(FixDelimiter);
            string fixProtocol = message.Substring(2, firstDelimeter - 2);
            if (!FixProtocolStrings.Contains(fixProtocol))
            {
                return false;
            }
            int bodyLengthStartIndex = message.IndexOf(FixDelimiter + "9=") + 3;
            if(bodyLengthStartIndex == -1)
            {
                return false;
            }
            int bodyLengthEndIndex = message.IndexOf(FixDelimiter, bodyLengthStartIndex);
            if(bodyLengthStartIndex == -1)
            {
                return false;
            }
            if(!int.TryParse(message.Substring(bodyLengthStartIndex, bodyLengthEndIndex - bodyLengthStartIndex)
                ,NumberStyles.Integer,CultureInfo.InvariantCulture,out int bodyLength))
            {
                return false;
            }
            int bodyStartIndex = bodyLengthEndIndex + 1;
            int bodyEndIndex = message.IndexOf(FixDelimiter + "10=") + 1;
            if (bodyEndIndex == -1)
            {
                return false;
            }
            if(bodyEndIndex - bodyStartIndex != bodyLength)
            {
                return false;
            }
            int calculatedChecksum = 0;
            string body = message.Substring(0, bodyEndIndex);
            for (int i = 0; i < bodyEndIndex; calculatedChecksum += message[i++]) ;
            calculatedChecksum %= 256;
            int checksumStartIndex = bodyEndIndex + 3;
            int checkSumEndIndex = message.Length - 1;
            if (!int.TryParse(message.Substring(checksumStartIndex, checkSumEndIndex - checksumStartIndex)
                , NumberStyles.Integer, CultureInfo.InvariantCulture, out int checksum))
            {
                return false;
            }
            if(calculatedChecksum != checksum)
            {
                return false;
            }
            return true;
        }

        public static string GetTag(string msg, string tag)
        {
            string searchString = $"\u0001{tag}=";
            int startIndex = msg.IndexOf(searchString) + searchString.Length;
            if (startIndex == -1)
                return "";
            int endIndex = msg.IndexOf("\u0001", startIndex);
            return msg.Substring(startIndex, endIndex - startIndex);
        }
        public static string GetTag(string msg, int tag)
        {
            return GetTag(msg, tag.ToString(CultureInfo.InvariantCulture));
        }

    }
}
