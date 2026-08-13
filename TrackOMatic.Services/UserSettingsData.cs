using System.Text.Json.Serialization;

namespace TrackOMatic.Services;

/// <summary>
/// Represents the flattened JSON structure for user settings.
/// This is used for JSON serialization/deserialization.
/// </summary>
public class UserSettingsData
{
    // Display toggles
    public bool TopMost { get; set; } = false;
    public bool SongDisplay { get; set; } = false;
    public bool CompactMode { get; set; } = true;
    public bool HelmInLevelOrder { get; set; } = false;
    public bool HelmDoors { get; set; } = false;
    public bool AutoSortPathHints { get; set; } = false;
    public bool EnemiesInAutofill { get; set; } = false;
    public bool ColoredBarrelPadMoves { get; set; } = false;
    public bool Autotracking { get; set; } = true;

    // Collectibles display
    public bool ShowTotalBPs { get; set; } = false;
    public bool ShowCompanyCoins { get; set; } = false;

    // Spoiler hint display
    public bool ShowKRoolOrder { get; set; } = true;
    public bool ShowHelmOrder { get; set; } = true;

    // Broadcast view
    [JsonIgnore]
    public bool BroadcastViewActive { get; set; } = false;
    public bool BroadcastHelmKRool { get; set; } = false;
    public bool BroadcastShopkeepers { get; set; } = false;
    public bool BroadcastSongDisplay { get; set; } = false;

    // Broadcast number label - stored as string in JSON for human readability
    public string BroadcastNumberLabel { get; set; } = "WothCount";

    // Hint display mode - stored as string in JSON for human readability
    public string HintDisplay { get; set; } = "MultipathHints";

    // Progressive hints - only user-controlled display option
    public bool ShowAmountForHints { get; set; } = false;
}
