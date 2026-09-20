using System.Diagnostics;
using System.Runtime.InteropServices;

using TrackOMatic.Logic.Models.Autotracking;

namespace TrackOMatic.AutoTracking.Windows;

/// <summary>
/// Windows implementation of IProcessMemoryReader.
/// Uses kernel32.dll and psapi.dll P/Invoke to access process memory.
/// Opens a fresh process handle for each read to ensure access rights are current.
/// </summary>
public partial class WindowsProcessMemoryReader : IProcessMemoryReader
{
    private const int PROCESS_WM_READ = 0x0010;
    private const int ERROR_PARTIAL_COPY = 299;

    private int? _lastFatalErrorProcessId = null;

    [LibraryImport("kernel32.dll")]
    private static partial IntPtr OpenProcess(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial int ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] buffer, uint size, IntPtr lpNumberOfBytesRead);

    [LibraryImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnumProcessModules(IntPtr hProcess, out IntPtr lphModule, uint cb, [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(IntPtr hObject);


    public IntPtr OpenHandle(int processId) => OpenProcess(PROCESS_WM_READ, false, processId);

    public void CloseHandleSafe(IntPtr handle)
    {
        if (handle != IntPtr.Zero)
        {
            CloseHandle(handle);
        }
    }

    public int FindProcessByName(string processName)
    {
        try
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                int id = processes[0].Id;
                foreach (var p in processes)
                {
                    p.Dispose();
                }
                return id;
            }
        }
        catch (Exception)
        {
            // Process not found
        }

        return -1;
    }

    public bool IsProcessRunning(int processId)
    {
        try
        {
            Process.GetProcessById(processId);
            return true;
        }
        catch (ArgumentException)
        {
            // Process not running
            return false;
        }
    }

    public string? GetProcessVersion(int processId)
    {
        try
        {
            var process = Process.GetProcessById(processId);
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(process.MainModule?.FileName ?? "");
            return fileVersionInfo.FileVersion;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public byte[] ReadBytes(int processId, uint address, uint length)
    {
        IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, processId);
        if (processHandle == IntPtr.Zero)
        {
            return new byte[length];
        }

        IntPtr ptrBytesRead = new(0);
        byte[] buffer = new byte[length];
        int result = ReadProcessMemory(processHandle, new UIntPtr(address), buffer, length, ptrBytesRead);

        if (result == 0)
        {
            int errorCode = Marshal.GetLastWin32Error();
            if (errorCode == ERROR_PARTIAL_COPY)
            {
                _lastFatalErrorProcessId = processId;
            }
        }

        CloseHandle(processHandle);
        return buffer;
    }

    public byte[] ReadBytes64(int processId, ulong address, uint length)
    {
        IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, processId);
        if (processHandle == IntPtr.Zero)
        {
            return new byte[length];
        }

        IntPtr ptrBytesRead = new(0);
        byte[] buffer = new byte[length];
        int result = ReadProcessMemory(processHandle, new UIntPtr(address), buffer, length, ptrBytesRead);

        if (result == 0)
        {
            int errorCode = Marshal.GetLastWin32Error();
            if (errorCode == ERROR_PARTIAL_COPY)
            {
                _lastFatalErrorProcessId = processId;
            }
        }

        CloseHandle(processHandle);
        return buffer;
    }

    public int ReadInt8(int processId, uint address)
    {
        var bytes = ReadBytes(processId, address, 1);
        return bytes.Length > 0 ? bytes[0] : 0;
    }

    public int ReadInt16(int processId, uint address)
    {
        var bytes = ReadBytes(processId, address, 2);
        return bytes.Length >= 2 ? BitConverter.ToInt16(bytes, 0) : 0;
    }

    public int ReadInt32(int processId, uint address)
    {
        var bytes = ReadBytes(processId, address, 4);
        return bytes.Length >= 4 ? BitConverter.ToInt32(bytes, 0) : 0;
    }

    public long ReadInt64(int processId, ulong address)
    {
        var bytes = ReadBytes64(processId, address, 8);
        return bytes.Length >= 8 ? BitConverter.ToInt64(bytes, 0) : 0;
    }

    /// <summary>
    /// Check if the last read against this process ID encountered a fatal error (error 299 - process likely dead).
    /// </summary>
    public bool DidLastReadFailWithFatalError(int processId)
    {
        return _lastFatalErrorProcessId == processId;
    }

    /// <summary>
    /// Clears the fatal error flag for a process ID.
    /// </summary>
    public void ClearFatalErrorFlag(int processId)
    {
        if (_lastFatalErrorProcessId == processId)
        {
            _lastFatalErrorProcessId = null;
        }
    }

    /// <summary>
    /// Fixes 8-bit read address alignment (64-bit).
    /// N64 emulators store bytes in a special byte-swapped format.
    /// </summary>
    public ulong Int8AddrFix(ulong addr) => addr ^ 3;

    /// <summary>
    /// Fixes 16-bit read address alignment (64-bit).
    /// N64 emulators store 16-bit values in a special byte-swapped format.
    /// </summary>
    public ulong Int16AddrFix(ulong addr) => addr ^ 2;
}
