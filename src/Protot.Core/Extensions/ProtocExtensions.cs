using NuGet.Configuration;

namespace Protot.Core.Extensions;

internal static class ProtocExtensions
{
    private const string TempProtoFileName = "temp.Proto";
    private const string ProtoDescriptionFile = "fileDesc";

    internal static string GetProtocPath()
    {
        var settings = Settings.LoadDefaultSettings(root: null);
        var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);
        var grpcToolFolder = Directory.GetDirectories($"{globalPackagesFolder}grpc.tools");
        var latestVersion = grpcToolFolder.MaxBy(x => x);
        string platformName = RuntimeExtensions.GetOsPlatformName();
        string processorArchitecture = RuntimeExtensions.GetProcessArchitecture();
        return RuntimeExtensions.IsWindows()
            ? $"{latestVersion}/tools/{platformName}_{processorArchitecture}/protoc.exe"
            : $"{latestVersion}/tools/{platformName}_{processorArchitecture}/protoc";
    }

    internal static string GetProtoFilePath()
    {
        string tempFolderPath = GetTempFolder();
        if (!Directory.Exists(tempFolderPath))
        {
            Directory.CreateDirectory(tempFolderPath);
        }
        return $"{tempFolderPath}/{TempProtoFileName}";
    }

    internal static string GetFileDescriptorPath()
    {
        return $"{GetTempFolder()}/{ProtoDescriptionFile}";
    }

    internal static string GetTempFolder()
    {
        return $"{Directory.GetCurrentDirectory()}/Temp";
    }
}