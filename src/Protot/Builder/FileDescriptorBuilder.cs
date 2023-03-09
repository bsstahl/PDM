using Protot.Constants;
using Protot.Entities;
using Protot.Enum;
using Protot.Exceptions;
using Protot.Extensions;

namespace Protot.Builder;

internal class FileDescriptorBuilder
{
    private readonly FileDescriptor _fileDescriptor = new();

    public FileDescriptor Build() => _fileDescriptor;

    public FileDescriptorBuilder AddProtoType(FileSection fileSection)
    {
        if (_fileDescriptor.ProtoType != null)
        {
            throw new ProtoFileParserException("Duplicate Proto Type defined");
        }
        var syntaxLine = fileSection.Lines.FirstOrDefault();
        var expression = syntaxLine?.ToProtoExpression();
        if (expression == null)
        {
            return this;
        }
        if (expression.Right.EqualValue(ProtoIdentifiers.Proto3))
        {
            _fileDescriptor.ProtoType = ProtoType.Proto3;
            return this;
        }
        if (expression.Right.EqualValue(ProtoIdentifiers.Proto2))
        {
            _fileDescriptor.ProtoType = ProtoType.Proto2;
            return this;
        }
        _fileDescriptor.ProtoType = ProtoType.Proto2;
        return this;
    }
    
    public FileDescriptorBuilder AddNamespace(FileSection fileSection)
    {
        if (_fileDescriptor.Namespace != null)
        {
            throw new ProtoFileParserException("Duplicate Namespace defined");
        }
        var expression = fileSection.Lines.FirstOrDefault()?.ToProtoExpression();
        if (expression == null)
        {
            return this;
        }

        _fileDescriptor.Namespace = expression.Right;
        return this;
    }
    
    public FileDescriptorBuilder AddImport(string import)
    {
        _fileDescriptor.Imports ??= new List<string>();
        _fileDescriptor.Imports.Add(import);
        return this;
    }
    
    public FileDescriptorBuilder AddMessages(FileSection fileSections)
    {
        _fileDescriptor.Messages ??= new Dictionary<string, ProtoMessage>();
        var message = fileSections.Lines.ToProtoMessage();
        if (message == null)
        {
            return this;
        }
        if (_fileDescriptor.Messages.ContainsKey(message.MessageType))
        {
            throw new ProtoFileParserException($"{message.MessageType} appeared multiple times");
        }
        _fileDescriptor.Messages.Add(message.MessageType, message);
        return this;
    }
    
    public FileDescriptorBuilder AddEnum(FileSection fileSections)
    {
        _fileDescriptor.Enums ??= new Dictionary<string, ProtoEnum>();
        var protoEnum = fileSections.Lines.ToProtoEnum();
        if (protoEnum == null)
        {
            return this;
        }
        if (_fileDescriptor.Enums.ContainsKey(protoEnum.Name))
        {
            throw new ProtoFileParserException($"{protoEnum.Name} appeared multiple times");
        }
        _fileDescriptor.Enums.Add(protoEnum.Name, protoEnum);
        return this;
        return this;
    }

    public FileDescriptorBuilder AssignMessageType()
    {
        if (_fileDescriptor.Messages == null)
        {
            throw new ProtoFileParserException($"No Message Found");
        }
        foreach (var message in _fileDescriptor.Messages)
        {
            foreach (var field in message.Value.Fields)
            {
                if (string.IsNullOrWhiteSpace(field.TypeAsString))
                {
                    throw new ProtoFileParserException($"{field.TypeAsString} is empty");
                }

                var wireType = field.TypeAsString.GetWireType(_fileDescriptor.Enums, _fileDescriptor.Messages);
                field.WireType = wireType;
            }
        }
        return this;
    }
}

