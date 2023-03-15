using Protot.Extensions;

namespace Protot.Entities;

internal sealed class PrototOptions
{
    public string? SourcePath { get; set; } = string.Empty;
    public string? TargetPath { get; set; } = string.Empty;
    
    public string? TransformationConfigPath{ get; set; } = string.Empty;
    
    public string? TransformationConfigOutPath{ get; set; } = string.Empty;
    
    public bool PrintHelp { get; set; }

    public bool Validate()
    {
        ValidateProtoFile(this.SourcePath, nameof(this.SourcePath));
        ValidateProtoFile(this.TargetPath, nameof(this.TargetPath));
        ValidateJsonFile(this.TransformationConfigPath, nameof(this.TransformationConfigPath));
        ValidateAndCreateJsonFile(this.TransformationConfigOutPath, nameof(this.TransformationConfigOutPath));
        return true;
    }

    private bool ValidateProtoFile(string? fileName, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.WriteLine($"{fieldName} is not valid path");
            return false;
        }
        
        if (!fileName.IsValidProtoFile())
        {
            Console.WriteLine($"{fieldName} is not valid proto file");
            return false;
        }

        if (File.Exists(fileName)) return true;
        Console.WriteLine($"{fieldName} is not exists");
        return false;

    }
    private bool ValidateJsonFile(string? path, string fieldName)
    {
        if (!ValidateJsonFileInternal(path, fieldName)) return false;

        if (File.Exists(path)) return true;
        Console.WriteLine($"{fieldName} is not exists");
        return false;

    }

    private static bool ValidateJsonFileInternal(string? path, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine($"{fieldName} is not valid path");
            return false;
        }

        if (path.IsValidJsonFile()) return true;
        Console.WriteLine($"{fieldName} is not valid proto file");
        return false;

    }

    private bool ValidateAndCreateJsonFile(string? path, string fieldName)
    {
        if (!ValidateJsonFileInternal(path, fieldName)) return false;

        if (File.Exists(path)) return true;
        if (path != null) File.Create(path);

        return true;
    }
}