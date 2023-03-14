namespace Protot.Entities;

internal sealed class ProtoEnum
{
    internal string Name { get; set; } = string.Empty;

    internal IList<EnumValue> Values { get; set; } = Array.Empty<EnumValue>();
}