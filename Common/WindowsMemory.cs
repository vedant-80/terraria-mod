using System;
using System.Runtime.InteropServices;

namespace NewMod.Common;

internal static class WindowsMemory
{
    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryStatus
    {
        public uint Length, Load;
        public ulong TotalPhysical, AvailablePhysical, TotalPageFile, AvailablePageFile, TotalVirtual, AvailableVirtual, AvailableExtendedVirtual;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatus status);

    public static string Describe()
    {
        if (!OperatingSystem.IsWindows()) return ConversationRuntime.Text("MemoryUnknown");
        var status = new MemoryStatus { Length = (uint)Marshal.SizeOf<MemoryStatus>() };
        if (!GlobalMemoryStatusEx(ref status)) return ConversationRuntime.Text("MemoryUnknown");
        const double gib = 1024d * 1024 * 1024;
        string reading = ConversationRuntime.Text("MemoryReading", (status.AvailablePhysical / gib).ToString("0.0"), (status.TotalPhysical / gib).ToString("0.0"));
        return reading + " " + ConversationRuntime.Text(status.AvailablePhysical < 2 * gib ? "MemoryLow" : "MemoryAdvisory");
    }
}
