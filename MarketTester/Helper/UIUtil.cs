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

        

        

        public static string SaveFileDialog(string[] filters,string initialDirectory)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            foreach (string filter in filters)
            {
                dialog.Filter += filter + " file|*." + filter;
            }
            dialog.InitialDirectory = initialDirectory;
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public static string OpenFileDialog(string[] filters, string initialDirectory)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = initialDirectory;
            foreach (string filter in filters)
            {
                dialog.Filter += filter + " file|*." + filter;
            }
            dialog.ShowDialog();
            return dialog.FileName;
        }
    }
}
