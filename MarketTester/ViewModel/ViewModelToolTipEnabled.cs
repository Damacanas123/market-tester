using BackOfficeEngine.GeneralBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.ViewModel
{
    public class ViewModelToolTipEnabled : BaseNotifier
    {
        private static bool toolTipEnabled = false;
        public bool ToolTipEnabled
        {
            get
            {
                return toolTipEnabled;
            }
            set
            {
                toolTipEnabled = value;
                NotifyPropertyChanged(nameof(ToolTipEnabled));
            }
        }
    }
}
