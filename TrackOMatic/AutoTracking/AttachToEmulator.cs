using System.Diagnostics;
using System.Management;

namespace TrackOMatic
{
    public static class AttachToEmulator
    {
        private static Process? FindProcess(string name)
        {
            try
            {
                var processes = Process.GetProcessesByName(name);
                return processes?.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static AttachedProcessInfo? AttachToProject64(Process target, IntPtr handle, GameVerificationInfo verificationInfo)
        {
            if (target.MainModule == null)
            {
                return null;
            }
            string filePath = target.MainModule.FileName;
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
                if (Memory.ReadInt32(handle, potentialOffset + verificationInfo.TargetAddress) == verificationInfo.TargetValue)
                {
                    Console.WriteLine(potentialOffset + verificationInfo.TargetAddress);
                    return new AttachedProcessInfo(target, handle, potentialOffset);
                }
            }
            return null;
        }

        private static AttachedProcessInfo? AttachToBizhawk(Process target, IntPtr handle, GameVerificationInfo verificationInfo)
        {
            Int64 addressDLL = 0;
            foreach (ProcessModule mo in target.Modules)
            {
                if (mo.ModuleName.ToLower() == "mupen64plus.dll")
                {
                    addressDLL = mo.BaseAddress.ToInt64();
                    break;
                }
            }

            if (addressDLL == 0)
            {
                addressDLL = 2024407040;
            }

            for (uint potentialOffset = 0x5A000; potentialOffset < 0x5658DF; potentialOffset += 16)
            {
                var addressToCheck = (uint)(potentialOffset + verificationInfo.TargetAddress);
                if (Memory.ReadInt16(handle, addressToCheck) == verificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, handle, (uint)(addressDLL + potentialOffset));
                }
            }

            return null;
        }

        private static AttachedProcessInfo? AttachToRMG(Process target, IntPtr handle, GameVerificationInfo gameVerificationInfo)
        {
            ulong addressDLL = 0;
            foreach (ProcessModule mo in target.Modules)
            {
                if (mo.ModuleName.ToLower() == "mupen64plus.dll")
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
                ulong readAddress = Memory.ReadInt64(handle, romAddrStart);
                // use this previously read address to find the game verification data
                var testValue = Memory.ReadInt32(handle, (readAddress + 0x80000000 + gameVerificationInfo.TargetAddress));
                if ((testValue & 0xffffffff) == gameVerificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, handle, readAddress + 0x80000000);
                }
            }
            return null;
        }

        private static string GetParentProcessName(Process process)
        {
            var myId = process.Id;
            var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
            var search = new ManagementObjectSearcher("root\\CIMV2", query);
            var results = search.Get().GetEnumerator();
            results.MoveNext();
            var queryObj = results.Current;
            var parentId = (uint)queryObj["ParentProcessId"];
            var parent = Process.GetProcessById((int)parentId);
            return parent.ProcessName;
        }

        private static AttachedProcessInfo? RunRetroarchScan(Process target, IntPtr handle, GameVerificationInfo gameVerificationInfo, ulong addressDLL, uint lowerBound, uint upperBound, uint step, bool isMupen)
        {
            for (uint potOff = lowerBound; potOff < upperBound; potOff += step)
            {
                ulong romAddrStart = addressDLL + potOff;
                ulong readAddress = Memory.ReadInt64(handle, romAddrStart);
                if (isMupen)
                {
                    readAddress = Memory.ReadInt64(handle, (addressDLL + potOff + 4) & readAddress);
                    readAddress += 0x80000000;
                }

                var testValue = Memory.ReadInt32(handle, (readAddress + gameVerificationInfo.TargetAddress));
                if ((testValue & 0xFFFFFFFF) == gameVerificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, handle, readAddress);
                }
            }
            return null;
        }

        private static AttachedProcessInfo? AttachToRetroarch(Process target, IntPtr handle, GameVerificationInfo gameVerificationInfo)
        {
            ulong addressDLL = 0;
            bool isMupen = false;
            foreach (ProcessModule mo in target.Modules)
            {
                if (mo.ModuleName.ToLower() == "parallel_n64_next_libretro.dll")
                {
                    addressDLL = (ulong)mo.BaseAddress.ToInt64();
                    break;
                }
                else if (mo.ModuleName.ToLower() == "mupen64plus_next_libretro.dll")
                {
                    addressDLL = (ulong)mo.BaseAddress.ToInt64();
                    isMupen = true;
                    break;
                }
            }

            AttachedProcessInfo? processInfo;
            if (addressDLL == 0)
            {
                return null;
            }

            var parentProcessName = GetParentProcessName(target);

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

        public static AttachedProcessInfo? Attach(GameVerificationInfo verificationInfo)
        {
            var emu_to_function_call = new Dictionary<string, Func<Process, IntPtr, GameVerificationInfo, AttachedProcessInfo?>>()
            {
                {"project64", AttachToProject64 },
                {"rmg", AttachToRMG },
                {"retroarch", AttachToRetroarch }
            };
            foreach (var entry in emu_to_function_call)
            {
                var process = FindProcess(entry.Key);
                if (process == null)
                {
                    continue;
                }

                IntPtr handle = Memory.OpenHandle(process);
                if (handle == IntPtr.Zero)
                {
                    continue;
                }

                var result = entry.Value(process, handle, verificationInfo);
                if (result == null)
                {
                    Memory.CloseHandleSafe(handle);
                }
                return result;
            }
            return null;
        }
    }
}
