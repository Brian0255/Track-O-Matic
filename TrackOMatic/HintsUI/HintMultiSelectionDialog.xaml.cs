using System.Windows;
using System.Windows.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for HintMultiSelectionDialog.xaml
/// </summary>
public partial class HintMultiSelectionDialog : Window
{
    private readonly MultiGridItemSelectorViewModel _viewModel;
    public HintMultiSelectionDialog(IReadOnlyList<HintGridCell> items, IList<ItemName> itemsFoundAlready, string title)
    {
        if (items is null || items.Count == 0)
        {
            throw new ArgumentException("Items collection cannot be null or empty.");
        }

        // create the view models from the items passed in.
        var dialogItems = new List<HintDialogItemViewModel>();
        foreach (var cell in items)
        {
            var isSelected = itemsFoundAlready?.Contains(cell.ItemName) ?? false;
            dialogItems.Add(new HintDialogItemViewModel(cell, isSelected));
        }

        _viewModel = new MultiGridItemSelectorViewModel(dialogItems);
        _viewModel.CloseRequested += (s, e) => Close();

        InitializeComponent();

        DataContext = _viewModel;
        Title = title;
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
