using System.Windows.Controls;

using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Regions;
/// <summary>
/// Interaction logic for UiRegion.xaml
/// </summary>
public partial class UiRegion : UserControl
{
    internal IParsedSpoilerDataService ParsedSpoilerDataService { get; private init; }
    internal ISavedProgressProvider SavedProgressProvider { get; private init; }

    internal IAutotrackerService AutotrackerService { get; private init; }

    private readonly SynchronizationContext? _uiContext;

    public UiRegion()
    {
        ParsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>() ?? throw new InvalidOperationException("IParsedSpoilerDataService not found in service locator.");
        SavedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>() ?? throw new InvalidOperationException("ISavedProgressProvider not found in service locator.");
        AutotrackerService = ServiceLocator.GetService<IAutotrackerService>() ?? throw new InvalidOperationException("IAutotrackerService not found in service locator.");

        // Capture the UI synchronization context for marshaling autotracker events back to the UI thread
        _uiContext = SynchronizationContext.Current;

        InitializeComponent();

        ParsedSpoilerDataService.ParsedSpoilerDataChanged += OnSpoilerDataChanged;
        AutotrackerService.RegionLightingChanged += OnAutotrackerRegionLightingChanged;
    }

    private void OnSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e)
    {
        // throw new NotImplementedException();
    }

    private void OnAutotrackerRegionLightingChanged(object? sender, AutotrackerRegionEventArgs e)
    {
        // If we're on a different thread than the UI thread, marshal back to the UI context
        if (_uiContext != null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ =>
            {
                HandleRegionLightingChanged(e);
            }, null);
        }
        else
        {
            // Already on the UI thread
            HandleRegionLightingChanged(e);
        }
    }

    private void HandleRegionLightingChanged(AutotrackerRegionEventArgs e)
    {
        if (DataContext is not RegionViewModel viewModel)
        {
            return;
        }
        viewModel.SetLighting(e.Region == viewModel.RegionName);
    }
}
