using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;

namespace TrackOMatic.ViewModels.Hints;

public class MainHintViewModel : ObservableObject
{
    private IUserSettingsService _userSettingsService;

    #region Proxy Properties

    // Eventually have properties for each panel's view model here as well.

    public bool DisplayNoHints => _userSettingsService.HintDisplay == HintDisplayMode.Off;

    public bool DisplayDirectHints => _userSettingsService.HintDisplay == HintDisplayMode.DirectItemHints;

    public bool DisplayPathHints => _userSettingsService.HintDisplay == HintDisplayMode.MultipathHints;

    public bool IsHelmDoorPanelEnabled => _userSettingsService.HelmDoors;

    public bool IsMultipathCompactMode => _userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.MultipathHints;

    public bool IsDirectHintCompactMode => _userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.DirectItemHints;

    public bool IsMultipathStandardMode => !_userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.MultipathHints;

    public bool IsDirectHintStandardMode => !_userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.DirectItemHints;

    #endregion

    public MainHintViewModel(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));

        _userSettingsService.PropertyChanged += OnSettingChanged;
    }

    private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IUserSettingsService.HintDisplay))
        {
            OnPropertyChanged(nameof(DisplayNoHints));
            OnPropertyChanged(nameof(DisplayDirectHints));
            OnPropertyChanged(nameof(DisplayPathHints));

            // Do other things with hint display.
            OnPropertyChanged(nameof(IsDirectHintCompactMode));
            OnPropertyChanged(nameof(IsDirectHintStandardMode));
            OnPropertyChanged(nameof(IsMultipathCompactMode));
            OnPropertyChanged(nameof(IsMultipathStandardMode));
        }
        else if (e.PropertyName == nameof(IUserSettingsService.CompactMode))
        {
            // Do something with compact mode.
            OnPropertyChanged(nameof(IsDirectHintCompactMode));
            OnPropertyChanged(nameof(IsDirectHintStandardMode));
            OnPropertyChanged(nameof(IsMultipathCompactMode));
            OnPropertyChanged(nameof(IsMultipathStandardMode));
        }
        else if (e.PropertyName == nameof(IUserSettingsService.HelmDoors))
        {
            OnPropertyChanged(nameof(IsHelmDoorPanelEnabled));
        }
    }

    #region IDisposable

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unsubscribe from events
                _userSettingsService.PropertyChanged -= OnSettingChanged;
            }
            _disposed = true;
        }
    }

    ~MainHintViewModel()
    {
        Dispose(false);
    }

    #endregion
}
