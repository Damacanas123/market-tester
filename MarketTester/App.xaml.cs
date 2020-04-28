using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using MarketTester.UI.window;
using MarketTester.Helper;
using BackOfficeEngine.Helper;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using MarketTester.UI.Popup;

namespace MarketTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DispatcherUnhandledException += CurrentDomain_UnhandledException;
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(Window))
            });
            MarketTesterUtil.Bootstrap();
            MainWindowStarter.Start();
        }

        void CurrentDomain_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Util.LogError(e.Exception);
            PopupManager.OpenErrorPopup(new UserControlErrorPopup(e.Exception.ToString()));
            Environment.Exit(1);
        }

        public delegate void InvokeFunction();
        public static void Invoke(InvokeFunction func)
        {
            App.Current.Dispatcher.Invoke(func);
        }
    }
}
