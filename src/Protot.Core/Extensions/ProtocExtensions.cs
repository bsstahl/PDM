using NuGet.Configuration;

namespace Protot.Core.Extensions;

internal static class ProtocExtensions
{
    private const string TempProtoFileName = "temp.proto";
    private const string ProtoDescriptionFile = "fileDesc";

    internal static (string protocPath, string googleProtoFolder) GetProtocPath()
    {
        var settings = Settings.LoadDefaultSettings(root: null);
        var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);
        var grpcToolFolder = Directory.GetDirectories($"{globalPackagesFolder}grpc.tools");
        var latestVersion = grpcToolFolder.MaxBy(x=> x);
        string platformName = RuntimeExtensions.GetOsPlatformName();
        string processorArchitecture = RuntimeExtensions.GetProcessArchitecture();
        var protocPath = RuntimeExtensions.IsWindows()
            ? $"{latestVersion}/tools/{platformName}_{processorArchitecture}/protoc.exe"
            : $"{latestVersion}/tools/{platformName}_{processorArchitecture}/protoc";

        var googleProtoFolder = $"{latestVersion}/build/native/include/";

        return (protocPath, googleProtoFolder);
    }
    
   
    internal static string GetProtoFilePath()
    {
        string tempFolderPath = GetTempFolder();
        if(!Directory.Exists(tempFolderPath))
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

    internal static void ClearProtoTempDirectory()
    {
        var tempFolder = GetTempFolder();
        if (!Directory.Exists(tempFolder)) return;
        var tempFolderDirectory = new DirectoryInfo(tempFolder);
        foreach (var directory in tempFolderDirectory.EnumerateDirectories())
        {
            if (directory.Name != "google")
            {
                directory.Delete(true);
            }
        }
        //tempFolderDirectory.Delete(true);
    }
}