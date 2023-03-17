using Protot.Core.Entities;
using Protot.Core.Exceptions;

namespace Protot.Core.Extensions;

internal static class ProtoFileDescriptorExtensions
{
    internal static string TryGetFieldNumber(
        this string fieldName,
        ProtoFileDescriptor fileDescriptor)
    {

       List<int> fieldNumbers = new List<int>();
        var embeddedFieldNames = fieldName.EmbeddedFieldNames();
        foreach (var embeddedField in embeddedFieldNames)
        {
            var fieldInfo = embeddedField.SearchField(fileDescriptor);
            if (fieldInfo == null)
            {
                throw new PrototMapperException($"{fieldName} not exist in message");
            }

            fieldNumbers.Add(fieldInfo.FieldNumber);
        }

        return string.Join('.', fieldNumbers);
    }

    private static ProtoMessageField? SearchField(this string fieldName, ProtoFileDescriptor fileDescriptor)
    {
        foreach (var message in fileDescriptor.Messages)
        {
            message.Value.Fields.TryGetValue(fieldName, out var field);
            if (field != null)
            {
                return field;
            }
        }

        return null;
    }
}