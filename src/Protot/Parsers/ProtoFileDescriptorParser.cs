using System.Diagnostics;
using Protot.Entities;
using Protot.Exceptions;
using Protot.Extensions;

namespace Protot;

internal sealed class ProtoFileDescriptorParser
{
    private string protoText;
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);
    internal ProtoFileDescriptorParser(string protoText)
    {
        this.protoText = protoText;
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
            return fileDescriptor?.ToProtoFileDescriptor();
        }
        finally
        {
            semaphoreSlim.Release();
        }

    }

    private async Task<bool> GenerateFileDescriptor()
    {
        await File.WriteAllTextAsync(ProtocExtensions.GetProtoFilePath(), this.protoText);

        var protoFilePath = ProtocExtensions.GetProtoFilePath();
        var fileDescriptiorPath = ProtocExtensions.GetFileDescriptorPath();
        var tempFolder = ProtocExtensions.GetTempFolder();
        if (!File.Exists(protoFilePath))
        {
            throw new FileNotFoundException("Unable to find Temp ProtoFile");
        }

        var protocPath = ProtocExtensions.GetProtocPath();

        string argument =
            $"{protoFilePath} --descriptor_set_out=\"{fileDescriptiorPath}\" --proto_path=\"{tempFolder}\"";

        ProcessStartInfo protocProcess = new ProcessStartInfo(
            protocPath,
            argument)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            WorkingDirectory = tempFolder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using var process = new Process();
        process.StartInfo = protocProcess;
        process.Start();


        await process.WaitForExitAsync();

        return process.ExitCode == 0;
    }
}