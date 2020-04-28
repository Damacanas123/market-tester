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
using MarketTester.Events;

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
            window.Width = 1536;
            window.Height = 864;
            window.SetMenu(new MarketTester.UI.Usercontrol.Menu.Upper.UserControlMainWindowUpperMenu());
            window.Show();
        }        

        public static void AddTab(UserControl content,string nameResourceKey,bool openCheckAlreadyOpened)
        {
            AddTab(content, nameResourceKey, openCheckAlreadyOpened, null);
        }

        public static void AddTab(UserControl content,string nameResourceKey,bool openCheckAlreadyOpened,OnCloseEventHandler onClose)
        {
            if (openCheckAlreadyOpened)
            {
                foreach (TabItem createdItem in tabControl.Items)
                {
                    if (createdItem.Name.Equals("k" + nameResourceKey))
                    {
                        tabControl.SelectedItem = createdItem;
                        return;
                    }
                }
            }

            TabItem item = new TabItem();
            item.Content = content;

            UserControlTabHeader tabHeader = new UserControlTabHeader();
            if (App.Current.Resources.Contains(nameResourceKey))
                tabHeader.TextBlockHeader.SetResourceReference(TextBlock.TextProperty, nameResourceKey);
            else
                tabHeader.TextBlockHeader.Text = nameResourceKey;
            tabHeader.ButtonCloseTab.tabControl = tabControl;
            tabHeader.ButtonCloseTab.item = item;
            item.Header = tabHeader;
            item.Name = "k" + nameResourceKey;
            tabControl.Items.Add(item);
            tabControl.SelectedIndex = tabControl.Items.Count - 1;

            if(onClose != null)
            {
                tabHeader.ButtonCloseTab.OnCloseEvent += onClose;
            }
        }

        
    }
}
