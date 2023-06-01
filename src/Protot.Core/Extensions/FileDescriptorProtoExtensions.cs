using System.Runtime.CompilerServices;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Protot.Core.Builders;
using Protot.Core.Entities;
using Protot.Core.Exceptions;

namespace Protot.Core.Extensions;

internal static class FileDescriptorProtoExtensions
{
    internal static IEnumerable<FileDescriptorProto> ExtractFileInfo(this string fileDescriptorFile)
    {
        using var stream = File.OpenRead(fileDescriptorFile);
        FileDescriptorSet descriptorSet = FileDescriptorSet.Parser.ParseFrom(stream);
        var byteStrings = descriptorSet.File.Select(f => f.ToByteString()).ToList();
        var descriptors = FileDescriptor.BuildFromByteStrings(byteStrings);
        return descriptors.Select(x => x.ToProto());
    }
    
   internal static ProtoFileDescriptor ToProtoFileDescriptor(this IEnumerable<FileDescriptorProto> fileDescriptorProtos, string messageToTransform)
    {
        if (fileDescriptorProtos == null)
        {
            throw new PrototMapperException($"{nameof(fileDescriptorProtos)} is null");
        }
       
        var builder = new ProtoFileDescriptorBuilder();
      
       foreach (var fileDescriptor in fileDescriptorProtos)
        {
            if (fileDescriptor.EnumType is not null)
            {
                foreach (var enumType in fileDescriptor.EnumType)
                {
                    builder.AddEnum(enumType.ToProtoEnum());
                }
            }
            
            foreach (var message in fileDescriptor.MessageType)
            {
                if (message.Name == messageToTransform)
                {
                    builder.AddMessage(message.ToProtoMessage());
                }
                else
                {
                    builder.AddReferenceMessage(message.ToProtoMessage());
                }
            }
        }
  
        return builder.Build();
    }


    private static ProtoEnum? ToProtoEnum(this EnumDescriptorProto? descriptorProto)
    {
        if (descriptorProto is null)
        {
            return null;
        }

        var protoEnum = new ProtoEnum
        {
            Name = descriptorProto.Name,
            Values = new List<EnumValue>()
        };
        foreach (var field in descriptorProto.Value)
        {
            protoEnum.Values.Add(new EnumValue(field.Name, field.Number));
        }

        return protoEnum;
    }
    
    private static ProtoMessage? ToProtoMessage(this DescriptorProto? descriptorProto)
    {
        if (descriptorProto is null)
        {
            return null;
        }

        var protoMessage = new ProtoMessage()
        {
            Name = descriptorProto.Name,
            Fields = new Dictionary<string, ProtoMessageField>()
            
        };

        foreach (var field in descriptorProto.Field)
        {
            protoMessage.Fields.Add(field.Name, new ProtoMessageField(
                field.Name,
                field.Number,
                field.ToWireType()));
        }
        return protoMessage;
    }
}