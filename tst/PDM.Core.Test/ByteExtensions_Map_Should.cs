namespace PDM.Core.Test;

public class ByteExtensions_Map_Should
{
    [Fact]
    public void ProperlyMapATypeToTheSameType()
    {
        var sourceData = new ProtoBuf.TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };
        
        var sourceMessage = sourceData.ToByteArray();

        var protoFilePath = @"../../../ProtoBuf/TwoFields.proto";
        var description = System.IO.File.ReadAllText(protoFilePath);

        var actual = sourceMessage.Map(description, description, string.Empty);
        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
    }
}
