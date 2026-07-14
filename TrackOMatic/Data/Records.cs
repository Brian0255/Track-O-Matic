using System.Diagnostics;

namespace TrackOMatic
{
    public record AttachedProcessInfo(Process Process, IntPtr Handle, ulong StartAddress);
    public record BLockerInfo(int item, int cost);
}
