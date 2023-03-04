using PDM.Constants;
using PDM.Entities;
using PDM.Enums;
using System.Buffers.Text;
using System.Globalization;
using System.Text;

namespace PDM.Builders;

public class TransformationBuilder
{
    private readonly CultureInfo _formatProvider = CultureInfo.InvariantCulture;
    private readonly List<Transformation> _transformations = new();

    public IEnumerable<Transformation> Build() => _transformations;

    public TransformationBuilder ReplaceField(string subType, string value)
    {
        _transformations.Add(new Transformation()
        {
            TransformationType = TransformationType.ReplaceField,
            SubType = subType,
            Value = value
        });
        return this;
    }

    public TransformationBuilder BlacklistField(int fieldNumber)
    {
        return this
            .ReplaceField(TransformationSubtype.Blacklist, fieldNumber.ToString(_formatProvider));
    }

    public TransformationBuilder RenameField(int sourceFieldNumber, int targetFieldNumber)
    {
        var source = sourceFieldNumber.ToString(_formatProvider);
        var target = targetFieldNumber.ToString(_formatProvider);
        return this
            .ReplaceField(TransformationSubtype.Renames, $"{source}:{target}");
    }

    public TransformationBuilder RenameFields(string value)
    {
        return this
            .ReplaceField(TransformationSubtype.Renames, value);
    }

    public TransformationBuilder IncludeField(int fieldNumber)
    {
        return this.IncludeFields(new int[] { fieldNumber });
    }

    public TransformationBuilder IncludeFields(IEnumerable<int> fieldNumbers)
    {
        return this.IncludeFields(string.Join(',', fieldNumbers));
    }

    public TransformationBuilder IncludeFields(string fieldNumbersList)
    {
        return this.ReplaceField(TransformationSubtype.Include, fieldNumbersList);
    }

    public TransformationBuilder InsertField(int targetFieldNumber, string value)
    {
        var target = targetFieldNumber.ToString(_formatProvider);
        var valueBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        _transformations.Add(new Transformation()
        {
            TransformationType = TransformationType.InsertField,
            Value = $"{target}:{valueBase64}"
        });
        return this;
    }
}
