using MarketTester.UI.Interface;
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
using MarketTester.ViewModel;

namespace MarketTester.UI.Usercontrol
{
    /// <summary>
    /// Interaction logic for UserControlFixSniffer.xaml
    /// </summary>
    public partial class UserControlFixSniffer : UserControl,ITabOnCloseHandler
    {
        public UserControlFixSniffer()
        {
            InitializeComponent();
        }

        public void OnClose()
        {
            ((ViewModelFixSniffer)this.DataContext).OnClose();
        }
    }
}
