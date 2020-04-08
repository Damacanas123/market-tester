using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace MarketTester.Helper
{
    public static class UIUtil
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static string SaveFileDialog(string [] filters)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            foreach(string filter in filters)
            {
                dialog.Filter += filter + " file|*." + filter;
            }            
            dialog.InitialDirectory = Util.APPLICATION_FREEFORMATSCHEDULE_DIR;
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public static string OpenFileDialog(string[] filters)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Util.APPLICATION_FREEFORMATSCHEDULE_DIR;
            foreach (string filter in filters)
            {
                dialog.Filter += filter + " file|*." + filter;
            }
            dialog.ShowDialog();
            return dialog.FileName;
        }
    }
}
