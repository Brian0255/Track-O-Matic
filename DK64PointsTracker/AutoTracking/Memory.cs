using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DK64PointsTracker
{
    public static class Memory
    {
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, Byte[] buffer, UInt32 size, IntPtr lpNumberOfBytesRead);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModules(IntPtr hProcess, [Out] IntPtr lphModule, UInt32 cb, [MarshalAs(UnmanagedType.U4)] out UInt32 lpcbNeeded);

        // functions to read memory at certain lengths


        //32-bit functions
        public static int ReadInt8(Process P, uint memAdr)
        {
            return ReadBytes(P, memAdr, 1)[0];
        }
        public static int ReadInt16(Process P, uint memAdr)
        {
            return BitConverter.ToInt16(ReadBytes(P, memAdr, 2), 0);
        }
        public static int ReadInt32(Process P, uint memAdr)
        {
            return BitConverter.ToInt32(ReadBytes(P, memAdr, 4), 0);
        }
        public static UInt32 ReadUInt32(Process P, uint memAdr)
        {
            IntPtr ptrBytesRead = new IntPtr(0);
            byte[] buffer = new byte[3];
            ReadProcessMemory(P.Handle, (UIntPtr)memAdr, buffer, 4, ptrBytesRead);
            return BitConverter.ToUInt32(ReadBytes(P, memAdr, 4), 0);
        }
        private static byte[] ReadBytes(Process P, uint memAdr, uint bytesToRead)
        {
            IntPtr ptrBytesRead = new IntPtr(0);
            byte[] buffer = new byte[bytesToRead];
            ReadProcessMemory(P.Handle, new UIntPtr(memAdr), buffer, bytesToRead, ptrBytesRead);
            return buffer;
        }

        //64-bit functions
        public static int ReadInt8(Process P, ulong memAdr)
        {
            return ReadBytes(P, memAdr, 1)[0];
        }

        public static int ReadInt16(Process P, ulong memAdr)
        {
            return BitConverter.ToInt16(ReadBytes(P, memAdr, 2), 0);
        }

        public static int ReadInt32(Process P, ulong memAdr)
        {
            return BitConverter.ToInt32(ReadBytes(P, memAdr, 4), 0);
        }

        public static ulong ReadInt64(Process P, ulong memAdr)
        {
            return BitConverter.ToUInt64(ReadBytes(P, memAdr, 8), 0);
        }

        private static byte[] ReadBytes(Process P, ulong memAdr, uint bytesToRead)
        {
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, P.Id);
            IntPtr ptrBytesRead = new IntPtr(0);
            byte[] buffer = new byte[bytesToRead];
            ReadProcessMemory(processHandle, new UIntPtr(memAdr), buffer, bytesToRead, ptrBytesRead);
            return buffer;
        }

        // general functions that needed a home somewhere lol

        public static uint Int8AddrFix(uint addr)
        {
            return (addr % 4) switch
            {
                0 => addr + 3,
                1 => addr + 1,
                2 => addr - 1,
                3 => addr - 3,
                _ => addr,
            };
        }


        public static uint Int16AddrFix(uint addr)
        {
            return (addr % 4) switch
            {
                2 or 3 => addr - 2,
                0 or 1 => addr + 2,
                _ => addr,
            };
        }

        public static ulong Int8AddrFix(ulong addr)
        {
            return (addr % 4) switch
            {
                0 => addr + 3,
                1 => addr + 1,
                2 => addr - 1,
                3 => addr - 3,
                _ => addr,
            };
        }


        public static ulong Int16AddrFix(ulong addr)
        {
            return (addr % 4) switch
            {
                2 or 3 => addr - 2,
                0 or 1 => addr + 2,
                _ => addr,
            };
        }

    }
}