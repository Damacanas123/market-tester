
using System.Collections.Generic;
using QuickFix.Fields;

namespace FixHelper
{
    public class FixValues
    {
        public static Dictionary<string, string> OrdStatus = new Dictionary<string, string>()
        {
            {"0","New" },
            {"1","Partially Filled" },
            {"2","Filled" },
            {"3","Done for day" },
            {"4","Canceled" },
            {"5","Replaced" },
            {"6","Pending Cancel" },
            {"7","Stopped" },
            {"8","Rejected" },
            {"9","Suspended" },
            {"A","Pending New" },
            {"B","Calculated" },
            {"C","Expired" },
            {"D","Accepted for Bidding" },
            {"E","Pending Replace" },
        };
        public static Dictionary<string, string> ExecType = new Dictionary<string, string>()
        {
            {"0","New" },
            {"3","Done for day" },
            {"4","Canceled" },
            {"5","Replaced" },
            {"6","Pending Cancel" },
            {"7","Stopped" },
            {"8","Rejected" },
            {"9","Suspended" },
            {"A","Pending New" },
            {"B","Calculated" },
            {"C","Expired" },
            {"D","Restated" },
            {"E","Pending Replace" },
            {"F","Trade" },
            {"G","Trade correct" },
            {"H","Trade Cancel" },
            {"I","Order Status" },
            {"J","Trade in a Clearing Hold" },
            {"K","Trade has been released to Clearing" },
            {"L","Triggered or activated by system" },
        };

        public const string NEWORDER_TEXT = "New Order";
        public const string ORDERCANCEL_TEXT = "Order Cancel";
        public const string ORDEREPLACE_TEXT = "Order Replace";
        public const string ORDERCANCELREJECT_TEXT = "Order Cancel Reject";
        public const string EXECUTIONREPORT_TEXT = "Execution Report";
        public const string BUSSINESSMESSAGEREJECT_TEXT = "Bussiness Message Reject";
        public const string SECURITYDEFINITIONREQUEST_TEXT = "Security Definition Request";
        public const string SECURITYSTATUSREQUEST_TEXT = "Security Status Request";
        public const string PRICEREFERENCEREQUEST_TEXT = "Price Reference Request";
        public const string APPLICATIONMESSAGEREQUEST_TEXT = "Application Message Request";
        public static Dictionary<string, string> MsgTypes = new Dictionary<string, string>()
        {
            {"D",NEWORDER_TEXT},
            {"F",ORDERCANCEL_TEXT },
            {"G",ORDEREPLACE_TEXT },
            {"9",ORDERCANCELREJECT_TEXT },
            {"8",EXECUTIONREPORT_TEXT },
            {"j",BUSSINESSMESSAGEREJECT_TEXT },
            {"c",SECURITYDEFINITIONREQUEST_TEXT },
            {"e",SECURITYSTATUSREQUEST_TEXT },
            {"pp",PRICEREFERENCEREQUEST_TEXT },
            {"BW",APPLICATIONMESSAGEREQUEST_TEXT }

        };

        public static Dictionary<string, string> MsgTypesOrderEntry = new Dictionary<string, string>()
        {
            {"D",NEWORDER_TEXT},
            {"F",ORDERCANCEL_TEXT },
            {"G",ORDEREPLACE_TEXT },
            {"9",ORDERCANCELREJECT_TEXT },
            {"8",EXECUTIONREPORT_TEXT }

        };
        public static Dictionary<string, string> MsgTypesOrderEntryOutbound = new Dictionary<string, string>()
        {
            {"D",NEWORDER_TEXT},
            {"F",ORDERCANCEL_TEXT },
            {"G",ORDEREPLACE_TEXT }

        };
        public static Dictionary<string, string> MsgTypesOrderEntryInbound= new Dictionary<string, string>()
        {
            {"8",EXECUTIONREPORT_TEXT},
            {"9",ORDERCANCELREJECT_TEXT },
            

        };
        public static Dictionary<string, string> MsgTypesReverse = new Dictionary<string, string>()
        {
            {NEWORDER_TEXT,"D" },
            {ORDERCANCEL_TEXT,"F" },
            {ORDEREPLACE_TEXT,"G" },
            {ORDERCANCELREJECT_TEXT,"9" },
            {EXECUTIONREPORT_TEXT,"8" },
            {BUSSINESSMESSAGEREJECT_TEXT,"j" },
            {SECURITYDEFINITIONREQUEST_TEXT,"c" },
            {SECURITYSTATUSREQUEST_TEXT,"e" },
            {PRICEREFERENCEREQUEST_TEXT,"pp" },
            {APPLICATIONMESSAGEREQUEST_TEXT,"BW" }

        };


