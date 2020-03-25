using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


using BackOfficeEngine.Model;
using MarketTester.Base;
using MarketTester.Helper;

namespace MarketTester.ViewModel
{
    public class ViewModelExportOrders : BaseNotifier
    {
        private string outFilePath;
        public string OutFilePath
        {
            get
            {
                return outFilePath;
            }
            set
            {
                if(outFilePath != value)
                {
                    outFilePath = value;
                    NotifyPropertyChanged(nameof(OutFilePath));
                }
            }
        }

        #region commands 
        public BaseCommand CommandOkayClick { get; set; }

        public void CommandOkayClickExecute(object param)
        {
            new Thread(() =>
            {
                Order.ExportOrders(OutFilePath);
            }).Start();
        }

        public bool CommandOkayClickCanExecute()
        {
            return Order.Orders.Count != 0;
        }
        #endregion

        public ViewModelExportOrders()
        {
            CommandOkayClick = new BaseCommand(CommandOkayClickExecute, CommandOkayClickCanExecute);
            OutFilePath = Util.APPLICATION_EXPORT_DIR + "export.orders";
        }
    }
}
