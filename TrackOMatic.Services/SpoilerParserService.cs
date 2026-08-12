using System.Runtime.CompilerServices;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services.SpoilerDeserialization;

[assembly: InternalsVisibleTo("TrackOMatic.Services.Test")]

namespace TrackOMatic.Services;

/// <summary>
/// Implementation of <see cref="ISpoilerService"/> that handles complete deserialization
/// and parsing of spoiler JSON files.
/// </summary>
public class SpoilerParserService : ISpoilerService
{
    private readonly RawSpoilerFileDeserializer _rawDeserializer = new();

    /// <summary>
    /// Deserializes and parses a spoiler file from a file path asynchronously.
    /// </summary>
    public async Task<DeserializationResult<ParsedSpoilerData>> DeserializeAndParseAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Deserialize the raw JSON file
            var rawResult = await _rawDeserializer.DeserializeFromFileAsync(filePath, cancellationToken);
            if (!rawResult.IsSuccess || rawResult.Data == null)
            {
                return DeserializationResult<ParsedSpoilerData>.Failure(
                    rawResult.ErrorMessage ?? "Failed to deserialize spoiler file.");
            }

            // Step 2: Parse the raw file into ParsedSpoilerData
            return await ParseAsync(rawResult.Data, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return DeserializationResult<ParsedSpoilerData>.Failure("Deserialization operation was cancelled.");
        }
        catch (Exception ex)
        {
            return DeserializationResult<ParsedSpoilerData>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deserializes and parses a spoiler from a JSON string asynchronously.
    /// </summary>
    public async Task<DeserializationResult<ParsedSpoilerData>> DeserializeAndParseFromStringAsync(
        string jsonContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Deserialize the JSON string
            var rawResult = await _rawDeserializer.DeserializeFromStringAsync(jsonContent, cancellationToken);
            if (!rawResult.IsSuccess || rawResult.Data == null)
            {
                return DeserializationResult<ParsedSpoilerData>.Failure(
                    rawResult.ErrorMessage ?? "Failed to deserialize JSON content.");
            }

            // Step 2: Parse the raw file into ParsedSpoilerData
            return await ParseAsync(rawResult.Data, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return DeserializationResult<ParsedSpoilerData>.Failure("Deserialization operation was cancelled.");
        }
        catch (Exception ex)
        {
            return DeserializationResult<ParsedSpoilerData>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Parses a raw spoiler file into ParsedSpoilerData.
    /// </summary>
    private Task<DeserializationResult<ParsedSpoilerData>> ParseAsync(
        RawSpoilerFile rawSpoiler,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException("Parsing operation was cancelled.");
            }
            if (rawSpoiler == null)
            {
                // Possibility of allowing for returning an empty ParsedSpoilerData in the future.
                return Task.FromResult(
                    DeserializationResult<ParsedSpoilerData>.Failure("Raw spoiler file is null."));
            }

            var possibleRegionInfo = rawSpoiler.HintData?.RegionDataDictionary?.FirstOrDefault().Value;
            SpoilerSettings? settings = SetUpSettings(possibleRegionInfo);

            var startingInfo = rawSpoiler.HintData?.StartingInfo ?? new();

            ParsedSpoilerData data = new()
            {
                SpoilerSettings = settings ?? new(),
                RegionData = ParseRegionPointData(rawSpoiler),
                PointSpread = ParsePointSpread(rawSpoiler),
                StartingItems = ParseStartingMoves(rawSpoiler),
                HelmOrder = ParseHelmOrder(startingInfo),
                FinalBossOrder = ParseFinalBossOrder(startingInfo),
                LevelOrder = ParseLevelOrder(startingInfo),
                RegionBarrierInfo = ParseBlockerInfo(startingInfo),
            };

            return Task.FromResult(DeserializationResult<ParsedSpoilerData>.Success(data));
        }
        catch (OperationCanceledException)
        {
            return Task.FromResult(
                DeserializationResult<ParsedSpoilerData>.Failure("Parsing operation was cancelled."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                DeserializationResult<ParsedSpoilerData>.Failure($"Parsing error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Sets up SpoilerSettings based on region spoiler data.
    /// </summary>
    internal static SpoilerSettings SetUpSettings(RawRegionSpoilerData? info)
    {
        if (info == null)
        {
            return new();
        }
        SpoilerSettings settings;
        bool pointsEnabled = info.Points != -1;
        bool vialsEnabled = !pointsEnabled;
        bool hoardEnabled = info.WothCount != -1;
        settings = new SpoilerSettings(pointsEnabled, vialsEnabled, hoardEnabled);
        return settings;
    }

    internal static Dictionary<RegionName, RegionSpoilerData> ParseRegionPointData(RawSpoilerFile rawSpoiler)
    {
        Dictionary<RegionName, RegionSpoilerData> regionDataDictionary = [];
        foreach (var kvp in rawSpoiler.HintData?.RegionDataDictionary ?? [])
        {
            RegionName region = Extensions.TryParseRegionFromLabel(kvp.Value.LevelName ?? "");
            if (region == RegionName.UNKNOWN)
            {
                continue;
            }

            // Sort vial color strings using VIAL_MAP, then convert to enum values
            var vialColorStrings = kvp.Value.VialColors ?? [];
            vialColorStrings.Sort((a, b) => SpoilerParserMappings.VIAL_MAP.TryGetValue(a, out var colorA) && SpoilerParserMappings.VIAL_MAP.TryGetValue(b, out var colorB)
                ? (int)colorA - (int)colorB
                : 0);
            List<VialColor> vialColors = [.. vialColorStrings
                .Select(v => SpoilerParserMappings.VIAL_MAP.TryGetValue(v, out var color) ? color : VialColor.NONE)
                .Where(v => v != VialColor.NONE)];

            regionDataDictionary[region] = new RegionSpoilerData
            {
                VialColors = vialColors,
                Points = kvp.Value.Points,
                WothCount = kvp.Value.WothCount
            };
        }

        // Don't forget the starting region's points.
        regionDataDictionary[RegionName.START] = new RegionSpoilerData
        {
            VialColors = [],
            Points = 0,
            WothCount = rawSpoiler.HintData?.StartingInfo?.StartingMovesWothCount ?? 0
        };

        return regionDataDictionary;
    }

    internal static List<int> ParseHelmOrder(StartingInfoRaw info)
    {
        List<int> helmOrder = [];
        foreach (var helm in info.HelmOrder ?? [])
        {
            helmOrder.Add(helm + 1);
        }
        return helmOrder;
    }

    internal static List<int> ParseFinalBossOrder(StartingInfoRaw info)
    {
        List<int> bossOrder = [];
        foreach (var helm in info.FinalBossOrder ?? [])
        {
            var imageIndex = (int)SpoilerParserMappings.KROOL_MAP_TO_IMAGE_INDEX[helm];
            bossOrder.Add(imageIndex);
        }
        return bossOrder;
    }

    internal static Dictionary<RegionName, int> ParseLevelOrder(StartingInfoRaw info)
    {
        Dictionary<RegionName, int> levelOrder = [];
        if (info.LevelOrder == null)
        {
            return levelOrder;
        }
        foreach (var (region, i) in EndGameMappings.LOBBY_ORDER.Select((r, i) => (r, i)))
        {
            var hasNoLevel = i >= info.LevelOrder.Count;
            var oldOrder = hasNoLevel ? 0 : info.LevelOrder![i];
            var newOrder = hasNoLevel ? 0 : (i + 1);
            var toChange = hasNoLevel ? region : EndGameMappings.LOBBY_ORDER[oldOrder];
            levelOrder[toChange] = newOrder;
        }
        // If Helm isn't included above, add to the end.
        if (info.LevelOrder != null && info.LevelOrder.Count == 7)
        {
            levelOrder[RegionName.HIDEOUT_HELM] = 8;
        }
        return levelOrder;
    }

    internal static Dictionary<PointCategory, int> ParsePointSpread(RawSpoilerFile rawSpoiler)
    {
        Dictionary<string, int> oldSpread = rawSpoiler.HintData?.PointSpread ?? [];
        Dictionary<PointCategory, int> newSpread = [];
        if (oldSpread.Count == 0)
        {
            return newSpread;
        }

        // Account for older versions that didn't have fairy moves split off.
        if (!oldSpread.ContainsKey("fairy_moves"))
        {
            oldSpread["fairy_moves"] = oldSpread["training_moves"];
        }

        foreach (var (key, value) in oldSpread)
        {
            if (SpoilerParserMappings.POINT_NAME_TO_CATEGORY.TryGetValue(key, out var category))
            {
                newSpread[category] = value;
            }
        }
        return newSpread;
    }

    /// <summary>
    /// Parses the B. Locker door information for each region.
    /// </summary>
    /// <remarks>The return type should change in the future to remove numerical indexing and instead utilize the <see cref="RegionName"/>.</remarks>
    /// <param name="info">The starting info from the spoiler log.</param>
    /// <returns>The B. Locker data.</returns>
    internal static List<BlockerInfoRaw> ParseBlockerInfo(StartingInfoRaw info)
    {
        List<BlockerInfoRaw> blockerInfoList = info.BlockerInfo ?? [];
        return blockerInfoList;
    }

    internal static Dictionary<ItemName, RegionName> ParseStartingMoves(RawSpoilerFile rawSpoiler)
    {
        StartingInfoRaw info = rawSpoiler.HintData?.StartingInfo ?? new();
        Dictionary<ItemName, RegionName> startingMoves = [];
        foreach (var kongIndex in info.StartingKongs ?? [])
        {
            var kongItem = SpoilerParserMappings.KONGS[kongIndex];
            startingMoves[kongItem] = RegionName.UNHINTABLE_MOVES;
        }
        foreach (var keyString in info.StartingKeys ?? [])
        {
            var key = SpoilerParserMappings.ITEM_MAP[keyString];
            startingMoves[key] = RegionName.UNHINTABLE_MOVES;
        }

        /*
        var shockwaveShuffle = rawSpoiler.Settings?.ShockwaveShuffle ?? "never";
        if (shockwaveShuffle == "start_with")
        {
            startingMoves.Add(ItemName.FAIRY_CAMERA, RegionName.START);
            startingMoves.Add(ItemName.SHOCKWAVE, RegionName.START);
        }
        */

        void ReadStartingMoves(List<string> starting_moves, RegionName regionToPlace)
        {
            var slams = 0;
            for (int i = 0; i < starting_moves.Count; ++i)
            {
                var itemString = starting_moves[i];
                if (itemString == "Progressive Slam")
                {
                    slams++;
                    itemString = itemString + " " + slams.ToString();
                }
                if (SpoilerParserMappings.RANDO_NAME_TO_ITEM_NAME.ContainsKey(itemString))
                {
                    startingMoves[SpoilerParserMappings.RANDO_NAME_TO_ITEM_NAME[itemString]] = regionToPlace;
                }
            }
        }

        ReadStartingMoves(info.StartingMoves ?? [], RegionName.START);
        ReadStartingMoves(info.UnhintableMoves ?? [], RegionName.UNHINTABLE_MOVES);

        // See if shopkeepers have to be added. Older rando seeds may require this.
        List<string> pool = rawSpoiler.ItemPool ?? [];
        string[] shopkeepers = ["Cranky", "Funky", "Candy", "Wrinkly"];
        if (shopkeepers.All((s) => !pool.Contains(s)))
        {
            ItemName[] localShopkeepers = [ItemName.CRANKY, ItemName.FUNKY, ItemName.CANDY, ItemName.SNIDE];
            foreach (var shopkeeper in localShopkeepers)
            {
                if (!startingMoves.ContainsKey(shopkeeper))
                {
                    startingMoves[shopkeeper] = RegionName.START;
                }
            }
        }

        return startingMoves;
    }
}
