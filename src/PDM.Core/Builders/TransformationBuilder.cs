using PDM.Constants;
using PDM.Entities;
using PDM.Enums;
using PDM.Extensions;
using System.Globalization;

namespace PDM.Builders;

public class TransformationBuilder
{
    private readonly List<Transformation> _transformations = new();

    public IEnumerable<Transformation> Build() => _transformations;

    public TransformationBuilder AddTransformation(TransformationType transformationType, string subType, string value)
    {
        _transformations.Add(new Transformation()
        {
            TransformationType = transformationType,
            SubType = subType,
            Value = value
        });
        return this;
    }

    // ***

    public TransformationBuilder ReplaceField(string subType, string value)
    {
        return this
            .AddTransformation(TransformationType.ReplaceField, subType, value);
    }

    public TransformationBuilder BlacklistField(int fieldNumber)
    {
        return this
            .ReplaceField(TransformationSubtype.Blacklist, fieldNumber.ToString(CultureInfo.InvariantCulture));
    }

    public TransformationBuilder RenameField(int sourceFieldNumber, int targetFieldNumber)
    {
        var source = sourceFieldNumber.ToString(CultureInfo.InvariantCulture);
        var target = targetFieldNumber.ToString(CultureInfo.InvariantCulture);
        return this
            .ReplaceField(TransformationSubtype.Renames, $"{source}:{target}");
    }

    public TransformationBuilder RenameField(string sourceField, int targetFieldNumber)
    {
        var target = targetFieldNumber.ToString(CultureInfo.InvariantCulture);
        return this.ReplaceField(
            TransformationSubtype.Renames, $"{sourceField}:{target}");
    }

    public TransformationBuilder RenameFields(string value)
    {
        return this
            .ReplaceField(TransformationSubtype.Renames, value);
    }

    public TransformationBuilder IncludeField(int fieldNumber)
    {
        return this
            .IncludeFields(new int[] { fieldNumber });
    }

    public TransformationBuilder IncludeFields(IEnumerable<int> fieldNumbers)
    {
        return this
            .IncludeFields(string.Join(',', fieldNumbers));
    }

    public TransformationBuilder IncludeFields(string fieldNumbersList)
    {
        return this
            .ReplaceField(TransformationSubtype.Include, fieldNumbersList);
    }

    // ***

    public TransformationBuilder InsertField(string transformationSubType, int fieldNumber, WireType wireType, object value)
    {
        return value is null
            ? throw new ArgumentNullException(nameof(value))
            : this.AddTransformation(TransformationType.InsertField, transformationSubType, $"{fieldNumber}:{wireType}:{value}");
    }

    public TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, string value)
    {
        return this
            .InsertField(TransformationSubtype.Static, fieldNumber, wireType, value);
    }

    public TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, int value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, value.ToString(CultureInfo.InvariantCulture));
    }

    internal TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, uint value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, value.ToString(CultureInfo.InvariantCulture));
    }

    public TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, bool value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, value.ToString(CultureInfo.InvariantCulture));
    }

    public TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, long value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, value.ToString(CultureInfo.InvariantCulture));
    }

    internal TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, ulong value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, value.ToString(CultureInfo.InvariantCulture));
    }

    public TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, float value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, value.ToString(CultureInfo.InvariantCulture));
    }

    public TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, double value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, value.ToString(CultureInfo.InvariantCulture));
    }

    public TransformationBuilder InsertStaticField(int fieldNumber, WireType wireType, byte[] value)
    {
        return this
            .InsertStaticField(fieldNumber, wireType, Convert.ToHexString(value));
    }

    public TransformationBuilder InsertStaticFieldSignedVarint(int fieldNumber, int value)
    {
        return this
            .InsertStaticField(fieldNumber, WireType.VarInt, value.ZZEncode());
    }

    public TransformationBuilder InsertStaticFieldSignedVarint(int fieldNumber, long value)
    {
        return this
            .InsertStaticField(fieldNumber, WireType.VarInt, value.ZZEncode());
    }
}
