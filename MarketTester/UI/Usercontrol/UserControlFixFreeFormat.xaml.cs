using MarketTester.Helper;
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
    public partial class UserControlFixFreeFormat : UserControl
    {
        public UserControlFixFreeFormat()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridTagValues.UnselectAll();
            DataGridScheduleItems.UnselectAll();
        }
    }
}
