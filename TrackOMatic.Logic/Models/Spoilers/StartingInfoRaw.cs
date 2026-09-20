using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Spoilers;

public class StartingInfoRaw
{
    /// <summary>
    /// Gets the order of the final bosses in the game, represented as a list of integers.
    /// </summary>
    /// <remarks>The integers will be mapped to more specific entries in the parsed output.</remarks>
    [JsonPropertyName("krool_order")]
    public List<int>? FinalBossOrder { get; init; }

    /// <summary>
    /// Gets the order of the kongs needed to destroy the Blast-O-Matic in Hideout Helm.
    /// </summary>
    [JsonPropertyName("helm_order")]
    public List<int>? HelmOrder { get; init; }

    /// <summary>
    /// Gets the starting kongs for the player for this seed.
    /// </summary>
    /// <remarks>The order in this collection is irrelevant.</remarks>
    [JsonPropertyName("starting_kongs")]
    public List<int>? StartingKongs { get; init; }

    /// <summary>
    /// Gets the starting keys for the player for this seed.
    /// </summary>
    /// <remarks>These should correspond to more friendly item names.</remarks>
    [JsonPropertyName("starting_keys")]
    public List<string>? StartingKeys { get; init; }

    /// <summary>
    /// Gets the starting moves for the player for this seed.
    /// </summary>
    [JsonPropertyName("starting_moves")]
    public List<string>? StartingMoves { get; init; }

    /// <summary>
    /// Gets the starting moves that are always implied and cannot be path hinted for this seed.
    /// </summary>
    [JsonPropertyName("starting_moves_not_hintable")]
    public List<string>? UnhintableMoves { get; init; }

    /// <summary>
    /// Gets the number of starting moves that are considered required to beat the seed.
    /// </summary>
    /// <remarks>This excludes the <see cref="UnhintableMoves"/>.</remarks>
    [JsonPropertyName("starting_moves_woth_count")]
    public int StartingMovesWothCount { get; init; }

    /// <summary>
    /// Gets the order of the levels in the game, represented as a list of integers.
    /// Each index slot corresponds to the vanilla level order.
    /// </summary>
    /// <remarks>A mapping function can convert to <see cref="RegionName"/> as required.</remarks>
    [JsonPropertyName("level_order")]
    public List<int>? LevelOrder { get; init; }

    /// <summary>
    /// Gets the B. Locker data for this seed. The order is guaranteed to follow the vanilla level order.
    /// </summary>
    [JsonPropertyName("blocker_info")]
    public List<BlockerInfoRaw>? BlockerInfo { get; init; }
}
