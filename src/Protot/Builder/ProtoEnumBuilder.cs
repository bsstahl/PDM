using Protot.Entities;

namespace Protot.Builder;

internal class ProtoEnumBuilder
{
    private readonly ProtoEnum _protoEnum = new();

    public ProtoEnum Build() => _protoEnum;

    public ProtoEnumBuilder CreateEnum(string name)
    {
        _protoEnum.Name = name;
        return this;
    }
    
    
    public ProtoEnumBuilder AddValue(string value)
    {
        _protoEnum.Values ??= new List<string>();
        _protoEnum.Values.Add(value);
        return this;
    }

}