using Protot.Entities;
using Protot.Exceptions;

namespace Protot.Extensions;

internal static class ProtoFileDescriptorExtensions
{
    internal static int TryGetFieldNumber(
        this TransformationField transformationField,
        ProtoFileDescriptor fileDescriptor)
    {
        if (!fileDescriptor.Messages.TryGetValue(transformationField.MessageName, out var message))
        {
            throw new PrototMapperException($"{transformationField.MessageName} not exist in fileDescriptor");
        }
        if (!message.Fields.TryGetValue(transformationField.FieldName, out var field))
        {
            throw new PrototMapperException($"{transformationField.FieldName} not exist in message");
        }
        return field.FieldNumber;
    }
}