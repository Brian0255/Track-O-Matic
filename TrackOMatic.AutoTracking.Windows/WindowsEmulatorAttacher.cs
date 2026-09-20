using System.Diagnostics;
using System.Management;

using TrackOMatic.Logic.Models.Autotracking;

namespace TrackOMatic.AutoTracking.Windows;

/// <summary>
/// Windows implementation of IEmulatorAttacher.
/// Supports Project64, Bizhawk (with Mupen64Plus), and Retroarch emulators.
/// Wraps the existing AttachToEmulator.cs logic using the IProcessMemoryReader abstraction.
/// </summary>
public class WindowsEmulatorAttacher(IProcessMemoryReader memoryReader) : IEmulatorAttacher
{
    private static readonly string _project64Id = "project64";
    private static readonly string _rmgId = "rmg";
    private static readonly string _retroarchId = "retroarch";

    private readonly IProcessMemoryReader _memoryReader = memoryReader;

    public AttachedEmulatorInfo? Attach(GameVerificationInfo verificationInfo)
    {
        var emu_to_function_call = new Dictionary<string, Func<Process, IntPtr, GameVerificationInfo, AttachedEmulatorInfo?>>()
        {
            {_project64Id, AttachToProject64 },
            {_rmgId, AttachToRMG },
            {_retroarchId, AttachToRetroarch }
        };
        foreach (var entry in emu_to_function_call)
        {
            var process = FindProcess(entry.Key);
            if (process == null)
            {
                continue;
            }

            IntPtr handle = _memoryReader.OpenHandle(process.Id);
            if (handle == IntPtr.Zero)
            {
                continue;
            }

            var result = entry.Value(process, handle, verificationInfo);
            if (result == null)
            {
                _memoryReader.CloseHandleSafe(handle);
            }
            return result;
        }
        return null;
    }

