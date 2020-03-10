using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BackOfficeEngine.Model;

using MarketTester.Base;
using MarketTester.Connection;

namespace MarketTester.ViewModel
{
    public class ViewModelOrderList : BaseNotifier
    {
        public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();
        public ViewModelOrderList()
        {
            Order.Orders.CollectionChanged += OnOrdersCollectionChanged;
        }

        private void OnOrdersCollectionChanged(object sender,System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Orders = Order.Orders;
        }
        
    }
}
