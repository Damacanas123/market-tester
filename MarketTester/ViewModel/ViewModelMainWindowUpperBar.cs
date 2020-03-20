using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackOfficeEngine.Model;

using MarketTester.Base;
using MarketTester.UI.Popup;

namespace MarketTester.ViewModel
{
    public class ViewModelMainWindowUpperBar :BaseNotifier
    {
        #region commands
        public BaseCommand ExportOrdersCommand { get; set; }
        public void ExportOrdersCommandExecute(object parameter)
        {
            PopupManager.OpenGeneralPopup(new UserControlExportOrders());
        }

        public bool ExportOrdersCommandCanExecute()
        {
            return Order.Orders.Count != 0;
        }
        #endregion

        public ViewModelMainWindowUpperBar()
        {
            ExportOrdersCommand = new BaseCommand(ExportOrdersCommandExecute, ExportOrdersCommandCanExecute);
        }
    }
}
