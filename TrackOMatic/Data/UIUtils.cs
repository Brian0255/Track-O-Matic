using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TrackOMatic
{
    static class UIUtils
    {

        public static void MoveWindowAndEnsureVisibile(Window window, double x, double y)
        {
            var currentScreen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            window.Left = Math.Max(currentScreen.WorkingArea.Left, x);
            if (window.Left + window.Width > currentScreen.WorkingArea.Left + currentScreen.WorkingArea.Width)
            {
                window.Left = currentScreen.WorkingArea.Left + currentScreen.WorkingArea.Width - window.Width;
            }
            window.Top = Math.Max(currentScreen.WorkingArea.Top, y);
            if (window.Top + window.Height > currentScreen.WorkingArea.Top + currentScreen.WorkingArea.Height)
            {
                window.Top = currentScreen.WorkingArea.Top + currentScreen.WorkingArea.Height - window.Height;
            }
        }
    }
}
