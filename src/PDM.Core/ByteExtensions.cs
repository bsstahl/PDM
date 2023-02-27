using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace PDM;

internal static class ByteExtensions
{
    internal static Task<byte[]> MapAsync(this byte[] sourceMessage, MessageDescriptor sourceDescriptor, MessageDescriptor targetDescriptor, string transformation)
    {
        if (sourceMessage is null) throw new ArgumentNullException(nameof(sourceMessage));
        if (sourceDescriptor is null) throw new ArgumentNullException(nameof(sourceDescriptor));
        if (targetDescriptor is null) throw new ArgumentNullException(nameof(targetDescriptor));

        var sourceProto = sourceDescriptor.Parser.ParseFrom(sourceMessage);
        var targetProto = targetDescriptor.Parser.ParseFrom(Array.Empty<byte>());

        foreach (var targetField in targetDescriptor.Fields.InFieldNumberOrder())
        {
            var sourceField = sourceDescriptor.FindFieldByName(targetField.Name);
            if (sourceField.FieldType == targetField.FieldType)
            {
                var targetFieldValue = sourceField.Accessor.GetValue(sourceProto);
                targetField.Accessor.SetValue(targetProto, targetFieldValue);
            }
        }

        return Task.FromResult<byte[]>(targetProto.ToByteArray());
    }
}
