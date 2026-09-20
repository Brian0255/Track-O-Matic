using System.Text.Json;
using System.Text.Json.Serialization;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Converters;

/// <summary>
/// Custom JsonConverter for Dictionary&lt;ItemName, SavedItem&gt; that filters out empty items
/// during serialization.
///
/// An item is considered empty if:
/// - It is not Starred
/// - It is in the UNKNOWN region
/// - It has not been Hinted
///
/// This prevents the save file from being bloated with default/empty item states.
/// </summary>
internal class SavedItemsConverter : JsonConverter<Dictionary<ItemName, SavedItem>>
{
    public override Dictionary<ItemName, SavedItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var items = new Dictionary<ItemName, SavedItem>();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected object for SavedItems");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string? propertyName = reader.GetString();
                reader.Read();

                if (propertyName != null && Enum.TryParse<ItemName>(propertyName, out var itemName))
                {
                    var savedItem = JsonSerializer.Deserialize<SavedItem>(ref reader, options);
                    if (savedItem != null)
                    {
                        items[itemName] = savedItem;
                    }
                }
            }
        }

        return items;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ItemName, SavedItem> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Only serialize non-empty items
        foreach (var kvp in value.Where(x => !x.Value.IsEmpty))
        {
            writer.WritePropertyName(kvp.Key.ToString());

            var json = JsonSerializer.Serialize(kvp.Value, options);
            using var doc = JsonDocument.Parse(json);
            doc.RootElement.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}
