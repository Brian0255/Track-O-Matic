using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TrackOMatic.Converters;

[ValueConversion(typeof(int), typeof(GridLength))]
public class CountToRelativeWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count && count >= 0)
        {
            return new GridLength(count, GridUnitType.Star);
        }
        return new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
