using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

using MarketTester.Extensions;
using MarketTester.UI.Usercontrol;

namespace MarketTester.UI.window
{
    public class MainWindowStarter
    {
        WindowBase window = new WindowBase();
        public MainWindowStarter()
        {
            window.MainGrid.Children.Add(new Usercontrol.MultiContainersUC.UserControlMainContainer());
            System.Windows.Forms.Screen screen = window.GetScreen();
            window.Width = screen.WorkingArea.Width * 0.8;
            window.Height = screen.WorkingArea.Height * 0.8;
            window.Show();
        }        
    }
}
