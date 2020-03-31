using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BackOfficeEngine;
using BackOfficeEngine.Model;

using MarketTester.Base;
using MarketTester.UI.Popup;
using MarketTester.UI.Usercontrol;
using MarketTester.UI.window;
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
            CommandCancelAllOrders = new BaseCommand(CommandCancelAllOrdersExecute, CommandCancelAllOrdersCanExecute);
            CommandLogAnalyzers = new BaseCommand(CommandLogAnalyzersExecute, CommandLogAnalyzersCanExecute);
        }
        #region commands
        #region CommanExportOrders
        public BaseCommand CommandExportOrders { get; set; }
        public void CommandExportOrdersExecute(object parameter)
        {
            PopupManager.OpenGeneralPopup(new UserControlExportOrders(),"StringExportOrders",300,150);
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
            openFileDialog.InitialDirectory = Util.APPLICATION_EXPORT_DIR;
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

        #region CommandCancelAllOrders
        public BaseCommand CommandCancelAllOrders { get; set; }

        public void CommandCancelAllOrdersExecute(object param)
        {
            Engine.GetInstance().CancelAllOrders();
        }

        public bool CommandCancelAllOrdersCanExecute()
        {
            return true;
        }
        #endregion

        #region CommandLogAnalyzers
        public BaseCommand CommandLogAnalyzers { get; set; }
        public void CommandLogAnalyzersExecute(object param)
        {
            MainWindowStarter.AddTab(new UserControlLogAnalyzer(), "StringLogAnalyzers", true);
        }
        public bool CommandLogAnalyzersCanExecute()
        {
            return true;
        }
        #endregion

        #endregion




    }
}
