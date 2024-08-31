using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace TrackOMatic
{
    static class UIUtils
    {
        public static float GetDpiScale()
        {
            Form form = new Form();
            Graphics g = form.CreateGraphics();
            float defaultDpi = 96.0f;
            float dpi = defaultDpi; // default DPI is 96

            try
            {
                dpi = g.DpiX;
            }
            finally
            {
                g.Dispose();
            }

            form.Dispose();
            return dpi / defaultDpi;
        }

        public static void MoveWindowAndEnsureVisibile(Window window, double x, double y)
        {
            float dpiScale = GetDpiScale();
            var currentScreen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            double windowScaledWidth = window.Width * dpiScale;
            double windowScaledHeight = window.Height * dpiScale;
            window.Left = Math.Max(currentScreen.WorkingArea.Left, x) / dpiScale;
            Console.WriteLine("mouse coords (" + x + ", " + y + ")");
            if (window.Left + window.Width > (currentScreen.WorkingArea.Left + currentScreen.WorkingArea.Width) / dpiScale)
            {
                window.Left = (currentScreen.WorkingArea.Left + currentScreen.WorkingArea.Width - windowScaledWidth) / dpiScale;
            }
            window.Top = Math.Max(currentScreen.WorkingArea.Top, y) / dpiScale;
            if (window.Top + window.Height > (currentScreen.WorkingArea.Top + currentScreen.WorkingArea.Height) / dpiScale)
            {
                window.Top = (currentScreen.WorkingArea.Top + currentScreen.WorkingArea.Height - windowScaledHeight) / dpiScale;
            }
            Console.WriteLine("Window Pos: (" + window.Left + ", " + window.Top + ")");
            Console.WriteLine("Window Size: (" + window.Width + ", " + window.Height + ")");
        }
    }
}
