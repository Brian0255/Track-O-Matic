using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TrackOMatic
{
    public class ItemCountBox : TextBox
    {


        public ItemCountBox()
        {
            PreviewMouseDown += ItemCount_MouseDown;
            PreviewKeyDown += ItemCount_PreviewKeyDown;
            LostFocus += ItemCount_LostFocus;
        }


        private void ItemCount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Text == "?")
            {
                Text = "";
            }
        }

        private void CheckSave()
        {
            if (Text != "?")
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.DataSaver.Save();
            }
        }

        private void ItemCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                CheckSave();
            }
        }

        private void ItemCount_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckSave();
        }
    }
}
