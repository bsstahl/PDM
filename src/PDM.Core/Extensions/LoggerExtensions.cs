using Microsoft.Extensions.Logging;
using PDM.Constants;
using PDM.Entities;

namespace PDM.Extensions;

internal static class LoggerExtensions
{
    static readonly Action<ILogger, Exception?> _noLoggerProvidedMessage
        = LoggerMessage.Define(LogLevel.Warning,
                LogEventId.NoLoggerProvided,
                "No logger provided -- default logger used");

    static readonly Action<ILogger, string, string, Exception?> _methodEntryMessage
        = LoggerMessage.Define<string, string>(LogLevel.Debug,
                LogEventId.MethodEntry,
                "Entered {ObjectName}.{MethodName}");

    static readonly Action<ILogger, string, string, Exception?> _methodExitMessage
        = LoggerMessage.Define<string, string>(LogLevel.Debug,
                LogEventId.MethodExit,
                "Exiting {ObjectName}.{MethodName}");

    static readonly Action<ILogger, string, string, Exception?> _largeDataMessage
        = LoggerMessage.Define<string, string>(LogLevel.Debug,
                LogEventId.MethodExit,
                "{Type}: {Value}");

    static readonly Action<ILogger, string, Exception?> _RequiredFieldMissingMessage
        = LoggerMessage.Define<string>(LogLevel.Error, 
            LogEventId.FieldMissing, 
            "{FieldName} is required to perform a mapping");

    static readonly Action<ILogger, int, Enums.WireType, Exception?> _parsingFieldMessage
        = LoggerMessage.Define<int, Enums.WireType>(LogLevel.Information,
            LogEventId.ParsingField,
            "Parsing field {FieldNumber} with wiretype {WireType}");

    static readonly Action<ILogger, IEnumerable<MessageField>, Exception?> _parseMessageResultMessage
        = LoggerMessage.Define<IEnumerable<MessageField>>(LogLevel.Debug,
            LogEventId.ParseMessageResult,
            "Parse message result: {Result}");

    static readonly Action<ILogger, Enums.TransformationType, string, Exception?> _buildingMappingMessage
        = LoggerMessage.Define<Enums.TransformationType, string>(LogLevel.Information,
            LogEventId.BuildingMapping,
            "Modifying mappings for {TransformationType}.{TransformationSubtype}");

    static readonly Action<ILogger, Mapping, Exception?> _mappingBuiltMessage
        = LoggerMessage.Define<Mapping>(LogLevel.Debug,
            LogEventId.MappingBuilt,
            "Mapping {Mapping} built");

    static readonly Action<ILogger, int, Exception?> _noFieldToRemoveMessage
        = LoggerMessage.Define<int>(LogLevel.Warning,
            LogEventId.NoFieldToRemove,
            "Field {FieldNumber} not present to remove");

    static readonly Action<ILogger, Mapping, Exception?> _mappingRemovedMessage
        = LoggerMessage.Define<Mapping>(LogLevel.Debug,
            LogEventId.MappingRemoved,
            "Mapping {Mapping} removed");

    static readonly Action<ILogger, MessageField, Exception?> _fieldMappingProcessedMessage
        = LoggerMessage.Define<MessageField>(LogLevel.Information,
            LogEventId.FieldMappingProcessed,
            "Mapping of field {FieldMapping} completed");

    static readonly Action<ILogger, MessageField, IEnumerable<byte>, Exception?> _messageFieldExported
        = LoggerMessage.Define<MessageField, IEnumerable<byte>>(LogLevel.Information,
            LogEventId.MessageFieldExported,
            "Message field {Field} exported with value {Value}");

    static readonly Action<ILogger, MessageField, Exception?> _invalidMessageField
        = LoggerMessage.Define<MessageField>(LogLevel.Warning,
            LogEventId.InvalidMessageField,
            "Invalid message field {Field}");


    internal static void NoLoggerProvided(this ILogger logger)
        => _noLoggerProvidedMessage.Invoke(logger, null);

    internal static void LogMethodEntry(this ILogger logger, string objectName, string methodName)
        => _methodEntryMessage.Invoke(logger, objectName, methodName, null);

    internal static void LogMethodExit(this ILogger logger, string objectName, string methodName)
        => _methodExitMessage.Invoke(logger, objectName, methodName, null);

    internal static void LogLargeData(this ILogger logger, string objectType, string value)
        => _largeDataMessage.Invoke(logger, objectType, value, null);

    internal static void LogRequiredFieldMissing(this ILogger logger, string fieldName)
        => _RequiredFieldMissingMessage.Invoke(logger, fieldName, null);

    internal static void LogParsingField(this ILogger logger, Entities.Tag tag)
        => _parsingFieldMessage.Invoke(logger, tag.FieldNumber, tag.WireType, null);

    internal static void LogParseMessageResult(this ILogger logger, IEnumerable<MessageField> results)
        => _parseMessageResultMessage.Invoke(logger, results, null);

    internal static void LogBuildingMapping(this ILogger logger, Transformation transformation)
        => _buildingMappingMessage.Invoke(logger, transformation.TransformationType, transformation.SubType, null);

    internal static void LogMappingBuilt(this ILogger logger, Mapping mapping)
        => _mappingBuiltMessage.Invoke(logger, mapping, null);

    internal static void LogNoFieldToRemove(this ILogger logger, int key)
        => _noFieldToRemoveMessage.Invoke(logger, key, null);

    internal static void LogMappingRemoved(this ILogger logger, Mapping mapping)
        => _mappingRemovedMessage.Invoke(logger, mapping, null);

    internal static void LogMessageFieldExported(this ILogger logger, MessageField messageField, IEnumerable<byte> wireTypeValue)
        => _messageFieldExported.Invoke(logger, messageField, wireTypeValue, null);

    internal static void LogInvalidMessageField(this ILogger logger, MessageField messageField)
        => _invalidMessageField.Invoke(logger, messageField, null);


    internal static void LogMappingRemovalCompleted(this ILogger logger, int key, Mapping? removedMapping)
    {
        if (removedMapping is null)
            logger.LogNoFieldToRemove(key);
        else
            logger.LogMappingRemoved(removedMapping);
    }

    internal static void LogFieldMappingProcessed(this ILogger logger, MessageField field)
        => _fieldMappingProcessedMessage.Invoke(logger, field, null);
}
