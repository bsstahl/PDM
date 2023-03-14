using PDM.Enums;

namespace Protot.Extensions;
using Google.Protobuf.Reflection;

internal static class TypeExtensions
{
    internal static WireType ToWireType(this FieldDescriptorProto fieldDescriptor)
    {
        if (fieldDescriptor.HasTypeName)
        {
            return  WireType.Len;
        }

        if (fieldDescriptor is {HasLabel: true, Label: FieldDescriptorProto.Types.Label.Repeated})
        {
            return WireType.Len;
        }
        switch (fieldDescriptor.Type)
        {
            case FieldDescriptorProto.Types.Type.Int32:
            case FieldDescriptorProto.Types.Type.Int64:
            case FieldDescriptorProto.Types.Type.Uint32:
            case FieldDescriptorProto.Types.Type.Uint64:
            case FieldDescriptorProto.Types.Type.Sint32:
            case FieldDescriptorProto.Types.Type.Sint64:
            case FieldDescriptorProto.Types.Type.Bool:
            case FieldDescriptorProto.Types.Type.Enum:
                return WireType.VarInt;
            case FieldDescriptorProto.Types.Type.Fixed64:
            case FieldDescriptorProto.Types.Type.Sfixed64:
            case FieldDescriptorProto.Types.Type.Double:
                return WireType.I64;
            case FieldDescriptorProto.Types.Type.String:
            case FieldDescriptorProto.Types.Type.Bytes:
                return WireType.Len;
            case FieldDescriptorProto.Types.Type.Fixed32:
            case FieldDescriptorProto.Types.Type.Sfixed32:
            case FieldDescriptorProto.Types.Type.Float:
                return WireType.I32;
            case FieldDescriptorProto.Types.Type.Group:
            case FieldDescriptorProto.Types.Type.Message:
            default:
                throw new ArgumentOutOfRangeException(nameof(fieldDescriptor.Type), fieldDescriptor.Type, null);
        }
    }

    internal static WireType ToWireType(this string typeString)
    {
        switch (typeString.ToLowerInvariant())
        {
            case "int32":
            case "int64":
            case "uint32":
            case "uint64":
            case "sint32":
            case "sint64":
            case "bool":
                return WireType.VarInt;
            case "fixed64":
            case "sfixed64":
            case "double":
                return WireType.I64;
            case "string":
            case "bytes":
                return WireType.Len;
            case "fixed32":
            case "sfixed32":
            case "float":
                return WireType.I32;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeString), typeString, null);
        }
    }
}