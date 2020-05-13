using BackOfficeEngine.Helper;
using BackOfficeEngine.Model;
using MarketTester.Base;
using MarketTester.Extensions;
using MarketTester.Helper;
using QuickFix.Fields;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.Sniffer
{
    public class DiffItem : BaseNotifier
    {
        private Dictionary<int, string> RequestTagValueMap { get; set; } = new Dictionary<int, string>();
        private Dictionary<int, string> ResponseTagValueMap { get; set; } = new Dictionary<int, string>();
        private string request;
        public string Request
        {
            get 
            {
                return request;
            }
            set
            {
                request = value;
                RequestTagValueMap = MarketTesterUtil.GetTagValuePairs(request);
                SessionID = Fix.GetTag(value, "49");
                SessionID += "----" + Fix.GetTag(value, "56");
                NotifyPropertyChanged(nameof(RequestMsgType));
                NotifyPropertyChanged(nameof(Symbol));
                NotifyPropertyChanged(nameof(TextOrderQty));
            }
        }

        private string response;

        public string Response
        {
            get { return response; }
            set
            {
                response = value;
                ResponseTagValueMap = MarketTesterUtil.GetTagValuePairs(response);
                SessionID = Fix.GetTag(value, "49");
                SessionID += "----" + Fix.GetTag(value, "56");
                NotifyPropertyChanged(nameof(ResponseMsgType));
            }
        }

        

        public string Symbol
        {
            get
            {
                if(RequestTagValueMap.TryGetValue(Tags.Symbol,out string symbol))
                {
                    return symbol;
                }
                return "";
            }
        }

        public string RequestMsgType
        {
            get
            {
                if (RequestTagValueMap.TryGetValue(Tags.MsgType, out string msgType))
                {
                    return msgType;
                }
                return "";
            }
        }

        public string ResponseMsgType
        {
            get
            {
                if (ResponseTagValueMap.TryGetValue(Tags.MsgType, out string msgType))
                {
                    return msgType;
                }
                return "";
            }
        }

        public string TextOrderQty
        {
            get
            {
                if (RequestTagValueMap.TryGetValue(Tags.OrderQty, out string value))
                {
                    return value;
                }
                return "";
            }
        }

        public string TextDelay
        {
            get
            {
                decimal value = (decimal)Delay.GetTotalMicroSeconds();
                if(value == 0)
                {
                    return "";
                }
                return (value/1000m).ToString(CultureInfo.InvariantCulture);
            }
        }

        public string TextRequestTime
        {
            get
            {
                return RequestTime.ToString(MarketTesterUtil.DateFormatMicrosecondPrecision, CultureInfo.InvariantCulture);
            }
        }

        public string TextResponseTime
        {
            get
            {
                return ResponseTime.ToString(MarketTesterUtil.DateFormatMicrosecondPrecision, CultureInfo.InvariantCulture);
            }
        }

        private DateTime requestTime;

        public DateTime RequestTime
        {
            get { return requestTime; }
            set
            {
                requestTime = value;
                NotifyPropertyChanged(nameof(TextRequestTime));
                NotifyPropertyChanged(nameof(TextDelay));
            }
        }

        private DateTime responseTime;

        public DateTime ResponseTime
        {
            get { return responseTime; }
            set
            {
                responseTime = value;
                NotifyPropertyChanged(nameof(TextResponseTime));
                NotifyPropertyChanged(nameof(TextDelay));
            }
        }

        public TimeSpan Delay
        {
            get
            {
                if (ResponseTime != null && RequestTime != null)
                    return ResponseTime - RequestTime;
                else
                    return new TimeSpan(0);
            }
        }


        private string sessionID;

        public string SessionID
        {
            get { return sessionID; }
            set
            {
                sessionID = value;
                NotifyPropertyChanged(nameof(SessionID));
            }
        }



    }
}
