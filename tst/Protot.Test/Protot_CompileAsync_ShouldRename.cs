using PDM.Enums;
using Protot.Builders;
using Protot.Enums;
using Serilog;
using Xunit.Abstractions;

namespace Protot.Test;

[ExcludeFromCodeCoverage]
public class Protot_CompileAsync_ShouldRename
{
    public Protot_CompileAsync_ShouldRename(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }
    
    [Theory]
    [InlineData("ThreeFields", "MismatchedType")]
    public async Task CompileRenameTransformation(string sourceFile, string targetFile)
    {
        var sourceProto = await sourceFile.GetProtoText();
        var targetProto = await targetFile.GetProtoText();
        var builder = new ProtoTransformationBuilder();
        var  transformations = builder.AddTransformation(
            TransformationType.ReplaceField,
            TransformationSubtype.Renames,
            "StringValue:StringValue"
        ).Build();


        var prototMapper = new Protot(sourceProto, targetProto, transformations);
        var transforamtions = await prototMapper.CompileAsync();
        
        Assert.NotEmpty(transforamtions);
        Assert.Contains(transforamtions, x => x.TransformationType == TransformationType.ReplaceField );
        Assert.Contains(transforamtions, x => x.Value == "5:50");
    }
    
    [Theory]
    [InlineData("AllTypes", "ThreeFields")]
    public async Task CompileEmbeddedRenamesTransformation(string sourceFile, string targetFile)
    {
        var sourceProto = await sourceFile.GetProtoText();
        var targetProto = await targetFile.GetProtoText();
        var builder = new ProtoTransformationBuilder();
        var  transformations = builder.AddTransformation(
            TransformationType.ReplaceField,
            TransformationSubtype.Renames,
            "EmbeddedMessageValue.EmbeddedStringValue:StringValue                                                                                                                                                  "
        ).Build();


        var prototMapper = new Protot(sourceProto, targetProto, transformations);
        var transforamtions = await prototMapper.CompileAsync();
        
        Assert.NotEmpty(transforamtions);
        Assert.Contains(transforamtions, x => x.TransformationType == TransformationType.ReplaceField);
        Assert.Contains(transforamtions, x => x.Value == "3200.10100:5");
    }
}