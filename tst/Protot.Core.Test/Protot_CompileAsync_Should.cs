using PDM.Enums;
using Protot.Core.Builders;
using Protot.Core.Entities;
using Protot.Core.Enums;
using Protot.Core.Exceptions;
using Serilog;
using Xunit.Abstractions;

namespace Protot.Core.Test;

[ExcludeFromCodeCoverage]
public class Protot_CompileAsync_Should
{
    public Protot_CompileAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowIfSourceFileNotSupplied(string source)
    {
        Assert.Throws<ArgumentNullException>(() => new PrototMapper(source, "target", new List<ProtoTransformation>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowIfTargetFileNotSupplied(string target)
    {
        Assert.Throws<ArgumentNullException>(() => new PrototMapper("source", target, new List<ProtoTransformation>()));
    }

    [Theory]
    [InlineData(null)]
    public void ThrowIfTransformationConfigurationNotSupplied(IEnumerable<ProtoTransformation>? transformationConfigurations)
    {
        Assert.Throws<ArgumentNullException>(() => new PrototMapper("source", "target", transformationConfigurations));
    }

    [Fact]
    public void ThrowIfTransformationConfigurationIsEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new PrototMapper("source", "target", Enumerable.Empty<ProtoTransformation>()));
    }

    [Theory]
    [InlineData("FakeFile", "MismatchedType")]
    [InlineData("MismatchedType", "FakeFile")]
    public async Task ThrowIfProtoFileIsInvalid(string sourceFile, string targetFile)
    {
        var builder = new ProtoTransformationBuilder();
        var transformations = builder.AddTransformation(
            TransformationType.ReplaceField,
            TransformationSubtype.Renames,
            "StringValue:StringValue"
        ).Build();

        var protot = new PrototMapper(sourceFile, targetFile, transformations);
        await Assert.ThrowsAsync<PrototMapperException>(async () => await protot.CompileAsync());
    }

    [Theory]
    [InlineData("ThreeFields", "MismatchedType")]
    public async Task ThrowIfFieldIsNotPresentInMessage(string sourceFile, string targetFile)
    {
        var builder = new ProtoTransformationBuilder();
        var transformations = builder.AddTransformation(
            TransformationType.ReplaceField,
            TransformationSubtype.Renames,
            "NotPresent:StringValue"
        ).Build();

        var protot = new PrototMapper(sourceFile, targetFile, transformations);
        await Assert.ThrowsAsync<PrototMapperException>(async () => await protot.CompileAsync());
    }
}