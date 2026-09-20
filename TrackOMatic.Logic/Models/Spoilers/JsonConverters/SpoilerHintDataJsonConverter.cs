using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackOMatic.Logic.Models.Spoilers.JsonConverters;

/// <summary>
/// Custom JSON converter that transforms the spoiler hints data structure
/// from individual properties (0, 1, 2, ..., 8) into a single
/// Dictionary&lt;int, RegionSpoilerData&gt;.
/// </summary>
/// <remarks>
/// The spoiler file format uses numeric string keys (0-8) for the 9 regions.
/// This converter flattens them into a clean dictionary for easier access.
///
/// Example JSON:
/// {
///   "0": { "level_name": "Jungle Japes", ... },
///   "1": { "level_name": "Angry Aztec", ... },
///   ...
///   "starting_info": { ... },
///   "point_spread": { ... }
/// }
///
/// Becomes:
/// {
///   "RegionDataDictionary": { 0 → RegionSpoilerData, 1 → RegionSpoilerData, ... }
///   "StartingInfo": { ... },
///   "PointSpread": { ... }
/// }
/// </remarks>
public class SpoilerHintDataJsonConverter : JsonConverter<SpoilerHintData>
{
    /// <summary>
    /// Reads the JSON and transforms it into a SpoilerHintData object
    /// with a dictionary of regions instead of individual properties.
    /// </summary>
    public override SpoilerHintData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // We'll manually parse the JSON object
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token.");
        }

        var regionsDictionary = new Dictionary<int, RawRegionSpoilerData>();
        StartingInfoRaw? startingInfo = null;
        Dictionary<string, int>? pointSpread = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            string? propertyName = reader.GetString();

            // Advance to the property value
            reader.Read();

            if (propertyName == "starting_info")
            {
                // starting_info can be either a JSON object or a JSON string
                if (reader.TokenType == JsonTokenType.String)
                {
                    string? jsonString = reader.GetString();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        startingInfo = JsonSerializer.Deserialize<StartingInfoRaw>(jsonString, options);
                    }
                }
                else
                {
                    startingInfo = JsonSerializer.Deserialize<StartingInfoRaw>(ref reader, options);
                }
            }
            else if (propertyName == "point_spread")
            {
                // point_spread can be either a JSON object or a JSON string
                if (reader.TokenType == JsonTokenType.String)
                {
                    string? jsonString = reader.GetString();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        pointSpread = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonString, options);
                    }
                }
                else
                {
                    pointSpread = JsonSerializer.Deserialize<Dictionary<string, int>>(ref reader, options);
                }
            }
            else if (int.TryParse(propertyName, out int regionIndex) && regionIndex >= 0 && regionIndex <= 8)
            {
                // Region data can be either a JSON object or a JSON string
                RawRegionSpoilerData? regionData = null;
                if (reader.TokenType == JsonTokenType.String)
                {
                    string? jsonString = reader.GetString();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        regionData = JsonSerializer.Deserialize<RawRegionSpoilerData>(jsonString, options);
                    }
                }
                else
                {
                    regionData = JsonSerializer.Deserialize<RawRegionSpoilerData>(ref reader, options);
                }

                if (regionData != null)
                {
                    regionsDictionary[regionIndex] = regionData;
                }
            }
        }

        return new SpoilerHintData
        {
            RegionDataDictionary = regionsDictionary,
            StartingInfo = startingInfo,
            PointSpread = pointSpread
        };
    }

    /// <summary>
    /// Writes the SpoilerHintData object back to JSON format.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, SpoilerHintData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Write regions dictionary as individual properties
        if (value.RegionDataDictionary != null)
        {
            foreach (var kvp in value.RegionDataDictionary)
            {
                writer.WritePropertyName(kvp.Key.ToString());
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }
        }

        // Write starting info
        if (value.StartingInfo != null)
        {
            writer.WritePropertyName("starting_info");
            JsonSerializer.Serialize(writer, value.StartingInfo, options);
        }

        // Write point spread
        if (value.PointSpread != null)
        {
            writer.WritePropertyName("point_spread");
            JsonSerializer.Serialize(writer, value.PointSpread, options);
        }

        writer.WriteEndObject();
    }
}