    private AttachedEmulatorInfo? AttachToProject64(Process target, IntPtr handle, GameVerificationInfo verificationInfo)
    {
        string filePath = target.MainModule!.FileName;
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);
        uint lowerBound = 0xDFD00000;
        uint upperBound = 0xE01F0000;
        if (versionInfo.FileMajorPart >= 4 && versionInfo.ProductPrivatePart > 5758)
        {
            lowerBound = 0xFDD00000;
            upperBound = 0xFE1FFFFF;
        }
        for (uint potentialOffset = lowerBound; potentialOffset < upperBound; potentialOffset += 1)
        {
            if (_memoryReader.ReadInt32(target.Id, potentialOffset + verificationInfo.TargetAddress) == verificationInfo.TargetValue)
            {
                Console.WriteLine(potentialOffset + verificationInfo.TargetAddress);
                return new AttachedEmulatorInfo(target.Id, _project64Id, potentialOffset, handle);
            }
        }
        return null;
    }

    private AttachedEmulatorInfo? AttachToRMG(Process target, IntPtr handle, GameVerificationInfo gameVerificationInfo)
    {
        ulong addressDLL = 0;
        foreach (ProcessModule mo in target.Modules)
        {
            if (mo.ModuleName.Equals("mupen64plus.dll", StringComparison.CurrentCultureIgnoreCase))
            {
                addressDLL = (ulong)mo.BaseAddress.ToInt64();
                break;
            }
        }

        if (addressDLL == 0)
        {
            return null;
        }

        for (uint potOff = 0x29C15D8; potOff < 0x2FC15D8; potOff += 16)
        {
            ulong romAddrStart = addressDLL + potOff;
            ulong readAddress = (ulong)_memoryReader.ReadInt64(target.Id, romAddrStart);
            // use this previously read address to find the game verification data
            var testValue = _memoryReader.ReadInt32(target.Id, (uint)(readAddress + 0x80000000 + gameVerificationInfo.TargetAddress));
            if ((testValue & 0xffffffff) == gameVerificationInfo.TargetValue)
            {
                return new AttachedEmulatorInfo(target.Id, _rmgId, readAddress + 0x80000000, handle);
            }
        }
        return null;
    }

    private AttachedEmulatorInfo? AttachToRetroarch(Process target, IntPtr handle, GameVerificationInfo gameVerificationInfo)
    {
        ulong addressDLL = 0;
        bool isMupen = false;
        foreach (ProcessModule mo in target.Modules)
        {
            if (mo is null)
            {
                continue;
            }
            if (mo.ModuleName.Equals("parallel_n64_next_libretro.dll", StringComparison.CurrentCultureIgnoreCase))
            {
                addressDLL = (ulong)mo.BaseAddress.ToInt64();
                break;
            }
            else if (mo.ModuleName.Equals("mupen64plus_next_libretro.dll", StringComparison.CurrentCultureIgnoreCase))
            {
                addressDLL = (ulong)mo.BaseAddress.ToInt64();
                isMupen = true;
                break;
            }
        }

        AttachedEmulatorInfo? processInfo;
        if (addressDLL == 0)
        {
            return null;
        }

        var parentProcessName = GetParentProcessName(target.Id);

        if (parentProcessName != null && parentProcessName == "parallel-launcher")
        {
            processInfo = RunRetroarchScan(target, handle, gameVerificationInfo, addressDLL, 0x1400000, 0x1800000, 16, isMupen);
        }
        else
        {
            //forcibly set isMupen to false even if it isn't just because retroarch is jank or something
            processInfo = RunRetroarchScan(target, handle, gameVerificationInfo, addressDLL, 0x000000, 0xFFFFFF, 4, false);
        }

        return processInfo;
    }

    /*
    public AttachedEmulatorInfo? AttachToBizhawk(GameVerificationInfo verificationInfo)
    {
        int bizhawkId = _memoryReader.FindProcessByName("EmuHawk");
        if (bizhawkId == -1)
        {
            return null;
        }

        // Bizhawk loads Mupen64Plus as a DLL; find its base address
        long addressDLL = 0;
        try
        {
            var process = Process.GetProcessById(bizhawkId);
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.Equals("mupen64plus.dll", StringComparison.CurrentCultureIgnoreCase))
                {
                    addressDLL = module.BaseAddress.ToInt64();
                    break;
                }
            }
        }
        catch (Exception)
        {
            return null;
        }

        if (addressDLL == 0)
        {
            addressDLL = 2024407040; // Fallback default
        }

        // Search within Bizhawk's memory space
        for (uint potentialOffset = 0x5A000; potentialOffset < 0x5658DF; potentialOffset += 16)
        {
            var addressToCheck = (uint)(potentialOffset + verificationInfo.TargetAddress);
            int readValue = _memoryReader.ReadInt16(bizhawkId, addressToCheck);
            if (readValue == verificationInfo.TargetValue)
            {
                return new AttachedEmulatorInfo(bizhawkId, "Bizhawk", (ulong)(addressDLL + potentialOffset));
            }
        }

        return null;
    }
    */

    public List<(int ProcessId, string EmulatorName)> GetAvailableEmulators()
    {
        var available = new List<(int, string)>();

        int projectId = _memoryReader.FindProcessByName("project64");
        if (projectId != -1)
        {
            available.Add((projectId, "Project64"));
        }

        int bizhawkId = _memoryReader.FindProcessByName("EmuHawk");
        if (bizhawkId != -1)
        {
            available.Add((bizhawkId, "Bizhawk"));
        }

        int retroarchId = _memoryReader.FindProcessByName("retroarch");
        if (retroarchId != -1)
        {
            available.Add((retroarchId, "Retroarch"));
        }

        int rmgId = _memoryReader.FindProcessByName("rmg");
        if (rmgId != -1)
        {
            available.Add((rmgId, "RMG"));
        }

        return available;
    }

    public bool IsEmulatorStillRunning(int processId)
    {
        return _memoryReader.IsProcessRunning(processId);
    }

    // Private helpers

    private AttachedEmulatorInfo? RunRetroarchScan(Process target, IntPtr handle, GameVerificationInfo gameVerificationInfo, ulong addressDLL, uint lowerBound, uint upperBound, uint step, bool isMupen)
    {
        for (uint potOff = lowerBound; potOff < upperBound; potOff += step)
        {
            ulong romAddrStart = addressDLL + potOff;
            ulong readAddress = (ulong)_memoryReader.ReadInt64(target.Id, romAddrStart);
            if (isMupen)
            {
                readAddress = (ulong)_memoryReader.ReadInt64(target.Id, (addressDLL + potOff + 4) & readAddress);
                readAddress += 0x80000000;
            }

            var testValue = _memoryReader.ReadInt32(target.Id, (uint)(readAddress + gameVerificationInfo.TargetAddress));
            if ((testValue & 0xFFFFFFFF) == gameVerificationInfo.TargetValue)
            {
                return new AttachedEmulatorInfo(target.Id, _retroarchId, readAddress, handle);
            }
        }
        return null;
    }

    private static string? GetParentProcessName(int processId)
    {
        try
        {
            var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", processId);
            var search = new ManagementObjectSearcher("root\\CIMV2", query);
            var results = search.Get().GetEnumerator();
            if (results.MoveNext())
            {
                var queryObj = results.Current;
                var parentId = (uint)queryObj["ParentProcessId"];
                var parent = Process.GetProcessById((int)parentId);
                return parent.ProcessName;
            }
        }
        catch (Exception)
        {
            // WMI not available or other error
        }

        return null;
    }

    private static Process? FindProcess(string name)
    {
        try
        {
            var list = Process.GetProcessesByName(name);
            return list.Length > 0 ? list[0] : null;
        }
        catch (InvalidOperationException) {
            return null;
        }
    }
}
