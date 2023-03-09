namespace Protot.Extensions;

internal static class FileExtensions
{
    internal static string GetProtocPath()
    {
        string osplatform = RuntimeExtensions.GetOsPlatformName();
        string processorArchitecture = RuntimeExtensions.GetProcessArchitecture();
        return RuntimeExtensions.IsWindows()
            ? $"{Directory.GetCurrentDirectory()}/tools/{osplatform}_{processorArchitecture}/protoc.exe"
            : $"{Directory.GetCurrentDirectory()}/tools/{osplatform}_{processorArchitecture}/protoc";
    }

    public static string GetTempProtoFileFolder()
    {
        return $"{Directory.GetCurrentDirectory()}/Temp";
    }
}