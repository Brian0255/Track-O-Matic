using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class HelmDoorPanel : UserControl
{
    public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(HelmDoorPanel));
    public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Color), typeof(HelmDoorPanel));

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

    public HelmDoorPanel()
    {
        InitializeComponent();

        var barrierService = ServiceLocator.GetService<IBarrierService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
        var itemSelectorService = ServiceLocator.GetService<IItemSelectorService>();

        DataContext = new HelmDoorPanelViewModel(
            barrierService,
            savedProgressProvider,
            itemSelectorService
        );

        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
    }
}
