namespace TrackOMatic.Logic.Models.Autotracking;

/// <summary>
/// Abstracts attaching to an emulator process and locating the game in memory.
/// Platform-specific implementations handle different emulators and OS mechanisms.
/// </summary>
public interface IEmulatorAttacher
{
    /// <summary>
    /// Attempts to find and attach to any emulator instances that are compatible with DK 64 Randomizer.
    /// </summary>
    /// <remarks>Each OS will have to handle things differently.</remarks>
    /// <param name="verificationInfo"></param>
    /// <returns></returns>
    AttachedEmulatorInfo? Attach(GameVerificationInfo verificationInfo);

    /// <summary>
    /// Gets a list of all currently running emulator processes.
    /// Useful for diagnostics and manual selection.
    /// </summary>
    /// <returns>List of tuples: (ProcessId, EmulatorName)</returns>
    List<(int ProcessId, string EmulatorName)> GetAvailableEmulators();

    /// <summary>
    /// Checks if a process is still running (for detecting when emulator is closed).
    /// </summary>
    /// <param name="processId">Process ID to check</param>
    /// <returns>true if process is running; false otherwise</returns>
    bool IsEmulatorStillRunning(int processId);
}
