using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace TrackOMatic
{
    public static class AttachToEmulator
    {
        private static Process FindProcess(string name)
        {
            Process target;
            try
            {
                target = Process.GetProcessesByName(name)[0];
            }
            catch (Exception) { return null; }
            return target;
        }
        private static AttachedProcessInfo AttachToProject64(Process target, GameVerificationInfo verificationInfo)
        {
            //Project64 versions can differ in memory so try different offsets
            for (uint potentialOffset = 0xDFD00000; potentialOffset < 0xE01FFFFF; potentialOffset += 16)
            {
                if (Memory.ReadInt32(target, potentialOffset + verificationInfo.TargetAddress) == verificationInfo.TargetValue)
                {
                    Console.WriteLine(potentialOffset + verificationInfo.TargetAddress);
                    Console.WriteLine(potentialOffset);
                    return new AttachedProcessInfo(target, potentialOffset);
                }
            }
            return null;
        }


        private static AttachedProcessInfo AttachToBizhawk(Process target, GameVerificationInfo verificationInfo)
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

            if (addressDLL == 0) addressDLL = 2024407040;

            for (uint potentialOffset = 0x5A000; potentialOffset < 0x5658DF; potentialOffset += 16)
            {
                var addressToCheck = (uint)(potentialOffset + verificationInfo.TargetAddress);
                if (Memory.ReadInt16(target, addressToCheck) == verificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, (uint)(addressDLL +  potentialOffset));
                }
            }

            return null;
        }


        private static AttachedProcessInfo AttachToRMG(Process target, GameVerificationInfo gameVerificationInfo)
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

            if (addressDLL == 0) return null;

            for (uint potOff = 0x29C15D8; potOff < 0x2FC15D8; potOff += 16)
            {
                ulong romAddrStart = addressDLL + potOff;
                ulong readAddress = Memory.ReadInt64(target, romAddrStart);
                // use this previously read address to find the game verification data
                var testValue = Memory.ReadInt32(target, (readAddress + 0x80000000 + gameVerificationInfo.TargetAddress));
                if ((testValue & 0xffffffff) == gameVerificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, readAddress + 0x80000000);
                }
            }
            return null;
        }

        private static AttachedProcessInfo AttachToRetroarch(Process target, GameVerificationInfo gameVerificationInfo)
        {
            ulong addressDLL = 0;
            bool mupen = false;
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
                    mupen = true;
                    break;
                }
            }

            if (addressDLL == 0) return null;

            for (uint potOff = 0; potOff < 0xFFFFFF; potOff += 4)
            {
                ulong romAddrStart = addressDLL + potOff;
                ulong readAddress = Memory.ReadInt64(target, (romAddrStart));
                var testValue = Memory.ReadInt32(target, (readAddress + gameVerificationInfo.TargetAddress));
                if (testValue == gameVerificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, readAddress);
                }
            }
            return null;
        }

        public static AttachedProcessInfo Attach(GameVerificationInfo verificationInfo)
        {
            var emu_to_function_call = new Dictionary<string, Func<Process, GameVerificationInfo, AttachedProcessInfo>>()
            {
                {"project64", AttachToProject64 },
                {"rmg", AttachToRMG },
                {"retroarch", AttachToRetroarch }
                //something weird keeps this from working?
                //{"emuhawk", AttachToBizhawk }
            };
            foreach (var entry in emu_to_function_call)
            {
                var process = FindProcess(entry.Key);
                if (process != null)
                {
                    return entry.Value(process, verificationInfo);
                }
            }
            return null;
        }
    }
}
