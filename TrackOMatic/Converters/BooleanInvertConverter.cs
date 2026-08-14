using System.Globalization;
using System.Windows.Data;

namespace TrackOMatic.Converters;

[ValueConversion(typeof(bool), typeof(bool))]
public class BooleanInvertConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value);

    private bool Convert(object value)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}
