namespace Protot.Core.Entities;

internal sealed class EnumValue
{
    internal string Name { get; }
    internal int Number { get; }

    internal EnumValue(string name, int number)
    {
        this.Name = name;
        this.Number = number;
    }
}