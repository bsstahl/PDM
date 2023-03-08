using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PDM.Builders;
using Serilog;
using System.Diagnostics;
using Xunit.Abstractions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
[Collection("MapperTests")]
public class ProtobufMapper_MapAsync_Should
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProtobufMapper> _mapperLogger;

    public ProtobufMapper_MapAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .BuildServiceProvider();

        _mapperLogger = _serviceProvider
            .GetRequiredService<ILogger<ProtobufMapper>>();
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
    public async Task NotFailIfNoLoggerIsSupplied()
    {
        _ = Trace.Listeners.Add(new SerilogTraceListener(Log.Logger));
        var sourceMessage = Convert.FromBase64String("KghmZThhYjAyYVXdOEHIeKTOuuQE");
        var target = new ProtobufMapper(null!, null);
        var actual = await target.MapAsync(sourceMessage);
    }

    [Fact]
    public async Task ThrowIfNoSourceMessageSupplied()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        byte[]? sourceMessage = null;
        var targetMapping = new TransformationBuilder()
            .Build();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => target.MapAsync(sourceMessage!));
    }

    [Fact]
    public async Task ThrowANotImplementedExceptionIfAnUnknownReplaceFieldSubtypeIsSpecified()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        byte[] sourceMessage = Array.Empty<byte>();
        var targetMapping = new List<Entities.Transformation>()
        {
            { new Entities.Transformation() 
                {
                    TransformationType = Enums.TransformationType.ReplaceField,
                    SubType = "TotallyFakeSubtype",
                    Value = "This doesn't matter at all"
                }
            }
        };

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() => target.MapAsync(sourceMessage!));
    }

    [Fact]
    public async Task NotFailIfAnIncludeExpressionIsUnresolvable()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var targetMapping = new TransformationBuilder()
            .IncludeField(9999)
            .Build();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        Assert.Empty(actual);
    }

    [Fact]
    public async Task NotFailIfABlacklistExpressionIsUnresolvable()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .BlacklistField(9999)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task RemoveTheTargetFieldsIfTheSourceOfARenameExpressionIsUnresolvable()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        // Alpha characters cannot be translated to field #s
        var targetMapping = new TransformationBuilder()
            .RenameFields("a:5,b:15")
            .Build();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        Assert.Empty(actual);
    }

    [Fact]
    public async Task NotRemoveTheTargetFieldsIfTheTargetOfARenameExpressionIsUnresolvable()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        // Alpha characters cannot be translated to field #s
        var targetMapping = new TransformationBuilder()
            .RenameFields("5:a,15:b")
            .Build();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyCopyToTheSameTypeUnmodifiedIfNoMappingSupplied()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, null);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyMapASubsetTypeToMatchingFields()
    {
        var sourceData = new ProtoBuf.ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .BlacklistField(10)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyBlacklistAField()
    {
        // Guarantees that specified fields will be set to
        // default values. This is helpful if PII is being
        // masked-out of a message
        var sourceData = new ProtoBuf.ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .BlacklistField(10)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.ThreeFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
        Assert.Equal(0, actualData.FloatValue);
    }

    [Fact]
    public async Task ProperlyRenameAField()
    {
        var sourceData = new ProtoBuf.ThreeFields()
        {
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .RenameField(5,50)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.MismatchedType.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyRenameMultipleFields()
    {
        var sourceData = new ProtoBuf.ThreeFields()
        {
            StringValue = String.Empty.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            IntegerValue = Int32.MaxValue.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .RenameFields("5:50,15:150")
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.MismatchedType.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.StringValue, actualData.StringValue);
        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
    }

    [Fact]
    public async Task IncludeValuesOnlyForFieldsSpecifiedInTheIncludeList()
    {
        var sourceData = new ProtoBuf.ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .IncludeField(10)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.ThreeFields.Parser.ParseFrom(actual);

        Assert.Equal(0, actualData.IntegerValue);
        Assert.Equal(String.Empty, actualData.StringValue);
        Assert.Equal(sourceData.FloatValue, actualData.FloatValue);
    }

    [Fact]
    public async Task ProperlyCopyAllFieldsToATargetOfTheSameType()
    {
        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, null);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.AllTypes.Parser.ParseFrom(actual);
        Assert.Equal(sourceData.Int32Value, actualData.Int32Value);
        Assert.Equal(sourceData.Int64Value, actualData.Int64Value);
        Assert.Equal(sourceData.UInt32Value, actualData.UInt32Value);
        Assert.Equal(sourceData.UInt64Value, actualData.UInt64Value);
        Assert.Equal(sourceData.SInt32Value, actualData.SInt32Value);
        Assert.Equal(sourceData.SInt64Value, actualData.SInt64Value);
        Assert.Equal(sourceData.BoolValue, actualData.BoolValue);
        Assert.Equal(sourceData.EnumValue, actualData.EnumValue);

        Assert.Equal(sourceData.Fixed64Value, actualData.Fixed64Value);
        Assert.Equal(sourceData.SFixed64Value, actualData.SFixed64Value);
        Assert.Equal(sourceData.DoubleValue, actualData.DoubleValue);

        Assert.Equal(sourceData.StringValue, actualData.StringValue);
        Assert.Equal(sourceData.BytesValue, actualData.BytesValue);
        Assert.NotNull(actualData.EmbeddedMessageValue); // Embedded Fields
        Assert.Equal(sourceData.EmbeddedMessageValue.EmbeddedStringValue, actualData.EmbeddedMessageValue.EmbeddedStringValue);
        Assert.Equal(sourceData.EmbeddedMessageValue.EmbeddedInt32Value, actualData.EmbeddedMessageValue.EmbeddedInt32Value);
        Assert.Equal(sourceData.RepeatedInt32Value, actualData.RepeatedInt32Value);

        Assert.Equal(sourceData.Fixed32Value, actualData.Fixed32Value);
        Assert.Equal(sourceData.SFixed32Value, actualData.SFixed32Value);
        Assert.Equal(sourceData.FloatValue, actualData.FloatValue);
    }

    [Fact]
    public async Task ProperlyInsertAStaticValue_VarintAsInt32()
    {
        var expected = Int32.MaxValue.GetRandom();

        var targetMapping = new TransformationBuilder()
            .InsertStaticField(1000, Enums.WireType.VarInt, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.Int32Value);
    }

    [Fact]
    public async Task ProperlyInsertAStaticValue_I32AsFixed32()
    {
        var expected = Convert.ToUInt32(Math.Abs(int.MaxValue.GetRandom()));

        var targetMapping = new TransformationBuilder()
            .InsertStaticField(4000, Enums.WireType.I32, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.Fixed32Value);
    }

    [Fact]
    public async Task ProperlyInsertAStaticValue_I32AsSFixed32()
    {
        var expected = int.MaxValue.GetRandom();

        var targetMapping = new TransformationBuilder()
            .InsertStaticField(4100, Enums.WireType.I32, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.SFixed32Value);
    }

    [Fact]
    public async Task ProperlyInsertAStaticValue_I32AsFloat()
    {
        var expected = float.MaxValue.GetRandom();

        var targetMapping = new TransformationBuilder()
            .InsertStaticField(4200, Enums.WireType.I32, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.FloatValue);
    }

    [Fact]
    public async Task ProperlyMapsFromEmbeddedMessageField()
    {
        var expected = Int32.MaxValue.GetRandom();

        var targetMapping = new TransformationBuilder()
            .IncludeField(0) // Clears out default mappings
            .RenameField("3200.10000", 15) // Include field 15 mapped from embedded message
            .RenameField("3200.10100", 5) // Include field 5 mapped from embedded message
            .Build();

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(_mapperLogger, targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.EmbeddedMessageValue.EmbeddedInt32Value, actualData.IntegerValue);
        Assert.Equal(sourceData.EmbeddedMessageValue.EmbeddedStringValue, actualData.StringValue);
    }

}
