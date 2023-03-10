using Microsoft.Extensions.Logging;
using PDM.Constants;
using PDM.Entities;

namespace PDM.Parser.Extensions;

internal static class LoggerExtensions
{
    static readonly Action<ILogger, int, Enums.WireType, Exception?> _parsingFieldMessage
    = LoggerMessage.Define<int, Enums.WireType>(LogLevel.Debug,
        LogEventId.ParsingField,
        "Parsing field {FieldNumber} with wiretype {WireType}");

    static readonly Action<ILogger, int, Enums.WireType, object, Exception?> _parseFieldResultMessage
        = LoggerMessage.Define<int, Enums.WireType, object>(LogLevel.Information,
            LogEventId.ParseFieldResult,
            "Field {FieldNumber} with wiretype {WireType} parsed as {Value}");

    static readonly Action<ILogger, IEnumerable<MessageField>, Exception?> _parseMessageResultMessage
        = LoggerMessage.Define<IEnumerable<MessageField>>(LogLevel.Debug,
            LogEventId.ParseMessageResult,
            "Parse message result: {Result}");



    internal static void LogParsingField(this ILogger logger, Entities.Tag tag)
    => _parsingFieldMessage.Invoke(logger, tag.FieldNumber, tag.WireType, null);

    internal static void LogParseFieldResult(this ILogger logger, Entities.Tag tag, object value)
        => _parseFieldResultMessage.Invoke(logger, tag.FieldNumber, tag.WireType, value, null);

    internal static void LogParseMessageResult(this ILogger logger, IEnumerable<MessageField> results)
        => _parseMessageResultMessage.Invoke(logger, results, null);
}
