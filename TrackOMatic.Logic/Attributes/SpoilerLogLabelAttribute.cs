namespace TrackOMatic.Logic.Attributes;

/// <summary>
/// Maps an enum value to its corresponding label in the spoiler log.
/// Used to deserialize spoiler log strings into strongly-typed enums.
/// </summary>
/// <remarks>
/// Example: ANGRY_AZTEC → "Angry Aztec", DK_ISLES → "DK Isles"
/// The attribute handles cases where PascalCase conversion wouldn't work.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="SpoilerLogLabelAttribute"/> class.
/// </remarks>
/// <param name="label">The exact label string from the spoiler log (e.g., "Angry Aztec").</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SpoilerLogLabelAttribute(string label) : Attribute
{
    /// <summary>
    /// Gets the label as it appears in the spoiler log.
    /// </summary>
    public string Label { get; } = label;
}
