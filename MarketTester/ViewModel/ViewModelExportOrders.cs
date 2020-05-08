using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BackOfficeEngine.Helper;
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

        private string infoText;

        public string InfoText
        {
            get { return infoText; }
            set
            {
                infoText = value;
                NotifyPropertyChanged(nameof(InfoText));
            }
        }

        private string infoTextResourceKey;

        public string InfoTextResourceKey
        {
            get { return infoTextResourceKey; }
            set
            {
                if (App.Current.Resources.Contains(value))
                {
                    infoTextResourceKey = value;
                    InfoText = App.Current.Resources[value].ToString();
                    NotifyPropertyChanged(nameof(InfoTextResourceKey));
                }
            }
        }



        #region commands 
        public BaseCommand CommandOkayClick { get; set; }

        public void CommandOkayClickExecute(object param)
        {
            InfoTextResourceKey = ResourceKeys.StringStartedExporting;
            Util.ThreadStart(() =>
            {
                Order.ExportOrders(OutFilePath);
                App.Invoke(() =>
                {
                    InfoTextResourceKey = ResourceKeys.StringFinishedExporting;
                });
            });
        }

        public bool CommandOkayClickCanExecute()
        {
            return Order.Orders.Count != 0;
        }
        #endregion

        public ViewModelExportOrders()
        {
            CommandOkayClick = new BaseCommand(CommandOkayClickExecute, CommandOkayClickCanExecute);
            OutFilePath = MarketTesterUtil.APPLICATION_EXPORT_DIR + "export.orders";
        }
    }
}
