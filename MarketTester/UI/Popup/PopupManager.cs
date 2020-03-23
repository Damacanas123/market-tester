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
        public static void OpenGeneralPopup(UserControl userControl)
        {
            OpenGeneralPopup(userControl, 400, 200);
        }
        public static void OpenGeneralPopup(UserControl userControl,double width,double height)
        {
            WindowBase popupWindow = new WindowBase(false,"Export Orders");
            popupWindow.MainGrid.Children.Add(userControl);
            popupWindow.Width = width;
            popupWindow.Height = height;
            popupWindow.ShowDialog();
        }
    }
}
