using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

using MarketTester.Extensions;
using MarketTester.UI.Usercontrol.Custom;
using System.Runtime.Remoting.Messaging;

namespace MarketTester.UI.window
{
    public class MainWindowStarter
    {
        private static WindowBase window = new WindowBase(true,"StringAppName");
        private static TabControl tabControl = new TabControl();
        public static void Start()
        {
            
            TabItem item = new TabItem();
            item.Content = new Usercontrol.MultiContainersUC.UserControlMainContainer();
            item.Header = "Main Page";
            tabControl.Items.Add(item);
            window.MainGrid.Children.Add(tabControl);
            System.Windows.Forms.Screen screen = window.GetScreen();
            window.Width = screen.WorkingArea.Width * 0.8;
            window.Height = screen.WorkingArea.Height * 0.8;
            window.SetMenu(new MarketTester.UI.Usercontrol.Menu.Upper.UserControlMainWindowUpperMenu());
            window.Show();
        }        

        public static void AddTab(UserControl content,string nameResourceKey,bool openCheckAlreadyOpened)
        {
            if (openCheckAlreadyOpened)
            {
                foreach(TabItem createdItem in tabControl.Items)
                {
                    if(createdItem.Name.Equals(nameResourceKey))
                    {
                        tabControl.SelectedItem = createdItem;
                        return;
                    }
                }
            }
            
            TabItem item = new TabItem();
            item.Content = content;

            UserControlTabHeader tabHeader = new UserControlTabHeader();
            tabHeader.TextBlockHeader.SetResourceReference(TextBlock.TextProperty, nameResourceKey);
            tabHeader.ButtonCloseTab.tabControl = tabControl;
            tabHeader.ButtonCloseTab.item = item;
            item.Header = tabHeader;
            item.Name = nameResourceKey;
            tabControl.Items.Add(item);
            tabControl.SelectedIndex = tabControl.Items.Count - 1;
        }

        
    }
}
