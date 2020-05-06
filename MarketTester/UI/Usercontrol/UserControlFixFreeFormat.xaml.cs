using MarketTester.Helper;
using MarketTester.UI.Interface;
using MarketTester.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarketTester.UI.Usercontrol
{
    /// <summary>
    /// Interaction logic for UserControlFixFreeFormat.xaml
    /// </summary>
    public partial class UserControlFixFreeFormat : UserControl,ITabOnCloseHandler
    {
        public UserControlFixFreeFormat()
        {
            InitializeComponent();
            this.DataContext = new ViewModelFixFreeFormat();
            ((ViewModelFixFreeFormat)DataContext).View = this;
        }

        public void OnClose()
        {
            ((ViewModelFixFreeFormat)DataContext).OnClose();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridTagValues.UnselectAll();
            DataGridScheduleItems.UnselectAll();
        }
    }
}
