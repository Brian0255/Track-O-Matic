using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class HintPanel : UserControl
{
    public HintPanel()
    {
        InitializeComponent();
    }

    public void OnAddHint(object sender, MouseEventArgs e)
    {
        if (DataContext is not HintPanelViewModel viewModel)
        {
            return;
        }

        if (viewModel.CreateHintCommand.CanExecute(null))
        {
            viewModel.CreateHintCommand.Execute(null);
        }
    }

    private void FilterButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not HintPanelViewModel viewModel)
        {
            return;
        }

        var dialog = new HintMultiSelectionDialog(HintDialogMapping.HintDialogMappings, [.. viewModel.ItemsToFilterBy], "Select Items to Prioritize");

        var mousePosition = Mouse.GetPosition(this);
        mousePosition = PointToScreen(mousePosition);
        UIUtils.MoveWindowAndEnsureVisible(dialog, mousePosition.X - 20, mousePosition.Y - 10);
        dialog.ShowDialog();
        var selectedItems = dialog.GetSelectedItems().Select(x => x.ItemName).ToList();

        // Pass selected items to ViewModel command
        if (viewModel.ApplyFilterCommand.CanExecute(selectedItems))
        {
            viewModel.ApplyFilterCommand.Execute(selectedItems);
        }
    }

    public void SortButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not HintPanelViewModel viewModel)
        {
            return;
        }

        if (viewModel.SortHintsCommand.CanExecute(null))
        {
            viewModel.SortHintsCommand.Execute(null);
        }
    }
}
