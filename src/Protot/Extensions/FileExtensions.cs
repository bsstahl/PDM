using System.Text;
using System.Text.Json;
using PDM.Entities;
using Protot.Core.Entities;

namespace Protot.Extensions;

internal static class FileExtensions
{
    internal static bool IsValidProtoFile(this string filePath)
    {
        return filePath.EndsWith(".proto");
    }
    
    internal static bool IsValidJsonFile(this string filePath)
    {
        return filePath.EndsWith(".json");
    }

    internal static async Task<ProtoFile> MapToProtoFile(this string filePath)
    {
        var protoContent = await ReadFileText(filePath);
        var directoryInfo = new DirectoryInfo(filePath).Parent;
        List<ProtoReferenceFile> referenceFiles = new List<ProtoReferenceFile>();
        var rootRefPath = directoryInfo.FullName;
        foreach (var directory in directoryInfo!.EnumerateDirectories("*.*", SearchOption.AllDirectories))
        {
            foreach (var file in directory.EnumerateFiles("*.proto"))
            {
                var fileContent = await ReadFileText(file.FullName);
                referenceFiles.Add(new ProtoReferenceFile(fileContent, $"{directory.FullName.Replace(rootRefPath, string.Empty, StringComparison.InvariantCultureIgnoreCase)}/{file.Name}"));
            }      
        }
      
        return new ProtoFile(protoContent, referenceFiles);
    }


    private static async Task<string> ReadFileText(this string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }
    
    internal static async Task<IEnumerable<ProtoTransformation>?> ParseProtoTransformation(this string filePath)
    {
        var fileContent = await ReadFileText(filePath);
        return JsonSerializer.Deserialize<IEnumerable<ProtoTransformation>>(fileContent);
    }
    
    internal static async Task<bool> WriteTransformation(this IEnumerable<Transformation>? transformations, string filePath)
    {
        if (transformations == null) return false;
        var content = JsonSerializer.Serialize(transformations);
        await File.WriteAllTextAsync(filePath, content);
        return true;

    }
}