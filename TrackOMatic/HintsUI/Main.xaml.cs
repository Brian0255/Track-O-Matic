using System.Windows.Controls;
using System.ComponentModel;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for Main.xaml
/// </summary>
public partial class Main : UserControl
{
    private MainHintViewModel? _currentViewModel;

    public Main()
    {
        InitializeComponent();

        _currentViewModel = new(
            ServiceLocator.GetService<IHintService>(),
            ServiceLocator.GetService<IShortcutService>(),
            ServiceLocator.GetService<IHintItemSelectionService>(),
            ServiceLocator.GetService<IItemTrackingService>(),
            ServiceLocator.GetService<IUserSettingsService>(),
            ServiceLocator.GetService<ISavedProgressProvider>()
        );
        DataContext = _currentViewModel;

        // Subscribe to mode changes to force ContentTemplateSelector re-evaluation
        _currentViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainHintViewModel.CurrentModeKey) && sender is MainHintViewModel vm)
        {
            // Force ContentPresenter to re-evaluate the template by clearing and re-setting Content
            // This works because WPF only calls SelectTemplate when detecting a content change
            ModeContentPresenter.Content = null;
            ModeContentPresenter.Content = vm;
        }
    }
}
