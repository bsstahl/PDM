﻿using PDM.Enums;
using System.Text.Json.Serialization;

namespace PDM.Entities;

public sealed record Tag
{
    public Tag(int fieldNumber, WireType wireType)
    {
        this.FieldNumber = fieldNumber;
        this.WireType = wireType;
    }

    public int FieldNumber { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WireType WireType { get; set; }

    public Varint AsVarint()
    {
        UInt64 value = Convert.ToUInt64((this.FieldNumber << 3) | (byte)this.WireType);
        return new Varint(value);
    }

    public static Tag? Parse(Varint vint)
    {
        Tag? result = null;

        if (vint is null)
            throw new ArgumentNullException(nameof(vint));
        else if (vint.Value < int.MaxValue)
        {
            var vintValue = vint.Value;
            var wireType = (WireType)Convert.ToByte(vintValue & 7);
            var fieldNumber = Convert.ToInt32(vintValue >> 3);
            result = new Tag(fieldNumber, wireType);
        }

        return result;
    }

}
