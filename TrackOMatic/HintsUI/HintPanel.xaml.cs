using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;

namespace TrackOMatic.HintsUI;

public partial class HintPanel : UserControl
{

    public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(HintPanel));
    public static readonly DependencyProperty HintPanelTypeProperty = DependencyProperty.Register(nameof(HintPanelType), typeof(HintPanelType), typeof(HintPanel));

    public List<ItemName> ItemsToFilterBy { get; private set; } = new();

    public Color LineColor
    {
        get { return (Color)GetValue(LineColorProperty); }
        set { SetValue(LineColorProperty, value); }
    }

    public HintPanelType HintPanelType
    {
        get => (HintPanelType)GetValue(HintPanelTypeProperty);
        set => SetValue(HintPanelTypeProperty, value);
    }

    public IUserSettingsService UserSettings { get; init; }

    public HintPanel()
    {
        UserSettings = ServiceLocator.GetService<IUserSettingsService>();
        InitializeComponent();
    }

    private HintInfo CreateNewHint(bool isSavedHint = false)
    {
        var newHint = new HintInfo(HintPanelType, isSavedHint);
        var insertionPoint = HintList.Children.Count - 1;
        HintList.Children.Insert(insertionPoint, newHint);
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

    private void OnPathItemsSelected()
    {
        if (UserSettings.AutoSortPathHints)
        {
            RefreshDisplayedHints();
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
        RefreshDisplayedHints();
        ApplyFilter();
    }
}
