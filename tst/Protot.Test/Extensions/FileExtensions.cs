namespace Protot.Test.Extensions;

[ExcludeFromCodeCoverage]
public static class FileExtensions
{
    public static async Task<string> GetProtoText(this string protoFileName)
    {
        return await File.ReadAllTextAsync(
            $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName}/ProtoBuf/{protoFileName}.proto");
    }
}