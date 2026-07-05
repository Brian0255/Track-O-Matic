using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TrackOMatic
{
    static class UIUtils
    {
        public static void AddToGridRow(Grid grid, UIElement element, int row)
        {
            grid.Children.Add(element);
            Grid.SetRow(element, row);
        }

        /// <summary>
        /// Gets the DPI scale factor for the given visual element.
        /// DPI scaling represents the ratio of current DPI to the standard 96 DPI baseline.
        /// </summary>
        /// <param name="visual">The visual element used to determine the DPI context. Cannot be null.</param>
        /// <returns>The DPI scale factor (1.0 = 96 DPI, 2.0 = 192 DPI, etc.)</returns>
        /// <remarks>This method needs refactoring for Linux/cross platform support when we get there.</remarks>
        public static double GetDpiScale(Visual visual)
        {
            if (visual == null)
            {
                throw new ArgumentNullException(nameof(visual));
            }

            var presentationSource = PresentationSource.FromVisual(visual);
            if (presentationSource?.CompositionTarget != null)
            {
                // M11 represents the X-axis scale factor in the transformation matrix
                return presentationSource.CompositionTarget.TransformToDevice.M11;
            }

            // Fallback to default if visual tree is not yet initialized
            return 1.0;
        }

        public static void MoveWindowAndEnsureVisibile(Window window, double x, double y)
        {
            // If the window hasn't been initialized yet (no PresentationSource), defer positioning
            // until SourceInitialized fires. This ensures GetDpiScale() has accurate DPI information.
            if (PresentationSource.FromVisual(window) == null)
            {
                void handler(object? s, EventArgs e)
                {
                    window.SourceInitialized -= (EventHandler)handler;
                    PositionWindowOnScreen(window, x, y);
                }

                window.SourceInitialized += (EventHandler)handler;
            }
            else
            {
                // Window is already initialized, position immediately
                PositionWindowOnScreen(window, x, y);
            }
        }

        private static void PositionWindowOnScreen(Window window, double x, double y)
        {
            double dpiScale = GetDpiScale(window);
            var currentScreen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            double windowScaledWidth = window.Width * dpiScale;
            double windowScaledHeight = window.Height * dpiScale;
            window.Left = Math.Max(currentScreen.WorkingArea.Left, x) / dpiScale;
            if (window.Left + window.Width > (currentScreen.WorkingArea.Left + currentScreen.WorkingArea.Width) / dpiScale)
            {
                window.Left = (currentScreen.WorkingArea.Left + currentScreen.WorkingArea.Width - windowScaledWidth) / dpiScale;
            }
            window.Top = Math.Max(currentScreen.WorkingArea.Top, y) / dpiScale;
            if (window.Top + window.Height > (currentScreen.WorkingArea.Top + currentScreen.WorkingArea.Height) / dpiScale)
            {
                window.Top = (currentScreen.WorkingArea.Top + currentScreen.WorkingArea.Height - windowScaledHeight) / dpiScale;
            }
        }
    }
}
