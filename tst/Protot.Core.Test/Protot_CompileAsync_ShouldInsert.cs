using PDM.Enums;
using Protot.Core;
using Protot.Core.Builders;
using Protot.Core.Enums;
using Protot.Core.Test.Extensions;
using Serilog;
using Xunit.Abstractions;

namespace Protot.Core.Test;

[ExcludeFromCodeCoverage]
[Collection("Insert Transforamtion")]
public class Protot_CompileAsync_ShouldInsert
{
    public Protot_CompileAsync_ShouldInsert(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }
    
    [Theory]
    [InlineData("ThreeFields", "MismatchedType")]
    public async Task InsertAStaticValue_VarintAsInt32(string sourceFile, string targetFile)
    {
        var sourceProto = await sourceFile.GetProtoText();
        var targetProto = await targetFile.GetProtoText();
        var builder = new ProtoTransformationBuilder();
        var  transformations = builder.AddTransformation(
            TransformationType.InsertField,
            TransformationSubtype.Static,
            "IntegerValue:int32:173559425"
        ).Build();


        var prototMapper = new PrototMapper(sourceProto, targetProto, transformations);
        var compliedTransformation = await prototMapper.CompileAsync();
        
        Assert.NotEmpty(compliedTransformation);
        Assert.Contains(compliedTransformation, x => x.TransformationType == TransformationType.InsertField);
        Assert.Contains(compliedTransformation, x => x.Value == "15:VarInt:173559425");
    }
    
}