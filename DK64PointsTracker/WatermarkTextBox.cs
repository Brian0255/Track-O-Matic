using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DK64PointsTracker
{
    public class WatermarkTextBox : TextBox
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox));

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public WatermarkTextBox()
        {
            GotFocus += WatermarkTextBox_GotFocus;
            LostKeyboardFocus += WatermarkTextBox_LostKeyboardFocus;
            ApplyWatermark();
        }

        private void WatermarkTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearWatermark();
        }

        public void Reset()
        {
            if(IsFocused) FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
            ApplyWatermark();
        }

        private void WatermarkTextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void ApplyWatermark()
        {
            if (string.IsNullOrEmpty(Text))
            {
                Text = Watermark;
                Foreground = (SolidColorBrush)FindResource("UnfocusedText");
            }
        }

        private void ClearWatermark()
        {
            if (Text == Watermark)
            {
                Text = string.Empty;
                Foreground = Brushes.White;
            }
        }
    }
}
