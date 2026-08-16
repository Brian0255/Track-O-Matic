using System.Text.Json.Serialization;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Converters;

namespace TrackOMatic.Logic.Models;

public class SavedProgress
{
    [JsonPropertyName("Version")]
    public Version SaveFileVersion { get; set; } = new Version(3, 0, 0);
    public Dictionary<ItemName, SavedItem> SavedItems { get; set; }
    public Dictionary<RegionName, string> SavedGBCounts { get; set; }
    public Dictionary<RegionName, int> BLockerImageIndexes { get; set; }
    public List<int> HelmDoorImageIndexes { get; set; }
    public List<string> HelmDoorCounts { get; set; }

    /// <summary>
    /// Gets the collection of saved hints that have been collected in the game.
    /// </summary>
    /// <remarks>
    /// This collection uses a custom converter for serialization/deserialization to handle
    /// migration from the legacy HintPanelKey format to the new HintPanelType enum format.
    /// </remarks>
    [JsonPropertyName("SavedHints")]
    [JsonConverter(typeof(SavedHintsConverter))]
    public List<SavedHint> SavedHints { get; set; } = [];

    [JsonPropertyName("spoilerPath")]
    public string SpoilerPath { get; set; }
    public List<int> HelmKongs { get; set; }
    public List<int> BossKongs { get; set; }
    public List<int> LevelOrder { get; set; }
    /// <summary>
    /// Gets or sets the collection of countable items (pearls, battle crowns, golden bananas, etc) that have been collected in the game.
    /// </summary>
    /// <remarks>This is currently not serialized to the save file, but perhaps that should change in the future.</remarks>
    [JsonIgnore]
    public Dictionary<ItemType, int> Collectibles { get; set; }
    /// <summary>
    /// Gets a value indicating whether the spoiler file has been loaded.
    /// </summary>
    /// <remarks>This should never be serialized to the save file.</remarks>
    [JsonIgnore]
    public bool IsSpoilerLoaded => !string.IsNullOrWhiteSpace(SpoilerPath);

    public SavedProgress()
    {
        SavedItems = [];
        SavedHints = [];
        SavedGBCounts = [];
        BLockerImageIndexes = [];
        HelmDoorImageIndexes = [];
        HelmDoorCounts = [];
        Collectibles = [];
        HelmKongs = [ 0, 0, 0, 0, 0 ];
        BossKongs = [ 0, 0, 0, 0, 0 ];
        LevelOrder = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
        SpoilerPath = "";
    }
}
