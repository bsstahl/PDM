using PDM.Entities;

namespace PDM.Parser.Extensions;

internal static class ByteExtensions
{
    internal static (MessageField, int) ParseVarint(this byte[] remainingBytes, Tag tag)
    {
        var payload = Varint.Parse(remainingBytes);
        var messageField = new MessageField(tag.FieldNumber, tag.WireType, payload.Value);
        return (messageField, payload.WireLength);
    }
}
