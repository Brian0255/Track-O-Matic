using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TrackOMatic.Converters;

[ValueConversion(typeof(int), typeof(Visibility))]
public class CountToVisibilityConverter : IValueConverter
{
    public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            return count > 0 ? Visibility.Visible : FalseVisibility;
        }

        ArgumentNullException.ThrowIfNull(culture);
        return FalseVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
