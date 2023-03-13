using PDM.Enums;
using Protot.Builder;
using Protot.Entities;
using Serilog;
using Xunit.Abstractions;

namespace Protot.Test;

[ExcludeFromCodeCoverage]
public class PrototMapper_CreateTransformationAsync_Should
{
    private string protoFolderPath;
    public PrototMapper_CreateTransformationAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
        this.protoFolderPath = $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName}/ProtoBuf";
    }
    [Fact]
    public async Task CreateRenameTransformation()
    {
        var sourceProto = await File.ReadAllTextAsync($"{this.protoFolderPath}/ThreeFields.proto");
        var targetProto = await File.ReadAllTextAsync($"{this.protoFolderPath}/MismatchedType.proto");
        var builder = new TransformationInputBuilder();
        var  transformations = builder.AddTransformation(TransformationType.ReplaceField,
            new TransformationField("ThreeFields", "StringValue", typeof(string)),
            new TransformationField("MismatchedType", "StringValue", typeof(string))
        ).Build();


        var prototMapper = new PrototMapper(sourceProto, targetProto, transformations);
        var transforamtions = await prototMapper.CreateTransformationAsync();
        
        Assert.NotEmpty(transforamtions);
    }
}