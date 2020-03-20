using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Model
{
    internal static class MessageManager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        internal static ProtocolType GetMsgProtocolType(string msg)
        {
            if (msg.Contains("FIXT.1.1"))
            {
                return ProtocolType.Fix50sp2;
            }
            else
            {
                throw new Exception("Unimplemented protocol type in message string");
            }
        }
    }
}
