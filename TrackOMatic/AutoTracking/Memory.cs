using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TrackOMatic
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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
        public static IntPtr OpenHandle(Process p) => OpenProcess(PROCESS_WM_READ, false, p.Id);
        public static void CloseHandleSafe(IntPtr handle)
        {
            if (handle != IntPtr.Zero) CloseHandle(handle);
        }
        private static byte[] ReadBytes(IntPtr handle, ulong memAdr, uint bytesToRead)
        {
            var buffer = new byte[bytesToRead];
            ReadProcessMemory(handle, new UIntPtr(memAdr), buffer, bytesToRead, IntPtr.Zero);
            return buffer;
        }
        public static int ReadInt8(IntPtr handle, ulong memAdr) => ReadBytes(handle, memAdr, 1)[0];
        public static int ReadInt16(IntPtr handle, ulong memAdr) => BitConverter.ToInt16(ReadBytes(handle, memAdr, 2), 0);
        public static int ReadInt32(IntPtr handle, ulong memAdr) => BitConverter.ToInt32(ReadBytes(handle, memAdr, 4), 0);
        public static ulong ReadInt64(IntPtr handle, ulong memAdr) => BitConverter.ToUInt64(ReadBytes(handle, memAdr, 8), 0);
        public static ulong Int8AddrFix(ulong addr) => addr ^ 3;
        public static ulong Int16AddrFix(ulong addr) => addr ^ 2;
    }
}