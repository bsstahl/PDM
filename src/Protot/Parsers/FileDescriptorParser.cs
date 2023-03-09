using Protot.Exceptions;
using Protot.Extensions;
using System.Diagnostics;

namespace Protot;

public class FileDescriptorParser
{
    private string _protoContent;
    private const string tempProtoFileName = "temp.Proto";
    private const string tempProtoDescFileName = "temp";

    public FileDescriptorParser(string protoContent)
    {
        if (string.IsNullOrWhiteSpace(protoContent))
        {
            throw new ProtoFileParserException($"{protoContent} is required");
        }

        this._protoContent = protoContent;
    }

    public async Task<string> ParseAsync()
    {
        var tempProtoFilePath = GetTempProtoFilePath();
        var tempProtoDirectory = Path.GetDirectoryName(tempProtoFilePath);

        await File.WriteAllTextAsync(tempProtoFilePath, this._protoContent);

        if (!File.Exists(tempProtoFilePath))
        {
            throw new FileNotFoundException("Unable to find Temp ProtoFile");
        }

        var protocPath = GetProtocPath();
        var arguments = string.Format(
            @"--proto_path=""{0}"" --descriptor_set_out=""{1}"" ""{2}""",
            tempProtoDirectory,
            tempProtoDescFileName,
            tempProtoFileName);

        var protocProcess = new ProcessStartInfo(protocPath, arguments)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            WorkingDirectory = tempProtoDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        Console.WriteLine(protocProcess.FileName + " " + protocProcess.Arguments, "protoc");
        using Process process = new Process();
        process.StartInfo = protocProcess;
        process.Start();

        // Read the output and error streams
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode == 0)
        {
            Console.WriteLine("protoc succeeded: " + output);
            return output.Trim();
        }
        else
        {
            Console.WriteLine("protoc failed: " + error);
            return error.Trim();
        }
    }

    private static string GetTempProtoFilePath()
    {
        string tempFolderPath = FileExtensions.GetTempProtoFileFolder();
        if (!Directory.Exists(tempFolderPath))
        {
            Directory.CreateDirectory(tempFolderPath);
        }
        return Path.Combine(tempFolderPath, tempProtoFileName);
    }

    private static string GetTempProtoDescriptorPath()
    {
        return Path.Combine(FileExtensions.GetTempProtoFileFolder(), tempProtoDescFileName);
    }

    private static string GetProtocPath()
    {
        string protocPath = FileExtensions.GetProtocPath();

        if (File.Exists(protocPath))
        {
            return protocPath;
        }

        throw new FileLoadException($"Unable to find protoPath {protocPath}");
    }
}