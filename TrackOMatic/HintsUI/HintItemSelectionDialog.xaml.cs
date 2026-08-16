using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;

namespace TrackOMatic.HintsUI;

/// <summary>
/// Interaction logic for HintItemSelectionDialog.xaml
/// </summary>
public partial class HintItemSelectionDialog : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public IUserSettingsService UserSettings;

    /// <summary>
    /// Proxy property for XAML binding to TopMost setting.
    /// </summary>
    public bool TopMostSetting
    {
        get => UserSettings.TopMost;
        set
        {
            if (UserSettings.TopMost != value)
            {
                UserSettings.TopMost = value;
                OnPropertyChanged(nameof(TopMostSetting));
            }
        }
    }

    public Dictionary<ItemName, bool> SelectedItems { get; private set; } = new();
    public HintItemSelectionDialog(List<ItemName> itemsToTurnOn)
    {
        // Get the injected user settings service via ServiceLocator
        UserSettings = ServiceLocator.GetService<IUserSettingsService>();
        InitializeComponent();
        UserSettings.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IUserSettingsService.TopMost))
            {
                OnPropertyChanged(nameof(TopMostSetting));
            }
        };
        foreach (var child in ItemGrid.Children)
        {
            if (child is SelectableHintItem hintItem)
            {
                hintItem.MouseDoubleClick += TextBlock_MouseDown;
                ItemName itemName = (ItemName)hintItem.Tag;
                if (itemsToTurnOn.Contains(itemName))
                {
                    hintItem.TurnOn();
                }
                else
                {
                    hintItem.Reset();
                }
            }
        }
    }

    private void ProcessItems()
    {
        SelectedItems = new();
        foreach (var child in ItemGrid.Children)
        {
            if (child is SelectableHintItem hintItem && hintItem.On)
            {
                SelectedItems.Add((ItemName)hintItem.Tag, false);
            }
        }
    }

    private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            ProcessItems();
            Close();
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        ProcessItems();
    }

    private void ItemGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ProcessItems();
            Close();
        }
    }
}
