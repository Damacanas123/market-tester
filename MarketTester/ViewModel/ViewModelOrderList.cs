using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BackOfficeEngine.Model;

using MarketTester.Base;
using MarketTester.Connection;
using MarketTester.UI.window;
using MarketTester.UI.Usercontrol;

namespace MarketTester.ViewModel
{
    public class ViewModelOrderList : BaseNotifier
    {
        private Order selectedOrder;
        public Order SelectedOrder
        {
            get
            {
                return selectedOrder;
            }
            set
            {
                selectedOrder = value;
                NotifyPropertyChanged(nameof(SelectedOrder));
            }
        }
        public ViewModelOrderList()
        {
            CommandOrderReplace = new BaseCommand(CommandOrderReplaceExecute, CommandOrderReplaceCanExecute);
        }

        public BaseCommand CommandOrderReplace { get; set; }

        public void CommandOrderReplaceExecute(object param)
        {
            WindowBase window = new WindowBase(false,"Order : " + SelectedOrder.NonProtocolID);
            UserControlOrderEntry1 uc = new UserControlOrderEntry1();
            window.MainGrid.Children.Add(uc);
            window.Width = 400;
            window.Height = 250;
            ViewModelOrderEntry vmOrderEntry = (ViewModelOrderEntry)uc.DataContext;
            vmOrderEntry.Side = SelectedOrder.Side;
            vmOrderEntry.Order = SelectedOrder;
            window.Show();

        }

        public bool CommandOrderReplaceCanExecute()
        {
            return true;
        }
        
        
    }
}
