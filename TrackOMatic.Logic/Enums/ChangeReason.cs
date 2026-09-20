namespace TrackOMatic.Logic.Enums;

/// <summary>
/// Represents the reason for a change in the application.
/// </summary>
public enum ChangeReason
{
    /// <summary>
    /// Indicates that the reason for the change is unknown.
    /// </summary>
    /// <remarks>This should never be the reason: it is here mainly to allow for default initialization.</remarks>
    Unknown = 0,
    /// <summary>
    /// Indicates that the change was made by the user.
    /// </summary>
    UserModified,
    /// <summary>
    /// Indicates that the change was made automatically by the system.
    /// </summary>
    AutoTracked,
    /// <summary>
    /// Indicates that the change was made when loading data from a file.
    /// </summary>
    LoadedFromFile,
    /// <summary>
    /// Indicates that the change was made when updating the domain data.
    /// </summary>
    DomainUpdate,
    /// <summary>
    /// Indicates that the change was made as part of a reset operation.
    /// </summary>
    Reset,
    /// <summary>
    /// Indicates that the change was made as part of a batch update operation.
    /// </summary>
    Batched,
}
