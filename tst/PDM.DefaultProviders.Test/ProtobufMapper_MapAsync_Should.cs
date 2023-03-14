using Microsoft.Extensions.DependencyInjection;
using PDM.Builders;
using PDM.Parser.Extensions;
using PDM.TestUtils.Extensions;
using PDM.TestUtils.ProtoBuf;
using PDM.TestUtils.Builders;
using Serilog;
using Xunit.Abstractions;

namespace PDM.DefaultProviders.Test;

[ExcludeFromCodeCoverage]
[Collection("MapperTests")]
public class ProtobufMapper_MapAsync_Should
{
    private readonly IServiceProvider _serviceProvider;

    public ProtobufMapper_MapAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Error()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .UseDefaultParser()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task ThrowANotImplementedExceptionIfAnUnknownReplaceFieldSubtypeIsSpecified()
    {
        var sourceData = new TwoFields()
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

        var target = _serviceProvider.GetMapper(targetMapping);
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() => target.MapAsync(sourceMessage!));
    }

    [Fact]
    public async Task NotFailIfAnIncludeExpressionIsUnresolvable()
    {
        var sourceData = new TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var targetMapping = new TransformationBuilder()
            .IncludeField(9999)
            .Build();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        Assert.Empty(actual);
    }

    [Fact]
    public async Task NotFailIfABlacklistExpressionIsUnresolvable()
    {
        var sourceData = new TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .BlacklistField(9999)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        var actualData = TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task RemoveTheTargetFieldsIfTheSourceOfARenameExpressionIsUnresolvable()
    {
        var sourceData = new TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        // Alpha characters cannot be translated to field #s
        var targetMapping = new TransformationBuilder()
            .RenameFields("a:5,b:15")
            .Build();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        Assert.Empty(actual);
    }

    [Fact]
    public async Task NotRemoveTheTargetFieldsIfTheTargetOfARenameExpressionIsUnresolvable()
    {
        var sourceData = new TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        // Alpha characters cannot be translated to field #s
        var targetMapping = new TransformationBuilder()
            .RenameFields("5:a,15:b")
            .Build();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage!);

        var actualData = TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyCopyToTheSameTypeUnmodifiedIfNoMappingSupplied()
    {
        var sourceData = new TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper();
        var actual = await target.MapAsync(sourceMessage);

        var actualData = TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyMapASubsetTypeToMatchingFields()
    {
        var sourceData = new ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .BlacklistField(10)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyBlacklistAField()
    {
        // Guarantees that specified fields will be set to
        // default values. This is helpful if PII is being
        // masked-out of a message
        var sourceData = new ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .BlacklistField(10)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ThreeFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
        Assert.Equal(0, actualData.FloatValue);
    }

    [Fact]
    public async Task ProperlyRenameAField()
    {
        var sourceData = new ThreeFields()
        {
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .RenameField(5, 50)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = MismatchedType.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyRenameMultipleFields()
    {
        var sourceData = new ThreeFields()
        {
            StringValue = String.Empty.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            IntegerValue = Int32.MaxValue.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .RenameFields("5:50,15:150")
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = MismatchedType.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.StringValue, actualData.StringValue);
        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
    }

    [Fact]
    public async Task IncludeValuesOnlyForFieldsSpecifiedInTheIncludeList()
    {
        var sourceData = new ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new TransformationBuilder()
            .IncludeField(10)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = ThreeFields.Parser.ParseFrom(actual);

        Assert.Equal(0, actualData.IntegerValue);
        Assert.Equal(String.Empty, actualData.StringValue);
        Assert.Equal(sourceData.FloatValue, actualData.FloatValue);
    }

    [Fact]
    public async Task ProperlyCopyAllFieldsToATargetOfTheSameType()
    {
        var sourceData = new ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper();
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);
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

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

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

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

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

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

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

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.FloatValue);
    }

    [Fact]
    public async Task ProperlyMapsFromEmbeddedMessageField()
    {
        var targetMapping = new TransformationBuilder()
            .IncludeField(0) // Clears out default mappings
            .RenameField("3200.10000", 15) // Include field 15 mapped from embedded message
            .RenameField("3200.10100", 5) // Include field 5 mapped from embedded message
            .Build();

        var sourceData = new ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = TwoFields.Parser.ParseFrom(actual);

        Assert.NotEmpty(sourceData.EmbeddedMessageValue.EmbeddedStringValue);
        Assert.NotEqual(0, sourceData.EmbeddedMessageValue.EmbeddedInt32Value);
        Assert.Equal(sourceData.EmbeddedMessageValue.EmbeddedStringValue, actualData.StringValue);
        Assert.Equal(sourceData.EmbeddedMessageValue.EmbeddedInt32Value, actualData.IntegerValue);
    }

}
