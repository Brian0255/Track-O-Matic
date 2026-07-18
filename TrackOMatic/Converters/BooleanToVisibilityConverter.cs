using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TrackOMatic.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : FalseVisibility;
        }
        return FalseVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}
