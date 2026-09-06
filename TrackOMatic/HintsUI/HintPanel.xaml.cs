using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Ui;
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

        if (e.LeftButton == MouseButtonState.Pressed && viewModel.CreateHintCommand.CanExecute(null))
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

        // Position dialog at cursor with small offset
        WindowPositioning.PositionDialogAtCursor(dialog, -20, -10);
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
