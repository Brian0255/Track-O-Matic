using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for HintMultiSelectionDialog.xaml
/// </summary>
public partial class HintMultiSelectionDialog : Window
{
    private readonly MultiGridItemSelectorViewModel _viewModel;
    private readonly Dictionary<ItemName, HintDialogItemViewModel> _itemViewModelsByName;

    public HintMultiSelectionDialog(IReadOnlyList<HintGridCell> items, IList<ItemName> itemsFoundAlready, string title)
    {
        if (items is null || items.Count == 0)
        {
            throw new ArgumentException("Items collection cannot be null or empty.");
        }

        // create the view models from the items passed in.
        var dialogItems = new List<HintDialogItemViewModel>();
        _itemViewModelsByName = new Dictionary<ItemName, HintDialogItemViewModel>();

        foreach (var cell in items)
        {
            var isSelected = itemsFoundAlready?.Contains(cell.ItemName) ?? false;
            var viewModel = new HintDialogItemViewModel(cell, isSelected);
            dialogItems.Add(viewModel);
            _itemViewModelsByName[cell.ItemName] = viewModel;
        }

        _viewModel = new MultiGridItemSelectorViewModel(dialogItems);
        _viewModel.CloseRequested += (s, e) => Close();

        InitializeComponent();

        // Wire each hard-coded HintDialogItem to its corresponding view model
        WireHardCodedItems();

        DataContext = _viewModel;
        Title = title;
    }

    private void WireHardCodedItems()
    {
        // Find the root content (StackPanel in this case)
        if (this.Content is FrameworkElement root)
        {
            WireItemsRecursive(root);
        }
    }

    private void WireItemsRecursive(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is HintDialogItem hintItem)
            {
                if (_itemViewModelsByName.TryGetValue(hintItem.ItemName, out var viewModel))
                {
                    hintItem.DataContext = viewModel;
                }
            }
            else
            {
                WireItemsRecursive(child);
            }
        }
    }

    public IList<HintGridCell> GetSelectedItems()
    {
        return _viewModel?.GetSelectedCells() ?? [];
    }

    private void OnDoneClicked(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _viewModel.ConfirmSelectionCommand.CanExecute(null))
        {
            _viewModel.ConfirmSelectionCommand.Execute(null);
            e.Handled = true;
        }
    }
}
