namespace TrackOMatic.Logic.Models.Autotracking;

/// <summary>
/// Information about a successfully attached emulator process.
/// </summary>
public record AttachedEmulatorInfo(
    int ProcessId,
    string EmulatorName,
    ulong BaseAddress,
    IntPtr ProcessHandle
);
