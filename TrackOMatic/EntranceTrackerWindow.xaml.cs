using System.Windows;

namespace TrackOMatic
{

    /// Interaction logic for EntranceTrackerWindow.xaml
    public partial class EntranceTrackerWindow : Window
    {
        public EntranceTrackerWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Only prevent closing if the main window is still open
            if (Application.Current.MainWindow != null && !Application.Current.MainWindow.IsLoaded)
            {
                // Main window is closing, allow this window to close too
                base.OnClosing(e);
                return;
            }

            // Hide instead of close so the window can be reopened
            e.Cancel = true;
            Hide();
            
            // Uncheck the menu item
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                TrackOMatic.Properties.Settings.Default.EntranceTracker = false;
                TrackOMatic.Properties.Settings.Default.Save();
            }
        }
    }
}
