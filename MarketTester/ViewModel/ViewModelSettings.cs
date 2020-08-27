using BackOfficeEngine.GeneralBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Helper;
using BackOfficeEngine;
namespace MarketTester.ViewModel
{
    public class ViewModelSettings : BaseNotifier
    {
        private SymbolISIN symbolISIN;

        public SymbolISIN SymbolISIN
        {
            get { return symbolISIN; }
            set
            {
                symbolISIN = value;
                SettingsBackOfficeEngine.Instance.SymbolISINSetting = value;
                NotifyPropertyChanged(nameof(SymbolISIN));
            }
        }

        public ViewModelSettings()
        {
            symbolISIN = SettingsBackOfficeEngine.Instance.SymbolISINSetting;
        }

    }
}
