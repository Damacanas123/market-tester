using BackOfficeEngine.MessageEnums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Exceptions;
using System.Text.RegularExpressions;
using System.IO;

namespace BackOfficeEngine.Helper
{
    public class Fix
    {
#if ITXR
        public static Dictionary<string, string> ISINCodeMap { get; set; } = new Dictionary<string, string>();
        public static void ITXRBootStrap()
        {
            string isin_path = Util.APPLICATION_STATIC_DIR + "ISINSymbolMap.csv";
            if (!File.Exists(isin_path))
            {
                return;
            }
            foreach(string line in File.ReadLines(isin_path))
            {
                try
                {

                    string[] values = line.Split(',');
                    ISINCodeMap[values[0]] = values[1];
                }
                catch
                {
                    
                }
            }
        }
#endif
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
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }
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

        public static bool IsSetTag(string msg,string tag)
        {
            return -1 != msg.IndexOf(FixDelimiter + tag + "=",StringComparison.InvariantCulture);
        }

        public static bool IsSetTag(string msg, int tag)
        {
            return IsSetTag(msg,tag.ToString(CultureInfo.InvariantCulture));
        }

        public static string ExtractFixMessageFromALine(string line)
        {
            int index, length;
            (index, length) = ExtractFixMessageIndexFromBuffer(line);
            if(index == -1 || length == 0)
            {
                return null;
            }
            else
            {
                return line.Substring(index, length);
            }

        }

        static Regex beginStringRegex = new Regex($"8=.*?{Fix.FixDelimiter}");
        static Regex rgxEnd = new Regex($"{FixDelimiter}10=.*?{FixDelimiter}");
        /// <summary>
        /// Searchs and finds the first eligible fix string and returns the starting index and the length of the message respectively.
        /// Does not control body length and checksum fields in the given string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static (int,int) ExtractFixMessageIndexFromBuffer(string line)
        {
            
            try
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return (-1, 0);
                }
                Match match = beginStringRegex.Match(line);
                int messageStartIndex = match.Index;
                if (messageStartIndex == -1)
                {
                    return (-1, 0);
                }
                string beginString = line.Substring(messageStartIndex + 2, match.Value.Length - 3);
                if (!Fix.FixProtocolStrings.Contains(beginString))
                {
                    throw new InvalidFixBeginString();
                }
                
                Match endMatch = rgxEnd.Match(line);
                if (endMatch.Index == -1)
                {
                    return (messageStartIndex, 0);
                }
                int messageEndIndex = endMatch.Index + endMatch.Value.Length;
                return (messageStartIndex, messageEndIndex - messageStartIndex);
            }
            catch (RegexMatchTimeoutException)
            {
                return (-1,0);
            }
            catch (InvalidFixBeginString)
            {
                return (-1, 0);
            }
            catch (Exception ex)
            {
                Util.LogDebugError(ex);
                return (-1, 0);
            }

        }
    }
}
