using TrackOMatic.Logic.Models.Spoilers;

namespace TrackOMatic.Services;

/// <summary>
/// Provides unified methods to deserialize and parse spoiler JSON files into strongly-typed
/// <see cref="ParsedSpoilerData"/> objects.
/// </summary>
/// <remarks>
/// This service handles the complete pipeline: JSON deserialization to raw spoiler file,
/// then transformation into parsed spoiler data ready for application use.
/// </remarks>
public interface ISpoilerService
{
    /// <summary>
    /// Deserializes and parses a spoiler file from a file path asynchronously.
    /// </summary>
    /// <param name="filePath">The full path to the spoiler JSON file.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="DeserializationResult{T}"/> containing the parsed spoiler data or error information.
    /// </returns>
    Task<DeserializationResult<ParsedSpoilerData>> DeserializeAndParseAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a deserialization and parsing operation.
/// </summary>
/// <typeparam name="T">The type of data that was deserialized and parsed.</typeparam>
public record DeserializationResult<T>(
    bool IsSuccess,
    T? Data,
    string? ErrorMessage
)
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static DeserializationResult<T> Success(T data) =>
        new(IsSuccess: true, Data: data, ErrorMessage: null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static DeserializationResult<T> Failure(string errorMessage) =>
        new(IsSuccess: false, Data: default, ErrorMessage: errorMessage);
}
