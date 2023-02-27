namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class ProtobufMapper_MapAsync_Should
{
    [Fact]
    public async Task ThrowIfNoSourceMessageSupplied()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        byte[]? sourceMessage = null;

        var target = new ProtobufMapper(ProtoBuf.TwoFields.Descriptor, ProtoBuf.TwoFields.Descriptor);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => target.MapAsync(sourceMessage!, string.Empty));
        Assert.Equal("sourceMessage", ex.ParamName);
    }

    [Fact]
    public async Task ThrowIfNoSourceDescriptorSupplied()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(null!, ProtoBuf.TwoFields.Descriptor);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => target.MapAsync(sourceMessage, string.Empty));
        Assert.Equal("sourceDescriptor", ex.ParamName);
    }

    [Fact]
    public async Task ThrowIfNoTargetDescriptorSupplied()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(ProtoBuf.TwoFields.Descriptor, null!);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => target.MapAsync(sourceMessage, string.Empty));
        Assert.Equal("targetDescriptor", ex.ParamName);
    }

    [Fact]
    public async Task ProperlyMapATypeToTheSameType()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };
        
        var sourceMessage = sourceData.ToByteArray();

        var target = new ProtobufMapper(ProtoBuf.TwoFields.Descriptor, ProtoBuf.TwoFields.Descriptor);
        var actual = await target.MapAsync(sourceMessage, string.Empty);
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

        var sourceMessage = sourceData.ToByteArray();

        var sourceDescriptor = ProtoBuf.ThreeFields.Descriptor;
        var targetDescriptor = ProtoBuf.TwoFields.Descriptor;

        var target = new ProtobufMapper(sourceDescriptor, targetDescriptor);
        var actual = await target.MapAsync(sourceMessage, string.Empty);
        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task IgnoreAFieldThatMatchesByNameButNotByType_FloatToString()
    {
        var sourceData = new ProtoBuf.ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var sourceDescriptor = ProtoBuf.ThreeFields.Descriptor;
        var targetDescriptor = ProtoBuf.MismatchedType.Descriptor;

        var target = new ProtobufMapper(sourceDescriptor, targetDescriptor);
        var actual = await target.MapAsync(sourceMessage, string.Empty);
        var actualData = ProtoBuf.MismatchedType.Parser.ParseFrom(actual);

        Assert.Equal(String.Empty, actualData.FloatValue);
        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }

    [Fact]
    public async Task IgnoreAFieldThatMatchesByNameButNotByType_StringToFloat()
    {
        var sourceData = new ProtoBuf.MismatchedType()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = float.MaxValue.GetRandom().ToString(),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var sourceDescriptor = ProtoBuf.MismatchedType.Descriptor;
        var targetDescriptor = ProtoBuf.ThreeFields.Descriptor;

        var target = new ProtobufMapper(sourceDescriptor, targetDescriptor);
        var actual = await target.MapAsync(sourceMessage, string.Empty);
        var actualData = ProtoBuf.ThreeFields.Parser.ParseFrom(actual);

        Assert.Equal(0.0, actualData.FloatValue);
        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }
}
