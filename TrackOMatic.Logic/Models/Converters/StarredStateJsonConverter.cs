using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackOMatic.Logic.Models.Converters;

internal class StarredStateJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.GetInt32() == 0,
            JsonTokenType.String => reader.GetString() switch
            {
                "true" => true,
                "0" => true,
                "Visible" => true,
                "visible" => true,
                _ => false,
            },
            _ => throw new JsonException($"Unexpected token type {reader.TokenType} for Starred field")
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
