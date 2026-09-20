using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackOMatic.Logic.Models.Converters;

/// <summary>
/// Custom JsonConverter that handles migration from SavedHintDto (with HintPanelKey)
/// to SavedHint (with HintPanelType) while preserving backward compatibility.
/// </summary>
internal class SavedHintsConverter : JsonConverter<List<SavedHint>>
{
    public override List<SavedHint> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hints = new List<SavedHint>();

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected array for SavedHints");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Deserialize into DTO, which handles legacy conversion
                var dto = JsonSerializer.Deserialize<SavedHintDto>(ref reader, options);
                if (dto != null)
                {
                    hints.Add(dto.ToSavedHint());
                }
            }
        }

        return hints;
    }

    public override void Write(Utf8JsonWriter writer, List<SavedHint> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var hint in value)
        {
            // Serialize SavedHint directly as JSON (enums as strings via StringEnumConverter)
            var json = JsonSerializer.Serialize(hint, options);
            using var doc = JsonDocument.Parse(json);
            doc.RootElement.WriteTo(writer);
        }

        writer.WriteEndArray();
    }
}
