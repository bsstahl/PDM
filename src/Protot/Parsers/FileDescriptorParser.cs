using System.Diagnostics;
using Protot.Entities;
using Protot.Exceptions;
using Protot.Extensions;

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

    public async Task Parse()
    { 
        await File.WriteAllTextAsync(GetTempProtoFilePath(), this._protoContent);
        
        if (!File.Exists(GetTempProtoFilePath()))
        {
            throw new FileNotFoundException("Unable to find Temp ProtoFile");
        }

        var protocPath = GetProtocPath();

        ProcessStartInfo protocProcess = new ProcessStartInfo(
            protocPath,
            string.Format(@"""--descriptor_set_out={0}"" ""--proto_path={1}"" ",
                GetTempProtoDescriptorPath,
                FileExtensions.GetTempProtoFileFolder()))
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            WorkingDirectory = Environment.CurrentDirectory
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
        }
        else
        {
            Console.WriteLine("protoc failed: " + error);
        }
    }
    private static string GetTempProtoFilePath()
    {
        string tempFolderPath = FileExtensions.GetTempProtoFileFolder();
        if(!Directory.Exists(tempFolderPath))
        {
            Directory.CreateDirectory(tempFolderPath);
        }
        return $"{tempFolderPath}/{tempProtoFileName}";
    }
    
    private static string GetTempProtoDescriptorPath()
    {
        return $"{FileExtensions.GetTempProtoFileFolder()}/{tempProtoDescFileName}";
    }
    
    private static string  GetProtocPath()
    {
        string protocPath = FileExtensions.GetProtocPath();
        if (File.Exists(protocPath))
        {
            return protocPath;
        }

        throw new FileLoadException($"Unable to find protoPath {protocPath}");
    }
}