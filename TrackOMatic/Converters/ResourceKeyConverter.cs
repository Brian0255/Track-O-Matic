using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TrackOMatic.Converters;

/// <summary>
/// Converts a string resource key to the actual resource object from the resource dictionary.
/// Searches the entire resource hierarchy (control, application, etc.) to find the resource.
/// </summary>
public class ResourceKeyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string resourceKey || string.IsNullOrEmpty(resourceKey))
        {
            return DependencyProperty.UnsetValue;
        }

        // Try to find the resource in the application dictionary first
        if (Application.Current?.Resources.Contains(resourceKey) == true)
        {
            return Application.Current.Resources[resourceKey];
        }

        // If not found in application resources, return UnsetValue so WPF can fall back gracefully
        // WPF will continue searching in parent resources/dictionary hierarchies
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
