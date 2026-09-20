namespace TrackOMatic.Logic.Enums;

/// <summary>
/// The <see cref="ItemSelectorMode"/> enum dictates which items are displayed in the
/// Basic Item Selector as well as how they are rendered.
/// </summary>
public enum ItemSelectorMode
{
    /// <summary>
    /// The <see cref="Door"/> mode displays all door types for both B. Locker
    /// as well as the two doors within Hideout Helm.
    /// </summary>
    Door,
    /// <summary>
    /// The <see cref="Helm"/> mode displays which Kongs are needed to destroy the Blast-O-Matic within Hideout Helm./>
    /// </summary>
    Helm,
    /// <summary>
    /// The <see cref="Boss"/> mode displays what the final bosses are for the seed.
    /// </summary>
    /// <remarks>
    /// The Kong heads are the same as in Helm, but represent a K. Rool phase specifically.
    /// The other boss icons are normally region bosses but could be available here depending on the seed settings.
    /// </remarks>
    Boss
}
