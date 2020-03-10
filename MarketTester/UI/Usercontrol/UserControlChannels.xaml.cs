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

using MarketTester.UI.Usercontrol.Custom;

namespace MarketTester.UI.Usercontrol
{
    /// <summary>
    /// Interaction logic for UserControlChannels.xaml
    /// </summary>
    public partial class UserControlChannels : UserControl
    {
        public UserControlChannels()
        {
            InitializeComponent();
            UserControlFlashingEllipse activeFlashingEllipse = new UserControlFlashingEllipse(13, 13, 750, 4.0 / 3.0,
                (SolidColorBrush)Application.Current.FindResource("ActiveGreen"),
                (SolidColorBrush)Application.Current.FindResource("ActiveGreen"));
            Grid.SetColumn(activeFlashingEllipse, 0);
            GridActiveSessionHeader.Children.Insert(0, activeFlashingEllipse);

            UserControlFlashingEllipse inactiveFlashingEllipse = new UserControlFlashingEllipse(13, 13, 750, 4.0 / 3.0,
                (SolidColorBrush)Application.Current.FindResource("InactiveRed"),
                (SolidColorBrush)Application.Current.FindResource("InactiveRed"));
            Grid.SetColumn(inactiveFlashingEllipse, 0);
            GridInactiveSessionHeader.Children.Insert(0, inactiveFlashingEllipse);
        }
    }
}
