using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TrackOMatic.Properties;

namespace TrackOMatic
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
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UpdatePadBarrelImages();
        }

        public void UpdatePadBarrelImages()
        {
            var dicts = Resources.MergedDictionaries;
            dicts.Clear();
            dicts.Add(new ResourceDictionary
            {
                Source = new Uri("Dictionary1.xaml", UriKind.Relative)
            });
            var path = Settings.Default.ColoredBarrelPadMoves ? "ColoredBarrelPadImages.xaml" : "BaseBarrelPadImages.xaml";
            dicts.Add(new ResourceDictionary
            {
                Source = new Uri(path, UriKind.Relative)
            });
        }
    }
}
