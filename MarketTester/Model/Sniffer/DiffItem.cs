using BackOfficeEngine.Helper;
using BackOfficeEngine.Model;
using MarketTester.Base;
using QuickFix.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.Sniffer
{
    public class DiffItem : BaseNotifier
    {

        private string request;

        public string Request
        {
            get { return request; }
            set
            {
                request = value;
                NotifyPropertyChanged(nameof(Request));
            }
        }

        private string response;

        public string Response
        {
            get { return response; }
            set
            {
                response = value;
                NotifyPropertyChanged(nameof(Response));
            }
        }

        private DateTime requestTime;

        public DateTime RequestTime
        {
            get { return requestTime; }
            set
            {
                requestTime = value;
                NotifyPropertyChanged(nameof(RequestTime));
                NotifyPropertyChanged(nameof(Delay));
            }
        }

        private DateTime responseTime;

        public DateTime ResponseTime
        {
            get { return responseTime; }
            set
            {
                responseTime = value;
                NotifyPropertyChanged(nameof(ResponseTime));
                NotifyPropertyChanged(nameof(Delay));
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
        
    }
}
