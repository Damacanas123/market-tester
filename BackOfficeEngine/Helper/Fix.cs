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
        public const char FixDelimiter = (char)1;
        public static string GetFixString(ProtocolType protocolType,(string,string) [] tagValuePairs)
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
            Console.WriteLine(msg.Length);
            uint checksumTotal = 0;
            msg += FixDelimiter;
            for (int i = 0; i < msg.Length; checksumTotal += msg[i++]) ;
            string checksum = (checksumTotal % 256).ToString(CultureInfo.InvariantCulture);
            msg += "10=" + checksum + FixDelimiter;
            return msg;
        }
    }
}
