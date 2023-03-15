using System.Runtime.InteropServices;

namespace Protot.Core.Extensions;

internal static class RuntimeExtensions
{
    internal static string GetOsPlatformName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return  "windows";
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "linux";
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "macosx";
        }

        throw new Exception("Unknown Platform Version");
    }

    internal static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
    internal static string GetProcessArchitecture()
    {
        return RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}