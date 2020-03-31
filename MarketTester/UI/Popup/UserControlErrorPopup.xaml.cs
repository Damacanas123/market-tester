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

namespace MarketTester.UI.Popup
{
    /// <summary>
    /// Interaction logic for UserControlErrorPopup.xaml
    /// </summary>
    public partial class UserControlErrorPopup : UserControl
    {
        public const string nameResourceKey = "StringErrorPopupName";
        public UserControlErrorPopup()
        {
            InitializeComponent();
        }
    }
}
