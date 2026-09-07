using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TrackOMatic.Converters;

[ValueConversion(typeof(int), typeof(GridLength))]
[ValueConversion(typeof(double), typeof(GridLength))]
public class CountToRelativeWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count && count >= 0)
        {
            return new GridLength(count, GridUnitType.Star);
        }
        if (value is double dcount && dcount >= 0)
        {
            return new GridLength(dcount, GridUnitType.Star);
        }
        if (value is bool bcount && bcount)
        {
            return new GridLength(1, GridUnitType.Star);
        }
        return new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
