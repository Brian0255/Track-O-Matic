using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;
/// <summary>
/// Interaction logic for Song.xaml
/// </summary>
public partial class Song : UserControl
{
    public Song()
    {
        InitializeComponent();

        var songService = ServiceLocator.GetService<ISongService>();
        DataContext = new SongViewModel(songService);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }
}
