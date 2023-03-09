namespace Protot;

public class ProtoFileParser
{
    private string protoText;
    public ProtoFileParser(string protoText)
    {
        if (string.IsNullOrWhiteSpace(this.protoText))
        {
            throw new ArgumentNullException($"Provide inputProtoText");
        }
        this.protoText = protoText;
    }

    public async Task ParseFileAsync()
    {
        if (!File.Exists(GetTempProtoFilePath()))
        {
            throw new FileNotFoundException("Unable to find Temp ProtoFile");
        }
    }
    
}