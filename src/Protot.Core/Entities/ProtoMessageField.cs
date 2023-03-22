using PDM.Enums;

namespace Protot.Core.Entities;

internal sealed class ProtoMessageField
{
    internal WireType WireType { get; }

    internal string Name { get; }

    internal int FieldNumber { get; }


    internal ProtoMessageField(string name, int fieldNumber, WireType wireType)
    {
        this.Name = name;
        this.FieldNumber = fieldNumber;
        this.WireType = wireType;
    }
}