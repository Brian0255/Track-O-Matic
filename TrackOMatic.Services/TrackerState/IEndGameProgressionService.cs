using TrackOMatic.Logic.Events;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Manages end-game progression state (Kongs for Blast-O-Matic and Final Boss Gauntlet).
/// </summary>
public interface IEndGameProgressionService
{
    #region Blast-O-Matic Domain
    /// <summary>
    /// Get the list of Kongs required for disabling the Blast-O-Matic in Helm.
    /// </summary>
    /// <returns></returns>
    IList<int> GetHelmKongs();

    /// <summary>
    /// Set the Kongs needed for disabling the Blast-O-Matic in Helm.
    /// </summary>
    /// <param name="kongs">The list of Kongs required. This must have exactly 5 elements.</param>
    void SetHelmKongs(IEnumerable<int> kongs);

    #endregion

    #region Final Boss Gauntlet Domain

    /// <summary>
    /// Get the list of Kongs required for defeating the Final Boss Gauntlet.
    /// </summary>
    /// <returns></returns>
    IList<int> GetBossKongs();

    /// <summary>
    /// Set the Kongs needed for defeating the Final Boss Gauntlet.
    /// </summary>
    /// <param name="kongs">The list of phases required. This must have exactly 5 elements.</param>
    void SetBossKongs(IEnumerable<int> kongs);
    #endregion

    #region Event Handlers

    event EventHandler<BlastStateChangedEventArgs>? BlastStateChanged;

    event EventHandler<GauntletStateChangedEventArgs>? GauntletStateChanged;

    #endregion
}
