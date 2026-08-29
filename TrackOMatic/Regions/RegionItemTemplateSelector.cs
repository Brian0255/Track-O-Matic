using System.Windows;
using System.Windows.Controls;

using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Regions;

public class RegionItemTemplateSelector : DataTemplateSelector
{
    public required DataTemplate VialItemTemplate { get; init; }
    public required DataTemplate SimpleItemTemplate { get; init; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is VialItemViewModel)
        {
            return VialItemTemplate;
        }
        if (item is RegionItemViewModel)
        {
            return SimpleItemTemplate;
        }

        return base.SelectTemplate(item, container);
    }
}
