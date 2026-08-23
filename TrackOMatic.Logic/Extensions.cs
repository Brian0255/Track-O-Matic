using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TrackOMatic.Logic.Attributes;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic;

public static class Extensions
{
    /// <summary>
    /// Converts a legacy name string (from <see cref="LegacyNameAttribute"/>)
    /// to the corresponding enum value.
    /// </summary>
    public static T FromLegacyToEnum<T>(this string value) where T : struct, Enum
    {
        // Try direct enum parse first
        if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
        {
            return result;
        }

        // Try legacy names
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var legacyNameAttr = field.GetCustomAttribute<LegacyNameAttribute>();
            if (legacyNameAttr?.LegacyName == value)
            {
                return (T)field.GetValue(null)!;
            }
        }

        // Fallback to default
        return default!;

    }

    /// <summary>
    /// Determines whether a <see cref="SavedItem"/> entry should be kept based on its state.
    /// Rules:
    /// 1. If starred, always keep
    /// 2. If hinted with opacity 0.375 and region is not <see cref="RegionName.UNKNOWN"/>, keep (right-click drag hint)
    /// 3. If found (opacity >= 1.0) in non-<see cref="RegionName.UNKNOWN"/> region and not autotracked, keep
    /// 4. If autotracked (opacity >= 1.0, region != <see cref="RegionName.UNKNOWN"/>), keep
    /// Otherwise remove.
    /// </summary>
    /// <returns>A boolean indicating whether the entry is kept.</returns>
    public static bool ShouldKeepSavedItem(this SavedItem item)
    {
        // Rule 1: Starred items always kept
        if (item.Starred == ItemVisibilityState.Visible)
        {
            return true;
        }

        // Rule 2: Right-click drag hint (opacity 0.375 in a region)
        if (Math.Abs(item.Opacity - 0.375) < 0.01 && item.Region != RegionName.UNKNOWN)
        {
            return true;
        }

        // Rule 3 & 4: Found or autotracked items
        if (item.Opacity >= 1.0 && item.Region != RegionName.UNKNOWN)
        {
            // Either found (not autotracked) or autotracked - both are valid
            return true;
        }

        // No valid reason to keep
        return false;
    }

    /// <summary>
    /// Gets the spoiler log label for a <see cref="RegionName"/> value.
    /// If no label is defined, returns the enum value name (UPPER_CASE format).
    /// </summary>
    /// <param name="region">The region enum value.</param>
    /// <returns>The spoiler log label string, or the enum name if no attribute is defined.</returns>
    public static string GetSpoilerLogLabel(this RegionName region)
    {
        var field = region.GetType().GetField(region.ToString());
        if (field == null)
        {
            return region.ToString();
        }

        var attribute = field.GetCustomAttribute<SpoilerLogLabelAttribute>();
        return attribute?.Label ?? region.ToString();
    }

    /// <summary>
    /// Attempts to parse a spoiler log label into a <see cref="RegionName"/> enum value.
    /// Invalid labels return <see cref="RegionName.UNKNOWN"/>.
    /// </summary>
    /// <param name="label">The spoiler log label (e.g., "Angry Aztec").</param>
    /// <returns>
    /// The corresponding <see cref="RegionName"/> value, or <see cref="RegionName.UNKNOWN"/> if not found.
    /// </returns>
    public static RegionName TryParseRegionFromLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return RegionName.UNKNOWN;
        }

        var fields = typeof(RegionName).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<SpoilerLogLabelAttribute>();
            if (attribute?.Label == label)
            {
                return (RegionName)field.GetValue(null)!;
            }
        }

        return RegionName.UNKNOWN;
    }

    public static PointCategory ToPointCategory(this ItemName itemName) => itemName switch
    {
        ItemName.KEY_1 => PointCategory.Key,
        ItemName.KEY_2 => PointCategory.Key,
        ItemName.KEY_3 => PointCategory.Key,
        ItemName.KEY_4 => PointCategory.Key,
        ItemName.KEY_5 => PointCategory.Key,
        ItemName.KEY_6 => PointCategory.Key,
        ItemName.KEY_7 => PointCategory.Key,
        ItemName.KEY_8 => PointCategory.Key,
        ItemName.BASIC_KEY => PointCategory.Key,

        ItemName.DONKEY => PointCategory.Kong,
        ItemName.DIDDY => PointCategory.Kong,
        ItemName.LANKY => PointCategory.Kong,
        ItemName.TINY => PointCategory.Kong,
        ItemName.CHUNKY => PointCategory.Kong,

        ItemName.BONGO_BLAST => PointCategory.Instrument,
        ItemName.GUITAR_GAZUMP => PointCategory.Instrument,
        ItemName.TROMBONE_TREMOR => PointCategory.Instrument,
        ItemName.SAXOPHONE_SLAM => PointCategory.Instrument,
        ItemName.TRIANGLE_TRAMPLE => PointCategory.Instrument,

        ItemName.COCONUT_GUN => PointCategory.Gun,
        ItemName.PEANUT_POPGUNS => PointCategory.Gun,
        ItemName.GRAPE_SHOOTER => PointCategory.Gun,
        ItemName.FEATHER_BOW => PointCategory.Gun,
        ItemName.PINEAPPLE_LAUNCHER => PointCategory.Gun,

        ItemName.GORILLA_GRAB => PointCategory.PhysicalMove,
        ItemName.CHIMPY_CHARGE => PointCategory.PhysicalMove,
        ItemName.ORANGSTAND => PointCategory.PhysicalMove,
        ItemName.PONYTAIL_TWIRL => PointCategory.PhysicalMove,
        ItemName.PRIMATE_PUNCH => PointCategory.PhysicalMove,

        ItemName.STRONG_KONG => PointCategory.BarrelMove,
        ItemName.ROCKETBARREL_BOOST => PointCategory.BarrelMove,
        ItemName.ORANGSTAND_SPRINT => PointCategory.BarrelMove,
        ItemName.MINI_MONKEY => PointCategory.BarrelMove,
        ItemName.HUNKY_CHUNKY => PointCategory.BarrelMove,

        ItemName.BABOON_BLAST => PointCategory.PadMove,
        ItemName.SIMIAN_SPRING => PointCategory.PadMove,
        ItemName.BABOON_BALLOON => PointCategory.PadMove,
        ItemName.MONKEYPORT => PointCategory.PadMove,
        ItemName.GORILLA_GONE => PointCategory.PadMove,

        ItemName.SHOCKWAVE => PointCategory.FairyMove,
        ItemName.FAIRY_CAMERA => PointCategory.FairyMove,
        ItemName.CAMERA_SHOCKWAVE => PointCategory.FairyMove,

        ItemName.CLIMBING => PointCategory.TrainingMove,
        ItemName.DIVING => PointCategory.TrainingMove,
        ItemName.BARREL_THROWING => PointCategory.TrainingMove,
        ItemName.VINE_SWINGING => PointCategory.TrainingMove,
        ItemName.ORANGE_THROWING => PointCategory.TrainingMove,

        ItemName.SNIPER_SCOPE => PointCategory.SharedMove,
        ItemName.HOMING_AMMO => PointCategory.SharedMove,
        ItemName.HOMING_SCOPE => PointCategory.SharedMove,
        ItemName.PROGRESSIVE_SLAM_1 => PointCategory.SharedMove,
        ItemName.PROGRESSIVE_SLAM_2 => PointCategory.SharedMove,
        ItemName.PROGRESSIVE_SLAM_3 => PointCategory.SharedMove,

        ItemName.CRANKY => PointCategory.Shopkeeper,
        ItemName.FUNKY => PointCategory.Shopkeeper,
        ItemName.CANDY => PointCategory.Shopkeeper,
        ItemName.SNIDE => PointCategory.Shopkeeper,

        ItemName.BEAN => PointCategory.Bean,

        _ => PointCategory.Unknown,
    };

    public static VialColor ToVialColor(this ItemName itemName) => itemName switch
    {
        ItemName.DONKEY => VialColor.KONG,
        ItemName.DIDDY => VialColor.KONG,
        ItemName.LANKY => VialColor.KONG,
        ItemName.TINY => VialColor.KONG,
        ItemName.CHUNKY => VialColor.KONG,
        ItemName.CRANKY => VialColor.KONG,
        ItemName.CANDY => VialColor.KONG,
        ItemName.FUNKY => VialColor.KONG,
        ItemName.SNIDE => VialColor.KONG,
        ItemName.KEY_1 => VialColor.KEY,
        ItemName.KEY_2 => VialColor.KEY,
        ItemName.KEY_3 => VialColor.KEY,
        ItemName.KEY_4 => VialColor.KEY,
        ItemName.KEY_5 => VialColor.KEY,
        ItemName.KEY_6 => VialColor.KEY,
        ItemName.KEY_7 => VialColor.KEY,
        ItemName.KEY_8 => VialColor.KEY,
        ItemName.COCONUT_GUN => VialColor.YELLOW,
        ItemName.BONGO_BLAST => VialColor.YELLOW,
        ItemName.GORILLA_GRAB => VialColor.YELLOW,
        ItemName.STRONG_KONG => VialColor.YELLOW,
        ItemName.BABOON_BLAST => VialColor.YELLOW,
        ItemName.PEANUT_POPGUNS => VialColor.RED,
        ItemName.GUITAR_GAZUMP => VialColor.RED,
        ItemName.CHIMPY_CHARGE => VialColor.RED,
        ItemName.ROCKETBARREL_BOOST => VialColor.RED,
        ItemName.SIMIAN_SPRING => VialColor.RED,
        ItemName.GRAPE_SHOOTER => VialColor.BLUE,
        ItemName.TROMBONE_TREMOR => VialColor.BLUE,
        ItemName.ORANGSTAND => VialColor.BLUE,
        ItemName.ORANGSTAND_SPRINT => VialColor.BLUE,
        ItemName.BABOON_BALLOON => VialColor.BLUE,
        ItemName.FEATHER_BOW => VialColor.PURPLE,
        ItemName.SAXOPHONE_SLAM => VialColor.PURPLE,
        ItemName.PONYTAIL_TWIRL => VialColor.PURPLE,
        ItemName.MINI_MONKEY => VialColor.PURPLE,
        ItemName.MONKEYPORT => VialColor.PURPLE,
        ItemName.PINEAPPLE_LAUNCHER => VialColor.GREEN,
        ItemName.TRIANGLE_TRAMPLE => VialColor.GREEN,
        ItemName.PRIMATE_PUNCH => VialColor.GREEN,
        ItemName.HUNKY_CHUNKY => VialColor.GREEN,
        ItemName.GORILLA_GONE => VialColor.GREEN,
        ItemName.CLIMBING => VialColor.CLEAR,
        ItemName.VINE_SWINGING => VialColor.CLEAR,
        ItemName.BARREL_THROWING => VialColor.CLEAR,
        ItemName.ORANGE_THROWING => VialColor.CLEAR,
        ItemName.DIVING => VialColor.CLEAR,
        ItemName.FAIRY_CAMERA => VialColor.CLEAR,
        ItemName.SHOCKWAVE => VialColor.CLEAR,
        ItemName.HOMING_AMMO => VialColor.CLEAR,
        ItemName.SNIPER_SCOPE => VialColor.CLEAR,
        ItemName.PROGRESSIVE_SLAM_1 => VialColor.CLEAR,
        ItemName.PROGRESSIVE_SLAM_2 => VialColor.CLEAR,
        ItemName.PROGRESSIVE_SLAM_3 => VialColor.CLEAR,
        ItemName.BEAN => VialColor.CLEAR,
        _ => VialColor.NONE
    };

    /// <summary>
    /// Convert a collectible <see cref="ItemType"/> to its corresponding resource key string.
    /// </summary>
    /// <param name="itemType">The <see cref="ItemType"/> to convert.</param>
    /// <returns>The resource key string corresponding to the given <see cref="ItemType"/>.</returns>
    public static string ToResourceKey(this ItemType itemType)
    {
        return itemType switch
        {
            ItemType.DONKEY_BLUEPRINT => "bp_dk",
            ItemType.DIDDY_BLUEPRINT => "bp_diddy",
            ItemType.LANKY_BLUEPRINT => "bp_lanky",
            ItemType.TINY_BLUEPRINT => "bp_tiny",
            ItemType.CHUNKY_BLUEPRINT => "bp_chunky",
            ItemType.TOTAL_BLUEPRINTS => "bp_total",
            ItemType.COMPANY_COIN => "count_company",
            ItemType.RAINBOW_COIN => "count_rainbow",
            ItemType.PEARL => "count_pearl",
            ItemType.BATTLE_CROWN => "count_crown",
            ItemType.BANANA_MEDAL => "count_medal",
            ItemType.FAIRY => "count_fairy",
            ItemType.GOLDEN_BANANA => "count_gb",
            _ => ""
        };
    }

    /// <summary>
    /// Convert an raw integer from the spoiler log/game to its corresponding <see cref="BarrierItems">BarrierItem</see>.
    /// </summary>
    /// <remarks>Why does the enum not use the same values as the raw game integers?</remarks>
    /// <param name="itemId">The integer to convert.</param>
    /// <returns>The corresponding <see cref="BarrierItems">BarrierItem</see> if found; otherwise, <c>null</c>.</returns>
    public static BarrierItems? FromRawGameToBarrier(this int itemId) => itemId switch
    {
        3 => BarrierItems.GOLDEN_BANANA,
        4 => BarrierItems.BLUEPRINT,
        5 => BarrierItems.FAIRY,
        7 => BarrierItems.CROWN,
        8 => BarrierItems.COMPANY_COIN,
        9 => BarrierItems.MEDAL,
        10 => BarrierItems.BEAN,
        11 => BarrierItems.PEARL,
        12 => BarrierItems.RAINBOW_COIN,
        _ => null
    };

    public static RegionName ToRegion(this HintPanelType hintPanelType) => hintPanelType switch
    {
        HintPanelType.Japes => RegionName.JUNGLE_JAPES,
        HintPanelType.Aztec => RegionName.ANGRY_AZTEC,
        HintPanelType.Factory => RegionName.FRANTIC_FACTORY,
        HintPanelType.Galleon => RegionName.GLOOMY_GALLEON,
        HintPanelType.Forest => RegionName.FUNGI_FOREST,
        HintPanelType.Caves => RegionName.CRYSTAL_CAVES,
        HintPanelType.Castle => RegionName.CREEPY_CASTLE,
        HintPanelType.Helm => RegionName.HIDEOUT_HELM,
        _ => RegionName.UNKNOWN
    };

    public static bool IsPlayableRegion(this RegionName region)
    {
        var playableRegions = EndGameMappings.LOBBY_ORDER.Concat([RegionName.START]).ToArray();
        return playableRegions.Contains(region);
    }
}
