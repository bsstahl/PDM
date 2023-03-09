namespace Protot.Entities;

internal class FileSection
{
    internal FileSectionType FileSectionType { get; set; }
    
    internal IList<string> Lines { get; set; }
}