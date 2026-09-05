using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels;

namespace TrackOMatic;
/// <summary>
/// Interaction logic for KongBlueprint.xaml
/// </summary>
public partial class KongBlueprint : UserControl
{
    public static readonly DependencyProperty CollectedTypeProperty = DependencyProperty.Register(
        nameof(CollectedType),
        typeof(ItemType),
        typeof(KongBlueprint),
        new PropertyMetadata(ItemType.MISC, OnBlueprintTypeChanged)
    );

    public static readonly DependencyProperty TurnedInTypeProperty = DependencyProperty.Register(
        nameof(TurnedInType),
        typeof(ItemType),
        typeof(KongBlueprint),
        new PropertyMetadata(ItemType.MISC, OnBlueprintTypeChanged)
    );

    public ItemType CollectedType
    {
        get => (ItemType)GetValue(CollectedTypeProperty);
        set => SetValue(CollectedTypeProperty, value);
    }

    public ItemType TurnedInType
    {
        get => (ItemType)GetValue(TurnedInTypeProperty);
        set => SetValue(TurnedInTypeProperty, value);
    }

    public KongBlueprint()
    {
        InitializeComponent();
    }


    private static void OnBlueprintTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not KongBlueprint control || e.NewValue is not ItemType)
        {
            return;
        }

        SetUpContext(control);
    }

    private static void SetUpContext(KongBlueprint control)
    {
        if (control.CollectedType == ItemType.MISC || control.TurnedInType == ItemType.MISC)
        {
            return;
        }

        // Create ViewModel with injected services
        var collectiblesService = ServiceLocator.GetService<ICollectiblesService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
        var userSettingsService = ServiceLocator.GetService<IUserSettingsService>();
        if (collectiblesService != null && parsedSpoilerDataService != null && savedProgressProvider != null && userSettingsService != null)
        {
            // Dispose the old view model if it exists.
            if (control.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            control.DataContext = new UiKongBlueprintViewModel(
                control.CollectedType,
                control.TurnedInType,
                collectiblesService,
                parsedSpoilerDataService,
                savedProgressProvider,
                userSettingsService
            );
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
        DataContext = null;
    }

    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is UiKongBlueprintViewModel viewModel && viewModel.IncrementCountCommand.CanExecute(null))
        {
            viewModel.IncrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is UiKongBlueprintViewModel viewModel && viewModel.DecrementCountCommand.CanExecute(null))
        {
            viewModel.DecrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not UiKongBlueprintViewModel viewModel)
        {
            return;
        }
        if (e.Delta > 0 && viewModel.IncrementCountCommand.CanExecute(null))
        {
            viewModel.IncrementCountCommand.Execute(null);
        }
        else if (e.Delta < 0 && viewModel.DecrementCountCommand.CanExecute(null))
        {
            viewModel.DecrementCountCommand.Execute(null);
        }
        e.Handled = true;
    }
}
