using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Worksheet
{
    public class ExcelConstants
    {
        public const string MESSAGE_TYPE = "Message Type";
        public const string MESSAGE_TYPE_NEW = "New";
        public const string MESSAGE_TYPE_REPLACE = "Replace";
        public const string MESSAGE_TYPE_CANCEL = "Cancel";
        public const string MESSAGE_TYPE_EXECUTION_REPORT = "Execution Rep.";
        public const string MESSAGE_TYPE_ORDERCANCELREJECT = "Cancel Reject";

        public const string EXEC_TYPE = "Exec Type";
        public const string ORD_STATUS = "Ord Status";

        public const string COMMENT = "Comment";


        public const string CLORDID = "ClOrdId";
        public const string ORIGCLORDID = "OrigClOrdId";

        public const string ORDERQTY = "OrderQty";

        public const string CUMQTY = "CumQty";

        public const string LEAVESQTY = "LeavesQty";

        public const string LASTQTY = "Last Shares";

        public const string PRICE = "Price";

        public const string LASTPX = "LastPx";

        public const string AVGPX = "AvgPx";

        public const string SEND_SEND_DIFF = "Send-Send Diff(milli sec)";

        public const string SEND_RECEIVE_DIFF = "Send-Receive Diff(milli sec)";

        public const string SEND_TIME = "Send time";

        public const string RECEIVE_TIME = "Receive time";

        public const string MESSAGE_ORIGIN = "Origin";
        public const string MESSAGE_ORIGIN_INBOUND = "Inbound";
        public const string MESSAGE_ORIGIN_OUTBOUND = "Outbound";

        public const string MESSAGE_FIX_RAW = "Fix Raw";
        public const string MESSAGE_FIX_PARSED = "Fix Parsed";

        public const string DATE_TIME_FORMAT = "yyyyMMdd-HH:mm:ss.fff";
        public const string ORDERID = "OrderId";
        public const string EXECID = "ExecId";
        public const double EXCEL_MAXROWHEIGHT = 409;
    }
}
