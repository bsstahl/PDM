
using Protot.Entities;
using Protot.Enum;

namespace Protot.Extensions;

internal static class ProtoTypeExtensions
{
    internal static WireType GetWireType(
        this string type, 
        IDictionary<string, ProtoEnum>? enums, 
        IDictionary<string, ProtoMessage>? messages)
    {
        if (type.ContainsValue("repeated"))
        {
            return WireType.Len;
        }
        
        if (enums != null && enums.ContainsKey(type))
        {
            return WireType.VarInt;
        }
        
        if (messages != null && messages.ContainsKey(type))
        {
            return WireType.Len;
        }

        switch (type.ToLowerInvariant())
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
                   throw new InvalidDataException("Invalid Data type");
        }
    }
}