using PDM.Enums;
using Protot.Core.Builders;
using Protot.Core.Enums;
using Protot.Core.Test.Extensions;
using Serilog;
using Xunit.Abstractions;

namespace Protot.Core.Test;

[ExcludeFromCodeCoverage]
[Collection("Rename Transformation")]
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
    public async Task SingleFieldRenameTransformation(string sourceFile, string targetFile)
    {
        var sourceProto = await sourceFile.GetProtoText();
        var targetProto = await targetFile.GetProtoText();
        var builder = new ProtoTransformationBuilder();
        var transformations = builder.AddTransformation(
            TransformationType.ReplaceField,
            TransformationSubtype.Renames,
            "StringValue:StringValue"
        ).Build();


        var prototMapper = new PrototMapper(sourceProto, targetProto, transformations);
        var compliedTransformation = await prototMapper.CompileAsync();

        Assert.NotEmpty(compliedTransformation);
        Assert.Contains(compliedTransformation, x => x.TransformationType == TransformationType.ReplaceField);
        Assert.Contains(compliedTransformation, x => x.Value == "5:50");
    }

    [Theory]
    [InlineData("AllTypes", "ThreeFields")]
    public async Task EmbeddedFieldRenameTransformation(string sourceFile, string targetFile)
    {
        var sourceProto = await sourceFile.GetProtoText();
        var targetProto = await targetFile.GetProtoText();
        var builder = new ProtoTransformationBuilder();
        var transformations = builder.AddTransformation(
            TransformationType.ReplaceField,
            TransformationSubtype.Renames,
            "EmbeddedMessageValue.EmbeddedStringValue:StringValue                                                                                                                                                  "
        ).Build();


        var prototMapper = new PrototMapper(sourceProto, targetProto, transformations);
        var compliedTransformation = await prototMapper.CompileAsync();

        Assert.NotEmpty(compliedTransformation);
        Assert.Contains(compliedTransformation, x => x.TransformationType == TransformationType.ReplaceField);
        Assert.Contains(compliedTransformation, x => x.Value == "3200.10100:5");
    }


    [Theory]
    [InlineData("AllTypes", "ThreeFields")]
    public async Task MultipleFieldsRenameTransformation(string sourceFile, string targetFile)
    {
        var sourceProto = await sourceFile.GetProtoText();
        var targetProto = await targetFile.GetProtoText();
        var builder = new ProtoTransformationBuilder();
        var transformations = builder.AddTransformation(
            TransformationType.ReplaceField,
            TransformationSubtype.Renames,
            "Int32Value:IntegerValue,StringValue:StringValue"
        ).Build();


        var prototMapper = new PrototMapper(sourceProto, targetProto, transformations);
        var compliedTransformation = await prototMapper.CompileAsync();

        Assert.NotEmpty(compliedTransformation);
        Assert.Contains(compliedTransformation, x => x.TransformationType == TransformationType.ReplaceField);
        Assert.Contains(compliedTransformation, x => x.Value == "1000:15,3000:5");
    }
}