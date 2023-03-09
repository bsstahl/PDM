using Protot.Enum;

namespace Protot.Entities;

internal class Field
{
   internal WireType WireType { get; set; }
    
    internal string Name { get; set; }
    
    internal int? Index { get; set; }
    
    internal string? TypeAsString { get; set; }
}