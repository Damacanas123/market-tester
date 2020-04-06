using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using MarketTester.UI.window;

namespace MarketTester.UI.Popup
{
    public static class PopupManager
    {
        public static void OpenGeneralPopup(UserControl userControl,string nameResourceKey)
        {
            OpenGeneralPopup(userControl,nameResourceKey, 400, 200);
        }
        public static void OpenGeneralPopup(UserControl userControl,string nameResourceKey,double width,double height)
        {
            WindowBase popupWindow = new WindowBase(false,nameResourceKey);
            popupWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            popupWindow.MainGrid.Children.Add(userControl);
            popupWindow.Width = width;
            popupWindow.Height = height;
            popupWindow.ShowDialog();
        }

        public static void OpenErrorPopup(UserControlErrorPopup popup)
        {
            OpenGeneralPopup(popup, ResourceKeys.StringErrorPopupName);
        }
    }
}
