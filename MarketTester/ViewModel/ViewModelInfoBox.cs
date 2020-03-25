using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Base;
using MarketTester.ViewModel.Manager;
using MarketTester.Enumeration;

namespace MarketTester.ViewModel
{
    public class ViewModelInfoBox : BaseNotifier
    {
        public ViewModelInfoBox()
        {
            InfoManager.InfoViewModels.Add(this);
            CommandClearInfoText = new BaseCommand(CommandClearInfoTextExecute, CommandClearInfoTextCanExecute);
        }
        private string infoText;
        public string InfoText { 
            get => infoText; 
            set 
            { 
                infoText = value; 
                NotifyPropertyChanged(nameof(InfoText)); 
            } 
        }

        private EInfo infoPriority;
        public EInfo InfoPriority { get => infoPriority; set { infoPriority = value; NotifyPropertyChanged(nameof(InfoPriority)); } }

        #region commands
        public BaseCommand CommandClearInfoText { get; set; }

        public void CommandClearInfoTextExecute(object param)
        {
            InfoText = "";
        }

        public bool CommandClearInfoTextCanExecute()
        {
            return true;
        }

        #endregion

    }
}
