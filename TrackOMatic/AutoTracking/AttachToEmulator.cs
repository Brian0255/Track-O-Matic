﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Management;
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
            string filePath = target.MainModule.FileName;
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            uint lowerBound = 0xDFD00000;
            uint upperBound = 0xE01F0000;
            if(versionInfo.FileMajorPart >= 4 && versionInfo.ProductPrivatePart > 5758)
            {
                lowerBound = 0xFDD00000;
                upperBound = 0xFE1FFFFF;
            }
            for (uint potentialOffset = lowerBound; potentialOffset < upperBound; potentialOffset += 1)
            {
                if (Memory.ReadInt32(target, potentialOffset + verificationInfo.TargetAddress) == verificationInfo.TargetValue)
                {
                    Console.WriteLine(potentialOffset + verificationInfo.TargetAddress);
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

        private static AttachedProcessInfo RunRetroarchScan(Process target, GameVerificationInfo gameVerificationInfo, ulong addressDLL, uint lowerBound, uint upperBound, uint step, bool isMupen)
        {
            for (uint potOff = lowerBound; potOff < upperBound; potOff += step)
            {
                ulong romAddrStart = addressDLL + potOff;
                ulong readAddress = Memory.ReadInt64(target, romAddrStart);
                if (isMupen)
                {
                    readAddress = Memory.ReadInt64(target, (addressDLL + potOff + 4) & readAddress);
                    readAddress += 0x80000000;
                }

                var testValue = Memory.ReadInt32(target, (readAddress + gameVerificationInfo.TargetAddress));
                if ((testValue & 0xFFFFFFFF) == gameVerificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, readAddress);
                }
            }
            return null;
        }

        private static AttachedProcessInfo AttachToRetroarch(Process target, GameVerificationInfo gameVerificationInfo)
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

            AttachedProcessInfo processInfo;
            if (addressDLL == 0) return null;

            var parentProcessName = GetParentProcessName(target);

            if (parentProcessName != null && parentProcessName == "parallel-launcher") {
                processInfo = RunRetroarchScan(target, gameVerificationInfo, addressDLL, 0x845000, 0xD56000, 16, isMupen);
            }
            else { 
                //forcibly set isMupen to false even if it isn't just because retroarch is jank or something
                processInfo = RunRetroarchScan(target, gameVerificationInfo, addressDLL, 0x000000, 0xFFFFFF, 4, false);
            }

            return processInfo;
        }
        public static AttachedProcessInfo Attach(GameVerificationInfo verificationInfo)
        {
            var emu_to_function_call = new Dictionary<string, Func<Process, GameVerificationInfo, AttachedProcessInfo>>()
            {
                {"project64", AttachToProject64 },
                {"rmg", AttachToRMG },
                {"retroarch", AttachToRetroarch }
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
