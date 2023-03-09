using System.Text.Json;
using Protot;
using Serilog;

string? GetProtoFolder()
{
    return $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName}/Protobuf";
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var filePath = GetProtoFolder();
// Not working give permission denied
//await new FileDescriptorParser(await File.ReadAllTextAsync($"{filePath}/AllTypes.proto")).Parse();

// Working as this is not using protoc
var fileDescriptor  =await new ProtoBufFileParser(await File.ReadAllTextAsync($"{filePath}/AllTypes.proto"))
 .Parse();

Log.Information(JsonSerializer.Serialize(fileDescriptor));


