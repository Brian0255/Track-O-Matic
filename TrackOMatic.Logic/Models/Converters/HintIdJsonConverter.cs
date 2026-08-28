using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackOMatic.Logic.Models.Converters;

/// <summary>
/// JSON converter for <see cref="HintId"/> that serializes it as a plain string value.
/// This ensures "HintId": "abcdef12" instead of "HintId": { "Value": "abcdef12" }
/// </summary>
public class HintIdJsonConverter : JsonConverter<HintId>
{
    public override HintId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing HintId. Expected String.");
        }

        string? value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new JsonException("HintId cannot be null or empty.");
        }

        try
        {
            return new HintId(value);
        }
        catch (ArgumentException)
        {
            return new HintId(Guid.NewGuid()); // Fallback to a new Guid if the value is invalid
        }
    }

    public override void Write(Utf8JsonWriter writer, HintId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
