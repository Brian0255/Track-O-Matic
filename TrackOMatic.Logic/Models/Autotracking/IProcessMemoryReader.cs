namespace TrackOMatic.Logic.Models.Autotracking;

/// <summary>
/// Abstracts reading memory from a running process.
/// Platform-specific implementations handle Windows P/Invoke, Linux /proc, etc.
/// </summary>
public interface IProcessMemoryReader
{
    /// <summary>
    /// Opens a handle to the process with the given ID for reading memory.
    /// </summary>
    /// <param name="processId">Process ID to open handle for</param>
    /// <returns>Handle to the process; IntPtr.Zero if handle cannot be opened</returns>
    IntPtr OpenHandle(int processId);

    /// <summary>
    /// Closes a previously opened process handle safely.
    /// </summary>
    /// <param name="handle">Handle to the process to close</param>
    void CloseHandleSafe(IntPtr handle);

    /// <summary>
    /// Attempts to find a process by name.
    /// </summary>
    /// <param name="processName">Name of the process (without extension, e.g., "project64")</param>
    /// <returns>Process ID if found; -1 if not found</returns>
    int FindProcessByName(string processName);

    /// <summary>
    /// Checks if a process with the given ID is still running.
    /// </summary>
    /// <param name="processId">Process ID to check</param>
    /// <returns>true if process is running; false otherwise</returns>
    bool IsProcessRunning(int processId);

    /// <summary>
    /// Gets the file version of the process executable (for version checking).
    /// </summary>
    /// <param name="processId">Process ID</param>
    /// <returns>
    /// Version string in format "major.minor.patch.build" (e.g., "4.0.0.5758");
    /// returns null if version cannot be determined
    /// </returns>
    string? GetProcessVersion(int processId);

    /// <summary>
    /// Reads a sequence of bytes from process memory at the specified address.
    /// </summary>
    /// <param name="processId">Process ID to read from</param>
    /// <param name="address">Memory address (32-bit)</param>
    /// <param name="length">Number of bytes to read</param>
    /// <returns>Byte array containing the read data; empty array if read fails</returns>
    byte[] ReadBytes(int processId, uint address, uint length);

    /// <summary>
    /// Reads a sequence of bytes from process memory at the specified address (64-bit).
    /// </summary>
    /// <param name="processId">Process ID to read from</param>
    /// <param name="address">Memory address (64-bit)</param>
    /// <param name="length">Number of bytes to read</param>
    /// <returns>Byte array containing the read data; empty array if read fails</returns>
    byte[] ReadBytes64(int processId, ulong address, uint length);

    /// <summary>
    /// Reads a single byte from process memory.
    /// </summary>
    int ReadInt8(int processId, uint address);

    /// <summary>
    /// Reads a 16-bit integer from process memory.
    /// </summary>
    int ReadInt16(int processId, uint address);

    /// <summary>
    /// Reads a 32-bit integer from process memory.
    /// </summary>
    int ReadInt32(int processId, uint address);

    /// <summary>
    /// Reads a 64-bit integer from process memory (64-bit address).
    /// </summary>
    long ReadInt64(int processId, ulong address);

    /// <summary>
    /// Fixes 8-bit read address alignment (64-bit).
    /// N64 emulators store bytes in a special byte-swapped format.
    /// </summary>
    ulong Int8AddrFix(ulong addr);

    /// <summary>
    /// Fixes 16-bit read address alignment (64-bit).
    /// N64 emulators store 16-bit values in a special byte-swapped format.
    /// </summary>
    ulong Int16AddrFix(ulong addr);
}
