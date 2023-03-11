using PDM.TestUtils.Extensions;

namespace PDM.TestUtils.Builders;

[ExcludeFromCodeCoverage]
public class ProtobufAllTypesBuilder
{
    readonly ProtoBuf.AllTypes _value = new();

    public ProtoBuf.AllTypes Build() => _value;

    public ProtobufAllTypesBuilder Int32Value(int value)
    {
        _value.Int32Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder Int64Value(Int64 value)
    {
        _value.Int64Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder UInt32Value(UInt32 value)
    {
        _value.UInt32Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder UInt64Value(UInt64 value)
    {
        _value.UInt64Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder SInt32Value(int value)
    {
        _value.SInt32Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder SInt64Value(Int64 value)
    {
        _value.SInt64Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder BoolValue(bool value)
    {
        _value.BoolValue = value;
        return this;
    }

    public ProtobufAllTypesBuilder EnumValue(ProtoBuf.SampleEnum value)
    {
        _value.EnumValue = value;
        return this;
    }

    public ProtobufAllTypesBuilder Fixed64Value(ulong value)
    {
        _value.Fixed64Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder SFixed64Value(long value)
    {
        _value.SFixed64Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder DoubleValue(double value)
    {
        _value.DoubleValue = value;
        return this;
    }

    public ProtobufAllTypesBuilder StringValue(string value)
    {
        _value.StringValue = value;
        return this;
    }

    public ProtobufAllTypesBuilder BytesValue(Google.Protobuf.ByteString value)
    {
        _value.BytesValue = value;
        return this;
    }

    public ProtobufAllTypesBuilder BytesValue(byte[] value)
    {
        return this
            .BytesValue(Google.Protobuf.ByteString.FromBase64(Convert.ToBase64String(value)));
    }

    public ProtobufAllTypesBuilder EmbeddedMessageValue(ProtoBuf.SampleEmbeddedMessage value)
    {
        _value.EmbeddedMessageValue = value;
        return this;
    }

    public ProtobufAllTypesBuilder EmbeddedMessageValue(int embeddedInt32Value, string embeddedStringValue)
    {
        return this.EmbeddedMessageValue(new ProtoBuf.SampleEmbeddedMessage()
        {
            EmbeddedInt32Value = embeddedInt32Value,
            EmbeddedStringValue = embeddedStringValue
        });
    }

    public ProtobufAllTypesBuilder AddRepeatedInt32Value(int value)
    {
        _value.RepeatedInt32Value.Add(value);
        return this;
    }

    public ProtobufAllTypesBuilder AddRandomRepeatedInt32Value(int count = 0)
    {
        var repeatCount = (count == 0) ? 10.GetRandom(3) : count;
        for (int i = 0; i < repeatCount; i++)
            _ = this.AddRepeatedInt32Value(int.MaxValue.GetRandom());
        return this;
    }

    public ProtobufAllTypesBuilder Fixed32Value(uint value)
    {
        _value.Fixed32Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder SFixed32Value(int value)
    {
        _value.SFixed32Value = value;
        return this;
    }

    public ProtobufAllTypesBuilder FloatValue(float value)
    {
        _value.FloatValue = value;
        return this;
    }

    public ProtobufAllTypesBuilder UseRandomValues()
    {
        return this
            .Int32Value(int.MaxValue.GetRandom())
            .Int64Value(long.MaxValue.GetRandom())
            .UInt32Value(Convert.ToUInt32(int.MaxValue.GetRandom(0)))
            .UInt64Value(Convert.ToUInt64(long.MaxValue.GetRandom(0)))
            .SInt32Value(int.MaxValue.GetRandom())
            .SInt64Value(long.MaxValue.GetRandom())
            .BoolValue(true.GetRandom())
            .EnumValue((ProtoBuf.SampleEnum)2.GetRandom())
            .Fixed64Value(Convert.ToUInt64(long.MaxValue.GetRandom(0)))
            .SFixed64Value(long.MaxValue.GetRandom())
            .DoubleValue(double.MaxValue.GetRandom())
            .StringValue(string.Empty.GetRandom())
            .BytesValue(Array.Empty<byte>().GetRandom())
            .EmbeddedMessageValue(int.MaxValue.GetRandom(), string.Empty.GetRandom())
            .AddRandomRepeatedInt32Value()
            .Fixed32Value(Convert.ToUInt32(int.MaxValue.GetRandom(0)))
            .SFixed32Value(int.MaxValue.GetRandom())
            .FloatValue(float.MaxValue.GetRandom());
    }
}
