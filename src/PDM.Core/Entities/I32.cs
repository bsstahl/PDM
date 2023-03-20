﻿using PDM.Extensions;
using System.Globalization;

namespace PDM.Entities;

public sealed record I32
{
    private readonly byte[] _rawData = new byte[] { 0, 0, 0, 0 };

    public byte[] GetRawData()
        => _rawData;

    public int WireLength
        => _rawData.Length;

    internal UInt32 Value
        => CalculateValue(_rawData);


    internal I32(byte[] rawData)
        => _rawData = rawData;

    public I32(int value)
        : this((uint)value)
    { }

    public I32(float value) 
        => _rawData = value.ToLittleEndianBytes();

    internal I32(UInt32 value)
        => _rawData = value.ToLittleEndianBytes();

    public static I32 Parse(byte[] message)
    {
        return message is null || message.Length != 4
            ? throw new ArgumentException($"{nameof(message)} for an I32 must have a length of 4")
            : new I32(message);
    }

    public static I32 Create(object value)
    {
        return value switch
        {
            int => new I32((int)value),
            uint => new I32((uint)value),
            float => new I32((float)value),
            string => I32.CreateFromString((string)value ?? string.Empty),
            _ => throw new NotImplementedException($"{value} ({value.GetType().Name}) cannot be parsed as a numeric")
        };
    }

    private static I32 CreateFromString(string value)
    {
        I32 result;
        if (int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
            result = new I32(longValue);
        else if (uint.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var ulongValue))
            result = new I32(ulongValue);
        else if (float.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
            result = new I32(doubleValue);
        else
            throw new NotImplementedException("{value} cannot be parsed as a numeric");
        return result;
    }

    private static UInt32 CalculateValue(byte[] value)
        => BitConverter.ToUInt32(value);

    public static I32 Empty
        => new I32(0);
}

