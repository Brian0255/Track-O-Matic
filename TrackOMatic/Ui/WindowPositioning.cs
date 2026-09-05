using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

using Point = System.Drawing.Point;

namespace TrackOMatic.Ui;

/// <summary>
/// Minimal window positioning utility for single-monitor dialogs.
/// Handles DPI-aware cursor-relative dialog placement with basic bounds checking.
/// Designed for simplicity and future Avalonia compatibility.
/// </summary>
static class WindowPositioning
{
    /// <summary>
    /// Gets the DPI scale factor for the given visual element.
    /// DPI scaling represents the ratio of current DPI to the standard 96 DPI baseline.
    /// </summary>
    /// <param name="visual">The visual element used to determine the DPI context. Cannot be null.</param>
    /// <returns>The DPI scale factor (1.0 = 96 DPI, 2.0 = 192 DPI, etc.)</returns>
    /// <remarks>This method may need refactoring for cross-platform support when we get there.</remarks>
    private static double GetDpiScale(Visual visual)
    {
        ArgumentNullException.ThrowIfNull(visual);

        var presentationSource = PresentationSource.FromVisual(visual);
        if (presentationSource?.CompositionTarget != null)
        {
            // M11 represents the X-axis scale factor in the transformation matrix
            return presentationSource.CompositionTarget.TransformToDevice.M11;
        }

        // Fallback to default if visual tree is not yet initialized
        return 1.0;
    }

    /// <summary>
    /// Gets the screen containing the given point, or falls back to the primary screen.
    /// PrimaryScreen is guaranteed to exist on Windows, so the null-forgiving operator is safe.
    /// </summary>
    private static Screen GetScreenOrPrimary(Point point)
    {
        return Screen.FromPoint(point) ?? Screen.PrimaryScreen!;
    }

    /// <summary>
    /// Constrains a position to stay within the given screen's working area.
    /// Assumes all coordinates are in logical pixels for the given DPI scale.
    /// </summary>
    private static void ClampToScreen(
        Screen screen,
        double dpiScale,
        Window dialog,
        ref double x,
        ref double y)
    {
        var screenBounds = screen.WorkingArea;
        double screenLeft = screenBounds.Left / dpiScale;
        double screenTop = screenBounds.Top / dpiScale;
        double screenRight = screenBounds.Right / dpiScale;
        double screenBottom = screenBounds.Bottom / dpiScale;

        // Clamp horizontal position
        if (x < screenLeft)
        {
            x = screenLeft;
        }
        else if (x + dialog.Width > screenRight)
        {
            x = screenRight - dialog.Width;
        }

        // Clamp vertical position
        if (y < screenTop)
        {
            y = screenTop;
        }
        else if (y + dialog.Height > screenBottom)
        {
            y = screenBottom - dialog.Height;
        }
    }

    /// <summary>
    /// Positions a dialog window at the cursor location with bounds checking.
    /// Detects which screen contains the cursor and positions relative to that screen.
    /// </summary>
    /// <param name="dialog">The window to position.</param>
    /// <param name="offsetX">Horizontal offset from cursor position (default: -20).</param>
    /// <param name="offsetY">Vertical offset from cursor position (default: -10).</param>
    /// <remarks>
    /// This defers positioning until SourceInitialized to ensure DPI information is available.
    /// It detects which monitor the cursor is on and constrains the dialog to that screen.
    /// If the dialog would go off-screen, it's repositioned to stay visible.
    /// </remarks>
    public static void PositionDialogAtCursor(Window dialog, double offsetX = -20, double offsetY = -10)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        // Defer positioning until window is initialized (DPI available)
        dialog.WindowStartupLocation = WindowStartupLocation.Manual;

        dialog.SourceInitialized += (s, e) =>
        {
            // Get cursor position (in physical pixels from Windows API)
            var cursorPhysical = Cursor.Position;

            // Detect which screen contains the cursor
            var screenForCursor = GetScreenOrPrimary(cursorPhysical);

            // Get DPI scale for this window
            double dpiScale = GetDpiScale(dialog);

            // Convert cursor physical coordinates to WPF logical coordinates
            double x = (cursorPhysical.X + offsetX) / dpiScale;
            double y = (cursorPhysical.Y + offsetY) / dpiScale;

            // Clamp to screen boundaries
            ClampToScreen(screenForCursor, dpiScale, dialog, ref x, ref y);

            // Apply position
            dialog.Left = x;
            dialog.Top = y;
        };
    }

    /// <summary>
    /// Centers a dialog relative to a point with bounds checking.
    /// Detects which screen contains the center point and constrains the dialog to that screen.
    /// </summary>
    /// <param name="dialog">The window to position.</param>
    /// <param name="centerX">X coordinate to center on (logical pixels).</param>
    /// <param name="centerY">Y coordinate to center on (logical pixels).</param>
    /// <remarks>
    /// This is useful for positioning a dialog relative to a known point (e.g., center of main window).
    /// It defers positioning until SourceInitialized to ensure window dimensions are available.
    /// The dialog is constrained to stay on the screen containing the center point.
    /// </remarks>
    public static void PositionDialogCenteredOn(Window dialog, double centerX, double centerY)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        // Defer positioning until window is initialized and has dimensions
        dialog.WindowStartupLocation = WindowStartupLocation.Manual;

        dialog.SourceInitialized += (s, e) =>
        {
            // Get DPI scale for this window
            double dpiScale = GetDpiScale(dialog);

            // Get the screen containing the center point
            // Convert from logical to physical coordinates to use Screen.FromPoint
            int centerPhysicalX = (int)(centerX * dpiScale);
            int centerPhysicalY = (int)(centerY * dpiScale);
            var screenForCenter = GetScreenOrPrimary(new Point(centerPhysicalX, centerPhysicalY));

            // Center the dialog on the given point
            double x = centerX - (dialog.ActualWidth / 2);
            double y = centerY - (dialog.ActualHeight / 2);

            // Clamp to screen boundaries
            ClampToScreen(screenForCenter, dpiScale, dialog, ref x, ref y);

            // Apply position
            dialog.Left = x;
            dialog.Top = y;
        };
    }
}
