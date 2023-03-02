using PDM.Builders;
using Serilog;
using Xunit.Abstractions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class ProtobufMapper_MapAsync_Should
{
    public ProtobufMapper_MapAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }

    [Fact]
    public async Task DoSomeWeirdThingsUnderKnownConditions()
    {
        // This test doesn't test anything. It documents that there
        // is some weirdness in the mapping of data-types due to
        // the fact that we only know the specified target wire-type
        // not the actual one. If the wire-format sizes match, the data
        // will transformed, with potentially odd results.
        //
        // Examples:
        // 	   A Len value (i.e. string) of length 8 will go into a I64 field
        //     A Len value (i.e. string) of length 4 will go into an I32 field
        //     Any I32 (i.e fixed32) will go into a Len field
        //     Any I64 (i.e.fixed64) will go into a Len field

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .StringValue("Peculiar") // Field 3000
            .BytesValue(new byte[] { (byte)'D', (byte)'u', (byte)'d', (byte)'e' }) // Field 3100
            .Fixed64Value(8241984707611551056) // Field 2000
            .Fixed32Value(1801675095) // Field 4000
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(5000, Enums.WireType.I64, 3000)
            .AddUnmodifiedSourceField(5100, Enums.WireType.I32, 3100)
            .AddUnmodifiedSourceField(5200, Enums.WireType.Len, 2000)
            .AddUnmodifiedSourceField(5300, Enums.WireType.Len, 4000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        var actual = await target.MapAsync(sourceMessage, targetMapping);
        Log.Verbose("TargetMessage: {TargetMessage}", Convert.ToBase64String(actual));

        var actualData = ProtoBuf.WeirdnessDemo.Parser.ParseFrom(actual);
        Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 3000, "String", sourceData.StringValue, 5000, actualData.StringStoredAsFixed64);
        Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 3100, "Byte[]", sourceData.BytesValue, 5100, actualData.BytesStoredAsFixed32);
        Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 2000, "Int64", sourceData.Fixed64Value, 5200, actualData.Fixed64StoredAsString);
        Log.Information("Source Field {sourceFieldNumber} of type {clrType} was {sourceValue} and was mapped to {targetFieldNumber} as {targetValue}", 4000, "Int32", sourceData.Fixed32Value, 5300, actualData.Fixed32StoredAsString);
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
        var targetMapping = new MappingBuilder()
            .Build();

        var target = new ProtobufMapper();
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => target.MapAsync(sourceMessage!, targetMapping));
    }

    [Fact]
    public async Task ThrowIfNoTargetMappingSupplied()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper();
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => target.MapAsync(sourceMessage, null!));
    }

    [Fact]
    public async Task ReturnTheDefaultValueForAllFieldsFromAZeroLengthMessage()
    {
        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(1000, Enums.WireType.VarInt, 1000)
            .AddUnmodifiedSourceField(1100, Enums.WireType.VarInt, 1100)
            .AddUnmodifiedSourceField(1200, Enums.WireType.VarInt, 1200)
            .AddUnmodifiedSourceField(1300, Enums.WireType.VarInt, 1300)
            .AddUnmodifiedSourceField(1400, Enums.WireType.VarInt, 1400)
            .AddUnmodifiedSourceField(1500, Enums.WireType.VarInt, 1500)
            .AddUnmodifiedSourceField(1600, Enums.WireType.VarInt, 1600)
            .AddUnmodifiedSourceField(1700, Enums.WireType.VarInt, 1700)
            .AddUnmodifiedSourceField(2000, Enums.WireType.I64, 2000)
            .AddUnmodifiedSourceField(2100, Enums.WireType.I64, 2100)
            .AddUnmodifiedSourceField(2200, Enums.WireType.I64, 2200)
            .AddUnmodifiedSourceField(3000, Enums.WireType.Len, 3000)
            .AddUnmodifiedSourceField(3100, Enums.WireType.Len, 3100)
            .AddUnmodifiedSourceField(3200, Enums.WireType.Len, 3200)
            .AddUnmodifiedSourceField(3300, Enums.WireType.Len, 3300)
            .AddUnmodifiedSourceField(4000, Enums.WireType.I32, 4000)
            .AddUnmodifiedSourceField(4100, Enums.WireType.I32, 4100)
            .AddUnmodifiedSourceField(4200, Enums.WireType.I32, 4200)
            .Build();

        var sourceMessage = Array.Empty<byte>();
        var target = new ProtobufMapper();
        var actual = await target.MapAsync(sourceMessage, targetMapping);
        Assert.Empty(actual); // The resulting message should also have zero length
    }

    [Fact]
    public async Task ProperlyCopyToTheSameTypeUnmodified()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(10, Enums.WireType.Len, 10)
            .AddUnmodifiedSourceField(20, Enums.WireType.VarInt, 20)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper();
        var actual = await target.MapAsync(sourceMessage, targetMapping);

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

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(10, Enums.WireType.Len, 5)
            .AddUnmodifiedSourceField(20, Enums.WireType.VarInt, 15)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper();
        var actual = await target.MapAsync(sourceMessage, targetMapping);

        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task ProperlyReturnAFalseBoolField()
    {
        var sourceData = new ProtoBuf.MismatchedType()
        {
            StringValue = String.Empty.GetRandom(),
            FloatValue = String.Empty.GetRandom(),
            IntegerValue = Int32.MaxValue.GetRandom(),
            BoolValue = false
        };

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(50, Enums.WireType.Len, 50)
            .AddUnmodifiedSourceField(100, Enums.WireType.Len, 100)
            .AddUnmodifiedSourceField(125, Enums.WireType.VarInt, 125)
            .AddUnmodifiedSourceField(150, Enums.WireType.VarInt, 150)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        var actual = await target.MapAsync(sourceMessage, targetMapping);
        Log.Verbose("TargetMessage: {TargetMessage}", Convert.ToBase64String(actual));

        var actualData = ProtoBuf.MismatchedType.Parser.ParseFrom(actual);
        Assert.Equal(sourceData.BoolValue, actualData.BoolValue);
    }

    [Fact]
    public async Task ProperlyCopyAllFieldsToATargetOfTheSameType()
    {
        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(1000, Enums.WireType.VarInt, 1000)
            .AddUnmodifiedSourceField(1100, Enums.WireType.VarInt, 1100)
            .AddUnmodifiedSourceField(1200, Enums.WireType.VarInt, 1200)
            .AddUnmodifiedSourceField(1300, Enums.WireType.VarInt, 1300)
            .AddUnmodifiedSourceField(1400, Enums.WireType.VarInt, 1400)
            .AddUnmodifiedSourceField(1500, Enums.WireType.VarInt, 1500)
            .AddUnmodifiedSourceField(1600, Enums.WireType.VarInt, 1600)
            .AddUnmodifiedSourceField(1700, Enums.WireType.VarInt, 1700)
            .AddUnmodifiedSourceField(2000, Enums.WireType.I64, 2000)
            .AddUnmodifiedSourceField(2100, Enums.WireType.I64, 2100)
            .AddUnmodifiedSourceField(2200, Enums.WireType.I64, 2200)
            .AddUnmodifiedSourceField(3000, Enums.WireType.Len, 3000)
            .AddUnmodifiedSourceField(3100, Enums.WireType.Len, 3100)
            .AddUnmodifiedSourceField(3200, Enums.WireType.Len, 3200)
            .AddUnmodifiedSourceField(3300, Enums.WireType.Len, 3300)
            .AddUnmodifiedSourceField(4000, Enums.WireType.I32, 4000)
            .AddUnmodifiedSourceField(4100, Enums.WireType.I32, 4100)
            .AddUnmodifiedSourceField(4200, Enums.WireType.I32, 4200)
            .Build();

        Log.Verbose("targetMapping: {TargetMapping}", System.Text.Json.JsonSerializer.Serialize(targetMapping));

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();

        var actual = await target.MapAsync(sourceMessage, targetMapping);
        Log.Verbose("TargetMessage: {TargetMessage}", Convert.ToBase64String(actual));

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
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_I64ToVarint()
    {
        // Try to put the Fixed64Value (I64) from the source message
        // Into the Int32Value (Varint) of the target message
        // Specifying the Varint wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(1000, Enums.WireType.VarInt, 2000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_LenToVarint()
    {
        // Try to put the StringValue (Len) from the source message
        // Into the Int32Value (Varint) of the target message
        // Specifying the Varint wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(1000, Enums.WireType.VarInt, 3000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_I32ToVarint()
    {
        // Try to put the FloatValue (I32) from the source message
        // Into the Int32Value (Varint) of the target message
        // Specifying the Varint wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(1000, Enums.WireType.VarInt, 4000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_VarintToI64()
    {
        // Try to put the Int32Value (VarInt) from the source message
        // Into the Fixed64Value (I64) of the target message
        // Specifying the I64 wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(2000, Enums.WireType.I64, 1000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_LenToI64()
    {
        // Try to put the StringValue (Len) from the source message
        // Into the Fixed64Value (I64) of the target message
        // Specifying the I64 wire format

        int stringLength;
        do
        {
            stringLength = 30.GetRandom(1);
        } while (stringLength == 8); // Test will fail with 8 characters

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .StringValue(string.Empty.GetRandom(stringLength))
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(2000, Enums.WireType.I64, 3000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_I32ToI64()
    {
        // Try to put the FloatValue (I32) from the source message
        // Into the Fixed64Value (I64) of the target message
        // Specifying the I64 wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(2000, Enums.WireType.I64, 4000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_VarintToLen()
    {
        // Try to put the Int32Value (VarInt) from the source message
        // Into the string (Len) of the target message
        // Specifying the Len wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(3000, Enums.WireType.Len, 1000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_VarintToI32()
    {
        // Try to put the Int32Value (Varint) from the source message
        // Into the Fixed32Value (I32) of the target message
        // Specifying the I32 wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(4000, Enums.WireType.I32, 1000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_I64toI32()
    {
        // Try to put the Fixed64Value (I64) from the source message
        // Into the Fixed32Value (I32) of the target message
        // Specifying the I32 wire format

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(4000, Enums.WireType.I32, 2000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

    [Fact]
    public async Task ThrowIfTheSourceWireFormatDoesNotMatch_LenToI32()
    {
        // Try to put the StringValue (Len) from the source message
        // Into the Fixed32Value (I32) of the target message
        // Specifying the I32 wire format

        int stringLength;
        do
        {
            stringLength = 30.GetRandom(1);
        } while (stringLength == 4); // Test will fail with 4 characters

        var sourceData = new Builders.ProtobufAllTypesBuilder()
            .UseRandomValues()
            .StringValue(string.Empty.GetRandom(stringLength))
            .Build();

        var targetMapping = new MappingBuilder()
            .AddUnmodifiedSourceField(4000, Enums.WireType.I32, 3000)
            .Build();

        var sourceMessage = sourceData.ToByteArray();
        Log.Verbose("SourceMessage: {SourceMessage}", Convert.ToBase64String(sourceMessage));

        var target = new ProtobufMapper();
        _ = await Assert.ThrowsAsync<Exceptions.WireTypeMismatchException>(() => target.MapAsync(sourceMessage, targetMapping));
    }

}
