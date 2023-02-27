namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class ByteExtensions_Map_Should
{
    [Fact]
    public async Task ProperlyMapATypeToTheSameType()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };
        
        var sourceMessage = sourceData.ToByteArray();

        var actual = await sourceMessage.MapAsync(ProtoBuf.TwoFields.Descriptor, ProtoBuf.TwoFields.Descriptor, string.Empty);
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

        var actual = await sourceMessage.MapAsync(sourceDescriptor, targetDescriptor, string.Empty);
        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
        Assert.Equal(sourceData.StringValue, actualData.StringValue);
    }
}
