using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class BLockerPanel : UserControl
{
    public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(nameof(Heading), typeof(string), typeof(BLockerPanel));
    public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(BLockerPanel));
    public string Heading
    {
        get { return (string)GetValue(HeadingProperty); }
        set { SetValue(HeadingProperty, value); }
    }

    public Color LineColor
    {
        get { return (Color)GetValue(LineColorProperty); }
        set { SetValue(LineColorProperty, value); }
    }

    public BLockerPanel()
    {
        InitializeComponent();

        var barrierService = ServiceLocator.GetService<IBarrierService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
        var itemSelectorService = ServiceLocator.GetService<IItemSelectorService>();

        DataContext = new BLockerPanelViewModel(
            barrierService,
            parsedSpoilerDataService,
            savedProgressProvider,
            itemSelectorService
        );

        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("BLockerPanel unloaded, verify if this is the right time.");
        (DataContext as IDisposable)?.Dispose();
    }
}
