using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DK64PointsTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            (MainWindow as MainWindow).Save();
        }
    }
}
