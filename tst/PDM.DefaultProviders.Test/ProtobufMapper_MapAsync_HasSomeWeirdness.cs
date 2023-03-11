using Microsoft.Extensions.DependencyInjection;
using PDM.Builders;
using PDM.Parser.Extensions;
using Serilog;
using Xunit.Abstractions;
using PDM.TestUtils.Extensions;
using PDM.TestUtils.ProtoBuf;

namespace PDM.DefaultProviders.Test;

[ExcludeFromCodeCoverage]
[Collection("MapperTests")]
public class ProtobufMapper_MapAsync_HasSomeWeirdness
{
    private readonly IServiceProvider _serviceProvider;

    public ProtobufMapper_MapAsync_HasSomeWeirdness(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .UseDefaultParser()
            .AddSingleton<ProtobufMapper>()
            .BuildServiceProvider();
    }

    //[Fact]
    //public async Task DoSomeWeirdThingsUnderKnownConditions()
    //{
    //    // This test doesn't test anything. It documents that there
    //    // is some weirdness in the mapping of data-types due to
    //    // the fact that we only know the specified target wire-type
    //    // not the actual one. If the wire-format sizes match, the data
    //    // will transformed, with potentially odd results.
    //    //
    //    // Examples:
    //    // 	   A Len value (i.e. string) of length 8 will go into a I64 field
    //    //     A Len value (i.e. string) of length 4 will go into an I32 field
    //    //     Any I32 (i.e fixed32) will go into a Len field
    //    //     Any I64 (i.e.fixed64) will go into a Len field

    //    var sourceData = new Builders.ProtobufAllTypesBuilder()
    //        .UseRandomValues()
    //        .StringValue("Peculiar") // Field 3000
    //        .BytesValue(new byte[] { (byte)'D', (byte)'u', (byte)'d', (byte)'e' }) // Field 3100
    //        .Fixed64Value(8241984707611551056) // Field 2000
    //        .Fixed32Value(1801675095) // Field 4000
    //        .Build();

    //    var targetMapping = new TransformationBuilder()
    //        .AddUnmodifiedSourceField(5000, Enums.WireType.I64, 3000)
    //        .AddUnmodifiedSourceField(5100, Enums.WireType.I32, 3100)
    //        .AddUnmodifiedSourceField(5200, Enums.WireType.Len, 2000)
    //        .AddUnmodifiedSourceField(5300, Enums.WireType.Len, 4000)
    //        .Build();

    //    var sourceMessage = sourceData.ToByteArray();

    //    var target = new ProtobufMapper(targetMapping);
    //    var actual = await target.MapAsync(sourceMessage);

    //    var actualData = ProtoBuf.WeirdnessDemo.Parser.ParseFrom(actual);
    //    Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 3000, "String", sourceData.StringValue, 5000, actualData.StringStoredAsFixed64);
    //    Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 3100, "Byte[]", sourceData.BytesValue, 5100, actualData.BytesStoredAsFixed32);
    //    Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 2000, "Int64", sourceData.Fixed64Value, 5200, actualData.Fixed64StoredAsString);
    //    Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 4000, "Int32", sourceData.Fixed32Value, 5300, actualData.Fixed32StoredAsString);
    //}

    [Fact]
    public async Task EncodeSignedIntsDifferently()
    {
        var expected = 2132; // Zig-Zag encoded 1066
        var sourceData = new TestUtils.ProtoBuf.AllTypes()
        {
            SInt32Value = 1066
        };
        
        var targetMapping = new TransformationBuilder()
            .RenameField(1400, 1000) // Go from signed to unsigned
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage); ;

        var actualData = AllTypes.Parser.ParseFrom(actual);

        // Since the value was encoded using Zig-Zag encoding
        // we won't get the "intended" value of 1066, but
        // will instead see the encoded value of 2132
        Assert.Equal(expected, actualData.Int32Value);
    }

}
