using System.Globalization;
using System.Windows.Data;

namespace TrackOMatic.Converters;

/// <summary>
/// A converter to compare an enum value with a parameter and return a boolean indicating if they are equal.
/// </summary>
[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
        {
            return false;
        }

        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? parameter : Binding.DoNothing;
    }
}
