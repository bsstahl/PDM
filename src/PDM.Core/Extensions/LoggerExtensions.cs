using Microsoft.Extensions.Logging;
using PDM.Constants;
using PDM.Entities;

namespace PDM.Extensions;

public static class LoggerExtensions
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

    static readonly Action<ILogger, Enums.TransformationType, string, Exception?> _buildingMappingMessage
        = LoggerMessage.Define<Enums.TransformationType, string>(LogLevel.Information,
            LogEventId.BuildingMapping,
            "Modifying mappings for {TransformationType}.{TransformationSubtype}");

    static readonly Action<ILogger, TargetMessageField, string, Exception?> _mappingBuiltMessage
        = LoggerMessage.Define<TargetMessageField, string>(LogLevel.Debug,
            LogEventId.MappingBuilt,
            "Mapping for {TargetMessageField} built during {Activity}");

    static readonly Action<ILogger, string, string, Exception?> _noFieldToRemoveMessage
        = LoggerMessage.Define<string, string>(LogLevel.Warning,
            LogEventId.NoFieldToRemove,
            "Field {FieldNumber} not present to remove during {Activity}");

    static readonly Action<ILogger, TargetMessageField, string, Exception?> _mappingRemovedMessage
        = LoggerMessage.Define<TargetMessageField, string>(LogLevel.Debug,
            LogEventId.MappingRemoved,
            "Mapping for {TargetMessageField} removed during {Activity}");

    static readonly Action<ILogger, Transformation, string, Exception?> _invalidTransformationMessage
        = LoggerMessage.Define<Transformation, string>(LogLevel.Warning,
            LogEventId.InvalidTransformation,
            "Invalid transformation {Transform} during {Activity}");

    static readonly Action<ILogger, Transformation, string, Exception?> _sourceFieldNotFoundMessage
        = LoggerMessage.Define<Transformation, string>(LogLevel.Warning,
        LogEventId.SourceFieldNotFound,
        "Source field not found in {Transform} during {Activity}");

    static readonly Action<ILogger, TargetMessageField, Exception?> _fieldMappingProcessedMessage
        = LoggerMessage.Define<TargetMessageField>(LogLevel.Information,
            LogEventId.FieldMappingProcessed,
            "Mapping of field {FieldMapping} completed");

    static readonly Action<ILogger, SourceMessageField, Exception?> _invalidSourceMessageField
        = LoggerMessage.Define<SourceMessageField>(LogLevel.Warning,
            LogEventId.InvalidMessageField,
            "Invalid source message field {Field}");

    public static void LogNoLoggerProvided(this ILogger logger)
        => _noLoggerProvidedMessage.Invoke(logger, null);

    public static void LogMethodEntry(this ILogger logger, string objectName, string methodName)
        => _methodEntryMessage.Invoke(logger, objectName, methodName, null);

    public static void LogMethodExit(this ILogger logger, string objectName, string methodName)
        => _methodExitMessage.Invoke(logger, objectName, methodName, null);

    public static void LogLargeData(this ILogger logger, string objectType, string value)
        => _largeDataMessage.Invoke(logger, objectType, value, null);

    internal static void LogRequiredFieldMissing(this ILogger logger, string fieldName)
        => _RequiredFieldMissingMessage.Invoke(logger, fieldName, null);

    internal static void LogBuildingMapping(this ILogger logger, Transformation transformation)
        => _buildingMappingMessage.Invoke(logger, transformation.TransformationType, transformation.SubType, null);

    internal static void LogMappingBuilt(this ILogger logger, TargetMessageField mapping, string activityName)
        => _mappingBuiltMessage.Invoke(logger, mapping, activityName, null);

    internal static void LogNoFieldToRemove(this ILogger logger, string key, string activity)
        => _noFieldToRemoveMessage.Invoke(logger, key, activity, null);

    internal static void LogMappingRemoved(this ILogger logger, TargetMessageField mapping, string activity)
        => _mappingRemovedMessage.Invoke(logger, mapping, activity, null);

    internal static void LogInvalidTransformation(this ILogger logger, Transformation transform, string activity)
        => _invalidTransformationMessage.Invoke(logger, transform, activity, null);

    internal static void LogSourceFieldNotFound(this ILogger logger, Transformation transform, string activity)
        => _sourceFieldNotFoundMessage.Invoke(logger, transform, activity, null);

    internal static void LogInvalidSourceField(this ILogger logger, SourceMessageField messageField)
        => _invalidSourceMessageField.Invoke(logger, messageField, null);

    internal static void LogMappingRemovalCompleted(this ILogger logger, string key, TargetMessageField? removedMapping, string activity)
    {
        if (removedMapping is null)
            logger.LogNoFieldToRemove(key, activity);
        else
            logger.LogMappingRemoved(removedMapping, activity);
    }

    internal static void LogFieldMappingProcessed(this ILogger logger, TargetMessageField field)
        => _fieldMappingProcessedMessage.Invoke(logger, field, null);
}
