using System.Diagnostics;
using Protot.Entities;
using Protot.Extensions;

namespace Protot;

public class ProtoFileParser
{
    private string protoText;
    public ProtoFileParser(string protoText)
    {
        if (string.IsNullOrWhiteSpace(protoText))
        {
            throw new ArgumentNullException($"Proto Content is empty");
        }
        this.protoText = protoText;
    }

    public async Task<ProtoDescriptor> ParseFileAsync()
    {
        bool isFileDescGenerated = await GenerateFileDescriptor();
        if (isFileDescGenerated)
        {
            var fileDescriptor = ProtocExtensions.GetFileDescriptorPath().ExtractFileInfo();
        }

        return new ProtoDescriptor();
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

        Console.WriteLine(protocProcess.FileName + " " + protocProcess.Arguments, "protoc");
        using Process process = new Process();
        process.StartInfo = protocProcess;
        process.Start();


        await process.WaitForExitAsync();

        return process.ExitCode == 0;
    }
}