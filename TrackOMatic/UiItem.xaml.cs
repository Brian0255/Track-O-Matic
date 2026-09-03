using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Regions;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic;
/// <summary>
/// Interaction logic for UiItem.xaml
/// </summary>
public partial class UiItem : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty ItemNameProperty = DependencyProperty.Register(
        nameof(ItemName),
        typeof(ItemName),
        typeof(UiItem),
        new PropertyMetadata(ItemName.BASIC_KEY, OnItemNameChanged));

    public ItemName ItemName
    {
        get => (ItemName)GetValue(ItemNameProperty);
        set => SetValue(ItemNameProperty, value);
    }

    public static readonly DependencyProperty HoverTextProperty = DependencyProperty.Register(
        nameof(HoverText),
        typeof(string),
        typeof(UiItem)
    );

    public string HoverText
    {
        get => (string)GetValue(HoverTextProperty);
        set => SetValue(HoverTextProperty, value);
    }

    private bool PressedLeft { get; set; } = false;

    private bool PressedRight { get; set; } = false;

    private Point _startPoint = new();

    internal IItemTrackingService ItemTrackingService { get; private init; }
    internal IParsedSpoilerDataService ParsedSpoilerDataService { get; private init; }
    internal ISavedProgressProvider SavedProgressProvider { get; private init; }
    internal IThemeService ThemeService { get; private init; }

    public UiItem()
    {
        ItemTrackingService = ServiceLocator.GetService<IItemTrackingService>() ?? throw new InvalidOperationException("IItemTrackingService not found in service locator.");
        ParsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>() ?? throw new InvalidOperationException("IParsedSpoilerDataService not found in service locator.");
        SavedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>() ?? throw new InvalidOperationException("ISavedProgressProvider not found in service locator.");
        ThemeService = ServiceLocator.GetService<IThemeService>() ?? throw new InvalidOperationException("IThemeService not found in service locator.");
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static void OnItemNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UiItem control && e.NewValue is ItemName itemName)
        {
            control.DataContext = new UiItemViewModel(
                itemName,
                control.ItemTrackingService,
                control.ParsedSpoilerDataService,
                control.SavedProgressProvider,
                control.ThemeService
            );
        }
    }

    private void ToggleStar()
    {
        if (DataContext is UiItemViewModel viewModel)
        {
            viewModel.ToggleStar();
        }
    }

    private void CheckMiddleClick(object sender, MouseEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            ToggleStar();
        }
    }

    private void UiItem_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta != 0)
        {
            ToggleStar();
            e.Handled = true;
        }
    }

    private void UiItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (DataContext is not UiItemViewModel viewModel)
        {
            return;
        }

        var dragService = ServiceLocator.GetService<IDragPreviewService>();
        var currentPoint = e.GetPosition(this);
        var control = (UIElement)sender;
        double width = control.RenderSize.Width;
        double height = control.RenderSize.Height;

        if (PressedLeft && !viewModel.IsDragging)
        {
            if (IsPastThreshold(_startPoint, currentPoint))
            {
                PressedLeft = false;
                Mouse.Capture(control);
                viewModel.BeginDrag(MouseDragType.Left, width, height);
                dragService.RegisterDragPreview(viewModel);
            }
        }
        else if (PressedRight && !viewModel.IsDragging)
        {
            if (IsPastThreshold(_startPoint, currentPoint))
            {
                PressedRight = false;
                Mouse.Capture(control);
                viewModel.BeginDrag(MouseDragType.Right, width, height);
                dragService.RegisterDragPreview(viewModel);
            }
        }

        if (viewModel.IsDragging)
        {
            // Convert local coordinates to main window coordinates
            Window? mainWindow = Window.GetWindow(this);
            if (mainWindow != null)
            {
                Point windowPoint = TransformToAncestor(mainWindow).Transform(currentPoint);
                viewModel.DragX = windowPoint.X;
                viewModel.DragY = windowPoint.Y;

                // Update the drag preview canvas
                dragService.UpdateDragPreview();
            }
            else
            {
                // Fallback to local coordinates if main window is not available
                viewModel.DragX = currentPoint.X + 5;
                viewModel.DragY = currentPoint.Y + 5;
            }
        }
    }

    private static bool IsPastThreshold(Point startPoint, Point currentPoint)
    {
        Vector diff = startPoint - currentPoint;
        return (
            Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance
        );
    }

    private void UiItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(this);

        if (e.ChangedButton == MouseButton.Left)
        {
            PressedLeft = true;
            PressedRight = false;
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            PressedRight = true;
            PressedLeft = false;
        }
        else if (e.ChangedButton == MouseButton.Middle)
        {
            ToggleStar();
            e.Handled = true;
        }
    }

    private void UiItem_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not UiItemViewModel viewModel)
        {
            return;
        }

        if (viewModel.IsDragging)
        {
            Mouse.Capture(null);

            PressedLeft = false;
            PressedRight = false;

            DependencyObject current = this;
            DependencyObject root = this;

            while (current is not null)
            {
                root = current;
                current = VisualTreeHelper.GetParent(current);
            }

            if (root is UIElement rootVisualContainer)
            {
                Point dropPoint = e.GetPosition(rootVisualContainer);
                ExecuteDrop(rootVisualContainer, dropPoint);
            }
            else
            {
                viewModel.EndDrag();
                ServiceLocator.GetService<IDragPreviewService>().ClearDragPreview();
            }

            return;
        }

        if (e.ChangedButton == MouseButton.Right)
        {
            PressedRight = false;
        }
        if (e.ChangedButton == MouseButton.Left)
        {
            if (PressedLeft)
            {
                PressedLeft = false;

                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    viewModel.ToggleStar();
                }
                else
                {
                    // Maybe consider autotrack protections here.
                    viewModel.RemoveFromRegion();
                }
            }
        }
    }

    private void ExecuteDrop(UIElement rootVisualContainer, Point dropPoint)
    {
        if (DataContext is not UiItemViewModel viewModel)
        {
            return;
        }

        viewModel.IsDragging = false;

        UiRegion? foundUiRegion = UIUtils.HitTestAt<UiRegion>(rootVisualContainer, dropPoint);
        if (foundUiRegion?.DataContext is not RegionViewModel regionViewModel)
        {
            viewModel.EndDrag();
        }
        else
        {
            // Pass the RegionViewModel directly
            bool canDrop = ServiceLocator.GetService<IDragDropCoordinator>().TryExecuteDrop(
                ItemName,
                regionViewModel,
                viewModel.MouseDragType
            );

            if (canDrop)
            {
                viewModel.CompleteDrag(regionViewModel.RegionName);
            }
            else
            {
                viewModel.EndDrag();
            }
        }

        ServiceLocator.GetService<IDragPreviewService>().ClearDragPreview();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }
}
