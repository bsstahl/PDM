namespace Protot.Core.Entities;

public class ProtoFile
{
    public ProtoFile(string content, IEnumerable<ProtoReferenceFile> referenceFiles)
    {
        this.Content = content;
        this.ReferenceFiles = referenceFiles;
        this.Validate();
    }
    
    public ProtoFile(string content)
    {
        this.Content = content;
        this.Validate();
    }

    public string Content { get; }
    public IEnumerable<ProtoReferenceFile>? ReferenceFiles { get; }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(this.Content))
        {
            throw new ArgumentNullException($"{this.Content} is empty");
        }

        if (this.ReferenceFiles == null) {return;}
        foreach (var file in this.ReferenceFiles)
        {
            if (string.IsNullOrWhiteSpace(file.Content))
            {
                throw new ArgumentNullException($"{file.Content} is empty");
            }
                
            if (string.IsNullOrWhiteSpace(file.ReferencePath))
            {
                throw new ArgumentNullException($"{file.ReferencePath} is empty");
            }
        }
    }
}