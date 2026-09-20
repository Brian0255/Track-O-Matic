using System.Windows;
using System.Windows.Controls;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

/// <summary>
/// Selects the appropriate layout template based on the current hint display mode and compact/standard setting.
/// This ensures only one set of hint panels exists in the visual tree at any time, preventing duplicate PopupRoot creation.
/// </summary>
public class HintModeTemplateSelector : DataTemplateSelector
{
    public required DataTemplate DirectHintCompactTemplate { get; set; }
    public required DataTemplate DirectHintStandardTemplate { get; set; }
    public required DataTemplate MultipathHintCompactTemplate { get; set; }
    public required DataTemplate MultipathHintStandardTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is not MainHintViewModel viewModel)
        {
            return base.SelectTemplate(item, container);
        }

        if (viewModel.IsDirectHintCompactMode)
        {
            return DirectHintCompactTemplate;
        }

        if (viewModel.IsDirectHintStandardMode)
        {
            return DirectHintStandardTemplate;
        }

        if (viewModel.IsMultipathCompactMode)
        {
            return MultipathHintCompactTemplate;
        }

        if (viewModel.IsMultipathStandardMode)
        {
            return MultipathHintStandardTemplate;
        }

        return base.SelectTemplate(item, container);
    }
}
