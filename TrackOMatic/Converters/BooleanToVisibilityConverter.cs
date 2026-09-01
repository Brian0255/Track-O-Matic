using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TrackOMatic.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BooleanToVisibilityConverter : IValueConverter
{
    public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return FalseVisibility;
        }

        var result = boolValue;
        var shouldInvert = parameter is string s && s.Equals("invert", StringComparison.InvariantCultureIgnoreCase);
        if (shouldInvert)
        {
            result = !result;
        }

        return result ? Visibility.Visible : FalseVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
