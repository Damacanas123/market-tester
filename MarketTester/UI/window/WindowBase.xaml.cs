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
using System.Windows.Shapes;

namespace MarketTester.UI.window
{
    /// <summary>
    /// Interaction logic for WindowBase.xaml
    /// </summary>
    public partial class WindowBase : Window
    {
        public bool IsMainWindow { get; private set; }
        public WindowBase(bool isMainWindow,string name)
        {
            IsMainWindow = isMainWindow;
            InitializeComponent();
            UpperBar.SetName(name);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Console.WriteLine(RestoreBounds.Width + "," + RestoreBounds.Height);
                var point = PointToScreen(e.MouseDevice.GetPosition(this));

                if (point.X <= RestoreBounds.Width / 2)
                {
                    Left = 0;
                }


                else if (point.X >= RestoreBounds.Width)
                {
                    Left = point.X - (point.X / this.ActualWidth) * RestoreBounds.Width;
                }


                else
                    Left = point.X - (RestoreBounds.Width / 2);

                Top = point.Y - (((FrameworkElement)sender).ActualHeight / 2);
                WindowState = WindowState.Normal;

            }
            this.DragMove();
        }

        public void SetMenu(UserControl menu)
        {
            UpperBar.SetMenu(menu);
        }
    }
}
