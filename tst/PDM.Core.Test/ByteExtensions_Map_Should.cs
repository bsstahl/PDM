using Google.Protobuf.WellKnownTypes;

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

    [Fact]
    public void ProperlyMapASubsetTypeToMatchingFields()
    {
        var sourceData = new ProtoBuf.ThreeFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            FloatValue = Convert.ToSingle(double.MaxValue.GetRandom()),
            StringValue = String.Empty.GetRandom()
        };

        var sourceMessage = sourceData.ToByteArray();

        var sourceDescriptionFilePath = @"../../../ProtoBuf/ThreeFields.proto";
        var sourceDescription = System.IO.File.ReadAllText(sourceDescriptionFilePath);
        var targetDescriptionFilePath = @"../../../ProtoBuf/TwoFields.proto";
        var targetDescription = System.IO.File.ReadAllText(targetDescriptionFilePath);

        var actual = sourceMessage.Map(sourceDescription, targetDescription, string.Empty);
        var actualData = ProtoBuf.TwoFields.Parser.ParseFrom(actual);

        Assert.Equal(sourceData.IntegerValue, actualData.IntegerValue);
    }
}