        public static Dictionary<string, string> MsgTypeOutbound = new Dictionary<string, string>()
        {
            {"D",NEWORDER_TEXT },
            {"F",ORDERCANCEL_TEXT },
            {"G",ORDEREPLACE_TEXT },
            {"c",SECURITYDEFINITIONREQUEST_TEXT },
            {"e",SECURITYSTATUSREQUEST_TEXT },
            {"pp",PRICEREFERENCEREQUEST_TEXT },
            {"BW",APPLICATIONMESSAGEREQUEST_TEXT }
        };

        public static Dictionary<string, string> MsgTypeInbound = new Dictionary<string, string>()
        {
            {"9",ORDERCANCELREJECT_TEXT },
            {"8",EXECUTIONREPORT_TEXT },
            {"j",BUSSINESSMESSAGEREJECT_TEXT },
        };

        public const string BUY_TEXT = "Buy";
        public const string SELL_TEXT = "Sell";
        public const string SELLSHORT_TEXT = "Sell Short";

        public static Dictionary<string, string> Sides = new Dictionary<string, string>()
        {
            {"1", BUY_TEXT},
            {"2", SELL_TEXT},
            {"5", SELLSHORT_TEXT},
        };

        public static Dictionary<string, string> SidesReverse = new Dictionary<string, string>()
        {
            {BUY_TEXT,"1"},
            {SELL_TEXT,"2"},
            {SELLSHORT_TEXT,"5"},
        };


        public const string LIMIT_TEXT = "Limit";
        public const string MARKET_TEXT = "Market";
        public const string MARKETWITHLEFTOVERASLIMIT_TEXT = "Market With Left Over as Limit";
        public const string MARKETONCLOSE_TEXT = "Market On Close";

        public static Dictionary<string, string> OrdTypes = new Dictionary<string, string>()
        {
            {"2",LIMIT_TEXT},
            {"1",MARKET_TEXT},
            {"K",MARKETWITHLEFTOVERASLIMIT_TEXT},
            {"5",MARKETONCLOSE_TEXT},
        };

        public static Dictionary<string, string> OrdTypesReverse = new Dictionary<string, string>()
        {
            {LIMIT_TEXT,"2"},
            {MARKET_TEXT,"1"},
            {MARKETWITHLEFTOVERASLIMIT_TEXT,"K"},
            {MARKETONCLOSE_TEXT,"5"},
        };

        public const string DAY_STRING = "Day";
        public const string GTC_STRING = "GTC";
        public const string IOC_STRING = "IOC";
        public const string FOK_STRING = "FOK";
        public const string GTD_STRING = "GTD";
        public const string ATCROSSING_STRING = "At Crossing";
        public const string GTS_STRING = "GTS";

        public static Dictionary<string, string> TimeInForces = new Dictionary<string, string>()
        {
            {"0",DAY_STRING},
            {"1",GTC_STRING},
            {"3",IOC_STRING},
            {"4",FOK_STRING},
            {"6",GTD_STRING},
            {"9",ATCROSSING_STRING},
            {"S",GTS_STRING},
        };

        public static Dictionary<string, string> TimeInForcesReverse = new Dictionary<string, string>()
        {
            {DAY_STRING,"0"},
            {GTC_STRING,"1"},
            {IOC_STRING,"3"},
            {FOK_STRING,"4"},
            {GTD_STRING,"6"},
            {ATCROSSING_STRING,"9"},
            {GTS_STRING,"S"},
        };

        public static Dictionary<int, Dictionary<string, string>> FixTagsToDictionary =
            new Dictionary<int, Dictionary<string, string>>()
        {
                {Tags.MsgType,MsgTypes},
                {Tags.ExecType,ExecType},
                {Tags.OrdStatus,OrdStatus},
        };

        public static Dictionary<string, string> IncomingExecTypesOrderEntry = new Dictionary<string, string>()
        {
            { QuickFix.Fields.ExecType.REJECTED.ToString(),"Rejected"},
            { QuickFix.Fields.ExecType.NEW.ToString(),"New"},
            { QuickFix.Fields.ExecType.REPLACED.ToString(),"Replaced"},
            { QuickFix.Fields.ExecType.CANCELED.ToString(),"Canceled"},
            { QuickFix.Fields.ExecType.PENDING_NEW.ToString(),"Pending New"},
            { QuickFix.Fields.ExecType.PENDING_REPLACE.ToString(),"Pending Replace"},
            { QuickFix.Fields.ExecType.PENDING_CANCEL.ToString(),"Pending Cancel"},
        };
    }
}
