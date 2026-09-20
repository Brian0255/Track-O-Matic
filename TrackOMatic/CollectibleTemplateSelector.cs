using System.Windows;
using System.Windows.Controls;

using TrackOMatic.ViewModels;

namespace TrackOMatic;

public class CollectibleTemplateSelector : DataTemplateSelector
{
    public required DataTemplate BlueprintTemplate { get; init; }
    public required DataTemplate CollectibleTemplate { get; init; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is UiTotalBlueprintViewModel)
        {
            return BlueprintTemplate;
        }
        if (item is UiCountableViewModel)
        {
            return CollectibleTemplate;
        }
        return base.SelectTemplate(item, container);
    }
}
