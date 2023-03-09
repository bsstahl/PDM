using Serilog;
using Xunit.Abstractions;

namespace Protot.Test;

[ExcludeFromCodeCoverage]
public class ProtoFileParser_ParseFileAsync_Should
{
    private string protoFolderPath;
    public ProtoFileParser_ParseFileAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
        this.protoFolderPath = $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName}/ProtoBuf";
    }
    [Theory]
    [InlineData("AllTypes.proto")]
    [InlineData("MismatchedType.proto")]
    [InlineData("ThreeFields.proto")]
    [InlineData("TwoFields.proto")]
    [InlineData("WeirdnessDemo.proto")]
    public async Task ProtoFileParser_ParseFileAsync_GenerateFileDescriptor(string protoFile)
    {
        var protoParser = new ProtoFileParser(await File.ReadAllTextAsync($"{this.protoFolderPath}/{protoFile}"));
        await protoParser.ParseFileAsync();
    }
}