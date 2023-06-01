namespace Protot.Core.Entities;

public class ProtoReferenceFile
{
    public ProtoReferenceFile(string content, string referencePath)
    {
        this.ReferencePath = referencePath;
        this.Content = content;
    }
    public string Content { get; set; }
    
    public string ReferencePath { get; set; }
}