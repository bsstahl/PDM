using System.Text.Json;
using Protot;
using Serilog;

string GetProtoFolder()
{
    return Path.Combine(        
        Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty,
        "Protobuf");
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var filePath = GetProtoFolder();

var fileContent = await File.ReadAllTextAsync(Path.Combine(filePath, "AllTypes.proto"));
var parser = new FileDescriptorParser(fileContent);
var fileDescriptor = await parser.ParseAsync();

// Working as this is not using protoc
//var fileDescriptor  =await new ProtoBufFileParser(await File.ReadAllTextAsync(Path.Combine(filePath, "AllTypes.proto")))
// .Parse();

Log.Information(JsonSerializer.Serialize(fileDescriptor));


