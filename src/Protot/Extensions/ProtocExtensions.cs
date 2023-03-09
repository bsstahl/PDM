using NuGet.Configuration;

namespace Protot.Extensions;

internal static class ProtocExtensions
{
    private const string  = "fileDesc.Proto";
    private const string ProtoDescriptionFile = "fileDesc";

    internal static string GetProtocPath()
    {
        var settings = Settings.LoadDefaultSettings(root: null);
        var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);

        var grpcToolFolder = Directory.GetDirectories($"{globalPackagesFolder}grpc.tools");

        var latestVersion = grpcToolFolder.MaxBy(x=> x);
        
        string platformName = RuntimeExtensions.GetOsPlatformName();
        string processorArchitecture = RuntimeExtensions.GetProcessArchitecture();
        return RuntimeExtensions.IsWindows()
            ? $"{latestVersion}/tools/{platformName}_{processorArchitecture}/protoc.exe"
            : $"{latestVersion}/tools/{platformName}_{processorArchitecture}/protoc";
    }

    internal static string GetTempProtoFileFolder()
    {
        return $"{Directory.GetCurrentDirectory()}/Temp";
    }
    
    internal static string GetTempProtoFilePath()
    {
        string tempFolderPath = FileExtensions.GetTempProtoFileFolder();
        if(!Directory.Exists(tempFolderPath))
        {
            Directory.CreateDirectory(tempFolderPath);
        }
        return $"{tempFolderPath}/{tempProtoFileName}";
    }
    
    internal static string GetTempProtoDescriptorPath()
    {
        return $"{GetTempProtoFileFolder()}/{FileDescProtoDescFileName}";
    }
}