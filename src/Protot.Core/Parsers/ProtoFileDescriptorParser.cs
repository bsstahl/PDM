using System.Diagnostics;
using Protot.Core.Entities;
using Protot.Core.Exceptions;
using Protot.Core.Extensions;
namespace Protot.Core;

internal sealed class ProtoFileDescriptorParser
{
    private ProtoFile protoFile;
    private string messageToTransform;
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);
    internal ProtoFileDescriptorParser(ProtoFile protoFile, string messageToTransform)
    {
        this.protoFile = protoFile;
        this.messageToTransform = messageToTransform;
    }

    internal async Task<ProtoFileDescriptor?> ParseFileAsync()
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            bool isFileDescGenerated = await GenerateFileDescriptor().ConfigureAwait(false);
            if (!isFileDescGenerated)
            {
                throw new PrototMapperException("Unable to get message Information");
            }

            var fileDescriptor = ProtocExtensions.GetFileDescriptorPath().ExtractFileInfo();
            return fileDescriptor?.ToProtoFileDescriptor(this.messageToTransform);
        }
        finally
        {
            semaphoreSlim.Release();
        }

    }

    private async Task<bool> GenerateFileDescriptor()
    {
        var protocInfo = ProtocExtensions.GetProtocPath();

        await SaveProtoFileInfo(protocInfo.googleProtoFolder);

        var protoFilePath = ProtocExtensions.GetProtoFilePath();
        var fileDescriptiorPath = ProtocExtensions.GetFileDescriptorPath();
        var tempFolder = ProtocExtensions.GetTempFolder();
        if (!File.Exists(protoFilePath))
        {
            throw new FileNotFoundException("Unable to find Temp ProtoFile");
        }

        string argument = $"{protoFilePath}  -I=\"{tempFolder}\" --descriptor_set_out=\"{fileDescriptiorPath}\" --include_imports";

        ProcessStartInfo protocProcess = new ProcessStartInfo(
            protocInfo.protocPath,
            argument)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
           // WorkingDirectory = tempFolder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using var process = new Process();
        process.StartInfo = protocProcess;
        process.Start();
       
        await process.WaitForExitAsync();
        
        bool isSuccess = process.ExitCode == 0;
        if (!isSuccess)
        {
            var output = await process.StandardError.ReadToEndAsync();
            throw new PrototMapperException(output);
        }

        return isSuccess;
    }

    private async Task SaveProtoFileInfo(string googleFolder)
    {
        ProtocExtensions.ClearProtoTempDirectory();
        await File.WriteAllTextAsync(ProtocExtensions.GetProtoFilePath(), this.protoFile.Content);
        if (this.protoFile.ReferenceFiles != null)
        {
            foreach (var reference in this.protoFile.ReferenceFiles)
            {
                var filePath = $"{ProtocExtensions.GetTempFolder()}{reference.ReferencePath}";
                var refFile = new FileInfo(filePath);
                var refDirectory = refFile.Directory;
                if (!refDirectory!.Exists)
                {
                    refDirectory.Create();
                }
               
                await File.WriteAllTextAsync($"{ProtocExtensions.GetTempFolder()}{reference.ReferencePath}", reference.Content);
            }
        }

        var googleDirectoryInfo = new DirectoryInfo(googleFolder);
        foreach (var file in googleDirectoryInfo.EnumerateFiles("*.proto"))
        {
            file.CopyTo($"{ProtocExtensions.GetTempFolder()}/protobuf/{file.Name}");
        } 
    }
}