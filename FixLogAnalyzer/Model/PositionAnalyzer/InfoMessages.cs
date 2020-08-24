using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixLogAnalyzer.Model
{
    internal static class InfoMessages
    {
        internal static string ClOrdIDNotSet = "ClOrdID not set.";
        internal static string BothOrigClOrdOrderIdNotSet = "Both OrigClOrdID and OrderID are unset.";
        internal static string MsgTypeNotSet = "MsgType not set";
        internal static string ResponseArrivedWhenThereisNoRequest = "A response arrived when there is no request";
        internal static string NoAccountOrTargetSubIdSetOnTradeReport = "No account or sub id set on trade report(150=F)";
        internal static string ExecTypeNotSetOnExecutionReport = "ExecType not set on execution report(35=8)";

    }
}
