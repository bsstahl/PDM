using PDM.Entities;
using Protot.Core.Entities;
using System.Text.Json;

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

    internal static async Task<string> ReadFileText(this string filePath)
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