using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BackOfficeEngine.Model;

using MarketTester.Base;
using MarketTester.UI.Popup;
using MarketTester.Helper;
using MarketTester.Enumeration;

namespace MarketTester.ViewModel
{
    public class ViewModelMainWindowUpperBar :BaseNotifier
    {
        public ViewModelMainWindowUpperBar()
        {
            CommandExportOrders = new BaseCommand(CommandExportOrdersExecute, CommandExportOrdersCanExecute);
            CommandClearOrders = new BaseCommand(CommandClearOrdersExecute, CommandClearOrdersCanExecute);
            CommandImportOrders = new BaseCommand(CommandImportOrdersExecute, CommandImportOrdersCanExecute);
            CommandChangeLanguage = new BaseCommand(CommandChangeLanguageExecute, CommandChangeLanguageCanExecute);
            
        }
        #region commands
        #region CommanExportOrders
        public BaseCommand CommandExportOrders { get; set; }
        public void CommandExportOrdersExecute(object parameter)
        {
            PopupManager.OpenGeneralPopup(new UserControlExportOrders(),300,150);
        }

        public bool CommandExportOrdersCanExecute()
        {
            return Order.Orders.Count != 0;
        }
        #endregion

        #region CommandClearOrders
        public BaseCommand CommandClearOrders { get; set; }

        public void CommandClearOrdersExecute(object param)
        {
            Order.ClearOrders();
        }

        public bool CommandClearOrdersCanExecute()
        {
            return Order.Orders.Count != 0;
        }
        #endregion

        #region CommandImportOrders
        public BaseCommand CommandImportOrders { get; set; }

        public void CommandImportOrdersExecute(object param)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != null)
            {
                try
                {
                    Order.ImportOrders(openFileDialog.FileName);
                }
                catch(Exception ex)
                {
                    Util.LogError(ex);
                }
                
            }
            
        }
        public bool CommandImportOrdersCanExecute()
        {
            return true;
        }
        #endregion

        #region CommandChangeLanguage
        public BaseCommand CommandChangeLanguage { get; set; }

        public void CommandChangeLanguageExecute(object param)
        {
            Settings.GetInstance().Language = (ELanguage)param;
        }

        public bool CommandChangeLanguageCanExecute()
        {
            return true;
        }
        #endregion

        #endregion




    }
}
