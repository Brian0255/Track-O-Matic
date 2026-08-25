using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using TrackOMatic.Logic.Enums;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class HintPanel : UserControl
{
    public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(HintPanel));
    public static readonly DependencyProperty HintPanelTypeProperty = DependencyProperty.Register(nameof(HintPanelType), typeof(HintPanelType), typeof(HintPanel));

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

        var dialog = new HintItemSelectionDialog(viewModel.ItemsToFilterBy.ToList())
        {
            Title = "Select Items to Prioritize"
        };
        var mousePosition = Mouse.GetPosition(this);
        mousePosition = PointToScreen(mousePosition);
        UIUtils.MoveWindowAndEnsureVisible(dialog, mousePosition.X - 20, mousePosition.Y - 10);
        dialog.ShowDialog();

        // Pass selected items to ViewModel command
        if (viewModel.ApplyFilterCommand.CanExecute(dialog.SelectedItems.Keys.ToList()))
        {
            viewModel.ApplyFilterCommand.Execute(dialog.SelectedItems.Keys.ToList());
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
