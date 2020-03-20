using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

using MarketTester.Extensions;
using MarketTester.UI;

namespace MarketTester.UI.window
{
    public class MainWindowStarter
    {
        WindowBase window = new WindowBase(true);
        public MainWindowStarter()
        {
            window.MainGrid.Children.Add(new Usercontrol.MultiContainersUC.UserControlMainContainer());
            System.Windows.Forms.Screen screen = window.GetScreen();
            window.Width = screen.WorkingArea.Width * 0.8;
            window.Height = screen.WorkingArea.Height * 0.8;
            window.SetMenu(new MarketTester.UI.Usercontrol.Menu.Upper.UserControlMainWindowUpperMenu());
            window.Show();
        }        
    }
}
