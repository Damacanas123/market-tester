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

namespace MarketTester.UI.Usercontrol.Custom
{
    /// <summary>
    /// Interaction logic for UserControlCloseButton.xaml
    /// </summary>
    public partial class UserControlCloseButton : UserControl
    {
        public UserControlCloseButton()
        {
            InitializeComponent();
        }
        public TabControl tabControl;
        public TabItem item;
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            tabControl.Items.Remove(item);
        }
    }
}
