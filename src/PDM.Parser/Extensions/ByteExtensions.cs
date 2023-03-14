using PDM.Entities;

namespace PDM.Parser.Extensions;

internal static class ByteExtensions
{
    internal static (SourceMessageField, int) ParseVarint(this byte[] remainingBytes, Tag tag)
    {
        var payload = Varint.Parse(remainingBytes);
        var messageField = new SourceMessageField(tag.FieldNumber.ToString(), tag.WireType, payload.Value);
        return (messageField, payload.WireLength);
    }
}
