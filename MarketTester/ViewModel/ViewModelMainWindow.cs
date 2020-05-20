using BackOfficeEngine.GeneralBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.ViewModel
{
    public class ViewModelMainWindow : BaseNotifier
    {
        private bool useQAtTheBeginning;

        public bool UseQAtTheBeginning
        {
            get { return useQAtTheBeginning; }
            set
            {
                useQAtTheBeginning = value;
                BackOfficeEngine.Engine.GetInstance().SetPutQInTheBeginning(value);
                NotifyPropertyChanged(nameof(UseQAtTheBeginning));                
            }
        }

    }
}
