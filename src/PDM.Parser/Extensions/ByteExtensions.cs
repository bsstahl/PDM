using PDM.Entities;
using System.Globalization;

namespace PDM.Parser.Extensions;

internal static class ByteExtensions
{
    internal static (SourceMessageField, int) ParseVarint(this byte[] remainingBytes, Tag tag)
    {
        var payload = Varint.Parse(remainingBytes);
        var key = tag.FieldNumber.ToString(CultureInfo.InvariantCulture);
        var messageField = new SourceMessageField(key, tag.WireType, payload.Value);
        return (messageField, payload.WireLength);
    }
}
