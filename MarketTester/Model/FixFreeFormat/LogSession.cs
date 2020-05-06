using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketTester.Base;

namespace MarketTester.Model.FixFreeFormat
{
    public class LogSession : BaseNotifier
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                NotifyPropertyChanged(nameof(IsChecked));
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
