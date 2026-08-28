using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("TrackOMatic.Services.Test")]

namespace TrackOMatic.Services;

/// <summary>
/// Handles file I/O and JSON serialization for game progress data.
/// Pure data persistence layer; does not mutate UI or game state.
/// </summary>
public class DataPersistenceService : IDataPersistenceService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<T> LoadAsync<T>(string filePath) where T : class, new()
    {
        if (!File.Exists(filePath))
        {
            return new T();
        }

        try
        {
            using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,  // Allow concurrent reads and deletes
                bufferSize: 4096,
                FileOptions.SequentialScan | FileOptions.Asynchronous  // Optimize for sequential read + async
            );
            using var reader = new StreamReader(stream);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var jsonString = await reader.ReadToEndAsync(cts.Token).ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions) ?? new T();
        }
        catch (Exception)
        {
            return new T();
        }
    }

    public async Task SaveAsync<T>(T data, string filePath, bool ensureDirectory = true) where T : class
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        }

        if (ensureDirectory)
        {
            string? directory = Path.GetDirectoryName(filePath);
            // Only create directory if it's not empty (empty string means file is in current directory)
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        var jsonString = JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(filePath, jsonString).ConfigureAwait(false);
    }
}
