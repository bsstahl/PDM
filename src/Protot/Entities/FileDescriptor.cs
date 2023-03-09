using Protot.Constants;

namespace Protot.Entities;

public sealed class FileDescriptor
{
    internal ProtoType? ProtoType { get; set; }
    
    internal string? Namespace { get; set; }
    
    internal IList<string>? Imports { get; set; }
    
    internal IDictionary<string, ProtoEnum>? Enums { get; set; }
    
    internal IDictionary<string, ProtoMessage>? Messages { get; set; }
}