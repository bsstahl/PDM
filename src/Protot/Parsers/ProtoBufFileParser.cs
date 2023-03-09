using System.Text;
using Protot.Builder;
using Protot.Constants;
using Protot.Entities;
using Protot.Exceptions;

namespace Protot;

public sealed class ProtoBufFileParser
{
    private string _protoContent;
    public ProtoBufFileParser(string protoContent)
    {
        if (string.IsNullOrWhiteSpace(protoContent))
        {
            throw new ProtoFileParserException($"{protoContent} is required");
        }
        
        this._protoContent = protoContent;
    }
    public async Task<FileDescriptor> Parse()
    {
        var sections = await MapToFileSections();
        var fileDescriptorBuilder = new FileDescriptorBuilder();
        foreach (var section in sections)
        {
            switch (section.FileSectionType)
            {
                case FileSectionType.Syntax:
                    fileDescriptorBuilder.AddProtoType(section);
                    break;
                case FileSectionType.Namespace:
                    fileDescriptorBuilder.AddNamespace(section);
                    break;
                case FileSectionType.Imports:
                    break;
                case FileSectionType.Message:
                    fileDescriptorBuilder.AddMessages(section);
                    break;
                case FileSectionType.Enum:
                    fileDescriptorBuilder.AddEnum(section);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return fileDescriptorBuilder.AssignMessageType().Build();
    }

    private async Task<List<FileSection>> MapToFileSections()
    {
        List<FileSection> sections = new List<FileSection>();
        string formattedText = await this.CleanAndFormat();
        using var reader = new StringReader(formattedText);
        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            line = line.Trim();
            if (line.StartsWith(ProtoIdentifiers.Syntax))
            {
                sections.Add(new FileSectionBuilder().Create(FileSectionType.Syntax).AddLine(line.Trim()).Build());
            }

            if (line.StartsWith(ProtoIdentifiers.Namespace))
            {
                sections.Add(new FileSectionBuilder().Create(FileSectionType.Namespace).AddLine(line.Trim()).Build());
            }

            if (line.StartsWith(ProtoIdentifiers.Message) || line.StartsWith(ProtoIdentifiers.Enum))
            {
                var fileSection = await this.ParseMessageOrEnumSection(line, reader);
                sections.Add(fileSection);
            }
        }

        return sections;
    }

    private async Task<FileSection> ParseMessageOrEnumSection(string? line, TextReader reader)
    {
        var fileSectionBuilder = new FileSectionBuilder();
         if (!string.IsNullOrWhiteSpace(line))
        {
            fileSectionBuilder.Create(line.StartsWith(ProtoIdentifiers.Message)? FileSectionType.Message : FileSectionType.Enum);
            fileSectionBuilder.AddLine(line);
        }
      
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.StartsWith("}"))
            {
                break;
            }

            if (line == "{")
            {
                continue;
            }
            fileSectionBuilder.AddLine(line);
        }

        return fileSectionBuilder.Build();
    }
    
    private async Task<string> CleanAndFormat()
    {
        string? line;
        var stringBuilder = new StringBuilder();
        using var reader = new StringReader(this._protoContent);
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int index = 0;
            foreach (var character in line.Trim().TakeWhile(character => character is not ('/' or '*')))
            {
                if (index == 0 && character == ' ')
                {
                    continue;
                }
                switch (character)
                {
                    case '{' or '}':
                        stringBuilder.Append('\n').Append(character).Append('\n');
                        break;
                    case ';':
                        stringBuilder.Append('\n');
                        break;
                    default:
                        stringBuilder.Append(character);
                        break;
                }

                index++;
            }
        }

        return stringBuilder.ToString();
    }
}