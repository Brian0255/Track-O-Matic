using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Logic.Enums;
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
    public static readonly DependencyProperty RegionNameProperty =
        DependencyProperty.Register(
        nameof(RegionName),
        typeof(RegionName),
        typeof(UiRegion),
        new PropertyMetadata(RegionName.UNKNOWN, OnRegionNameChanged)
    );

    public RegionName RegionName
    {
        get => (RegionName)GetValue(RegionNameProperty);
        set => SetValue(RegionNameProperty, value);
    }

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

    public bool TryDropItem(ItemName itemName, MouseDragType dragType)
    {
        if (DataContext is not RegionViewModel viewModel)
        {
            return false;
        }

        return viewModel.TryAcceptDrop(itemName, dragType);
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
        if (e.Region == RegionName && DataContext is RegionViewModel viewModel)
        {
            viewModel.SetLighting(e.LightUp);
        }
    }

    private static void OnRegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UiRegion control && e.NewValue is RegionName regionName)
        {
            // Update the DataContext or any other properties based on the new RegionName
            var itemTrackingService = ServiceLocator.GetService<IItemTrackingService>();
            var orchestrator = ServiceLocator.GetService<IRegionPlacementOrchestrator>();
            var themeService = ServiceLocator.GetService<IThemeService>();
            var slotProviderRegistry = ServiceLocator.GetService<RegionSlotProviderRegistry>();

            if (itemTrackingService != null && orchestrator != null && control.ParsedSpoilerDataService != null && control.SavedProgressProvider != null && themeService != null)
            {
                control.DataContext = new RegionViewModel(
                    regionName,
                    itemTrackingService,
                    control.ParsedSpoilerDataService,
                    control.SavedProgressProvider,
                    orchestrator,
                    themeService,
                    slotProviderRegistry);
            }
        }
    }
}
