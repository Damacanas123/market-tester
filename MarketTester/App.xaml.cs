using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using MarketTester.UI.window;
using MarketTester.Helper;

namespace MarketTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(Window))
            });
            
            Util.Bootstrap();
            MainWindowStarter.Start();
        }

        public delegate void InvokeFunction();
        public static void Invoke(InvokeFunction func)
        {
            App.Current.Dispatcher.Invoke(func);
        }
    }
}
