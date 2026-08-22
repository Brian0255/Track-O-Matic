using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;

namespace TrackOMatic.HintsUI;

public partial class HintPanel : UserControl
{

    public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(nameof(Heading), typeof(string), typeof(HintPanel));
    public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(HintPanel));
    public static readonly DependencyProperty HintTypeProperty = DependencyProperty.Register(nameof(HintType), typeof(HintType), typeof(HintPanel));
    public static readonly DependencyProperty HintPanelTypeProperty = DependencyProperty.Register(nameof(HintPanelType), typeof(HintPanelType), typeof(HintPanel));
    public static readonly DependencyProperty RegionNameProperty = DependencyProperty.Register(nameof(RegionName), typeof(RegionName), typeof(HintPanel));


    public List<ItemName> ItemsToFilterBy { get; private set; } = new();

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

    public HintType HintType
    {
        get { return (HintType)GetValue(HintTypeProperty); }
        set { SetValue(HintTypeProperty, value); }
    }

    public HintPanelType HintPanelType
    {
        get => (HintPanelType)GetValue(HintPanelTypeProperty);
        set => SetValue(HintPanelTypeProperty, value);
    }

    public RegionName RegionName
    {
        get { return (RegionName)GetValue(RegionNameProperty); }
        set { SetValue(RegionNameProperty, value); }
    }

    public IUserSettingsService UserSettings { get; init; }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        var hintSettings = HintTypeSettingsList.SETTINGS[HintType];
        SortButton.Visibility = hintSettings.HintSorterVisibility ? Visibility.Visible : Visibility.Collapsed;
        FilterButton.Visibility = hintSettings.HintSorterVisibility ? Visibility.Visible : Visibility.Collapsed;
    }
    public HintPanel()
    {
        Console.WriteLine(HintType);
        UserSettings = ServiceLocator.GetService<IUserSettingsService>();
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public void Reset()
    {
        foreach (var child in HintList.Children.OfType<HintInfo>().ToList())
        {
            HintList.Children.Remove(child);
        }
        ItemsToFilterBy = new();
        UpdateFilterImage();
    }

    private void RemoveHint(object sender, MouseEventArgs e)
    {
        if (e.RightButton == MouseButtonState.Pressed)
        {
            HintInfo hint = (HintInfo)sender;
            hint.OnRemove();
            HintList.Children.Remove(hint);
        }
    }

    private HintInfo CreateNewHint(bool isSavedHint = false)
    {
        var newHint = new HintInfo(HintType, HintPanelType, Name, isSavedHint, RegionName);
        var insertionPoint = HintList.Children.Count - 1;
        HintList.Children.Insert(insertionPoint, newHint);
        newHint.MouseDown += RemoveHint;
        newHint.ItemsOnPath.OnItemsSelected += OnPathItemsSelected;
        return newHint;
    }

    public void OnAddHint(object sender, MouseEventArgs e)
    {
        CreateNewHint();
    }

    public void AddSavedHint(SavedHint savedHint)
    {
        var newHint = CreateNewHint(true);
        newHint.SetUpFromSavedHint(savedHint);
    }

    public List<SavedHint> GetSavedHints()
    {
        List<SavedHint> hints = new();
        foreach (var hint in HintList.Children)
        {
            if (hint is HintInfo hintInfo)
            {
                hints.Add(hintInfo.SavedHint);
            }
        }
        return hints;
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
    }

    private List<HintInfo> FetchCurrentHintList()
    {
        return HintList.Children.OfType<HintInfo>().ToList();
    }

    private void BuildHintPanelFromHintList(List<HintInfo> hints)
    {
        var addNewHint = HintList.Children[HintList.Children.Count - 1];
        HintList.Children.Clear();
        foreach (var hint in hints)
        {
            HintList.Children.Add(hint);
        }
        HintList.Children.Add(addNewHint);
    }

    private void UpdateFilterImage()
    {
        string resource = ItemsToFilterBy.Count > 0 ? "filter_on" : "filter_empty";
        FilterButton.Source = (ImageSource)FindResource(resource);
    }

    private void ApplyFilter()
    {
        RefreshDisplayedHints();
    }

    /// <summary>
    /// Refreshes displayed hints: sorts by path item count (desc), then partitions by filter.
    /// Hints matching all filter items appear first, non-matching hints appear last.
    /// </summary>
    private void RefreshDisplayedHints()
    {
        var allHints = FetchCurrentHintList();

        // Step 1: Sort by path items count (descending)
        allHints.Sort((hint1, hint2) =>
            hint2.ItemsOnPath.SelectedItems.Count -
            hint1.ItemsOnPath.SelectedItems.Count);

        // Step 2: Partition by filter (matching hints first, rejects second)
        var (matchingHints, rejectedHints) = PartitionByFilter(allHints);

        // Step 3: Combine and rebuild display
        var combined = matchingHints.Concat(rejectedHints).ToList();
        BuildHintPanelFromHintList(combined);
        UpdateFilterImage();
    }

    /// <summary>
    /// Partitions hints into two groups: those matching the filter, and those that don't.
    /// A hint matches if ALL items in ItemsToFilterBy are present in its path items.
    /// </summary>
    private (List<HintInfo>, List<HintInfo>) PartitionByFilter(List<HintInfo> hints)
    {
        var matching = new List<HintInfo>();
        var rejected = new List<HintInfo>();

        foreach (var hint in hints)
        {
            if (ItemsToFilterBy.Count == 0)
            {
                // No filter = all hints visible
                matching.Add(hint);
            }
            else
            {
                var pathItemNames = hint.ItemsOnPath.SelectedItems.Keys.ToHashSet();

                // Check if all filter items are present in path items
                if (ItemsToFilterBy.All(filterItem => pathItemNames.Contains(filterItem)))
                {
                    matching.Add(hint);
                }
                else
                {
                    rejected.Add(hint);
                }
            }
        }

        return (matching, rejected);
    }

    private void Sort()
    {
        RefreshDisplayedHints();
    }

    private void OnPathItemsSelected()
    {
        if (UserSettings.AutoSortPathHints)
        {
            Sort();
        }

        ApplyFilter();
    }

    private void FilterButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var dialog = new HintItemSelectionDialog(ItemsToFilterBy.ToList())
        {
            Title = "Select Items to Prioritize"
        };
        var mousePosition = Mouse.GetPosition(this);
        mousePosition = PointToScreen(mousePosition);
        UIUtils.MoveWindowAndEnsureVisible(dialog, mousePosition.X - 20, mousePosition.Y - 10);
        dialog.ShowDialog();
        ItemsToFilterBy = dialog.SelectedItems.Keys.ToList();
        ApplyFilter();
    }

    public void SortButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Sort();
        ApplyFilter();
    }
}
