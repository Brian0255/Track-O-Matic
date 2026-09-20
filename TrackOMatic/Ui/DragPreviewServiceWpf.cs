using System.Windows;

using TrackOMatic.ViewModels;

namespace TrackOMatic.Ui;

public class DragPreviewServiceWpf : IDragPreviewService
{
    public void RegisterDragPreview(UiItemViewModel item)
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.RegisterDragPreview(item);
        }
    }

    public void UpdateDragPreview()
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.UpdateDragPreview();
        }
    }

    public void ClearDragPreview()
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.ClearDragPreview();
        }
    }
}
