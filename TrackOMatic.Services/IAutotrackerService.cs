using System.Diagnostics;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Logic.Models.Autotracking;

namespace TrackOMatic.Services;

/// <summary>
/// Defines the contract for autotracking services that monitor emulator memory
/// and track item collection, region changes, and game state in real time.
/// </summary>
public interface IAutotrackerService : IDisposable
{
    #region Event Callbacks

    /// <summary>
    /// Raised when the current region changes or region lighting needs to be updated.
    /// </summary>
    event EventHandler<AutotrackerRegionEventArgs>? RegionLightingChanged;

    /// <summary>
    /// Raised when the current song/music changes.
    /// </summary>
    event EventHandler<AutotrackerSongEventArgs>? SongChanged;

    /// <summary>
    /// Raised when the amount of items needed to unlock the next hint pack changes.
    /// </summary>
    event EventHandler<AutotrackerHintEventArgs>? HintProgressUpdated;

    /// <summary>
    /// Raised when the collectible item used to unlock hint packs changes.
    /// </summary>
    event EventHandler<AutotrackerProgHintItemEventArgs>? ProgHintItemUpdated;
    #endregion

    #region State Properties

    /// <summary>
    /// Gets the list of autotracked checks.
    /// </summary>
    IReadOnlyList<AutotrackedCheck> Checks { get; }

    /// <summary>
    /// Gets a dictionary tracking which items have already been processed.
    /// </summary>
    IReadOnlyDictionary<ItemName, bool> TrackedAlready { get; }

    /// <summary>
    /// Gets the starting items for this randomizer seed.
    /// </summary>
    IReadOnlyDictionary<ItemName, RegionName> StartingItems { get; }

    /// <summary>
    /// Gets the game verification info used to verify emulator compatibility.
    /// </summary>
    GameVerificationInfo? GameVerificationInfo { get; }

    /// <summary>
    /// Gets the current region the player is in.
    /// </summary>
    RegionName CurrentRegion { get; }

    /// <summary>
    /// Gets the name of the current song/game the song is from.
    /// </summary>
    string CurrentSongGame { get; }

    /// <summary>
    /// Gets the name of the current song.
    /// </summary>
    string CurrentSongName { get; }

    /// <summary>
    /// Gets the major version of the randomizer used for this seed.
    /// </summary>
    int RandomizerVersion { get; }

    /// <summary>
    /// Gets the minor/sub version of the randomizer used for this seed.
    /// </summary>
    int RandomizerSubVersion { get; }
    #endregion

    #region Service Methods
    /// <summary>
    /// Resets the autotracker to its initial state and detaches from any emulator.
    /// </summary>
    void Reset();

    /// <summary>
    /// Resets the collection state but keeps the current emulator attachment.
    /// </summary>
    void ResetChecks();

    /// <summary>
    /// Sets the starting items for this seed, excluding them from tracking.
    /// </summary>
    /// <param name="newItems">Dictionary mapping item names to their starting regions.</param>
    void SetStartingItems(Dictionary<ItemName, RegionName> newItems);

    /// <summary>
    /// Marks that a spoiler log has been loaded for this session.
    /// </summary>
    /// <param name="fileName">The name of the spoiler file loaded.</param>
    void SetSpoilerLoaded(string fileName);

    /// <summary>
    /// Updates the hint pack collectible item to what we need to reveal hints.
    /// </summary>
    void UpdateProgHintItem();

    /// <summary>
    /// Updates the hint pack progress tracking based on current item counts.
    /// </summary>
    void UpdateAmountToNextHint();

    /// <summary>
    /// Manually process a saved item (used when loading from data files).
    /// </summary>
    /// <param name="item">The item to process.</param>
    void ProcessSavedItem(ItemName item);

    /// <summary>
    /// Checks if a specific item has already been tracked.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item was already tracked; false otherwise.</returns>
    bool ItemWasTracked(ItemName item);

    /// <summary>
    /// Starts the autotracking service, beginning emulator polling and item tracking.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the autotracking service gracefully.
    /// </summary>
    void Stop();

    /// <summary>
    /// Shuts down the autotracking service and releases all resources.
    /// </summary>
    void Shutdown();
    #endregion
}
