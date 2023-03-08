using Microsoft.Extensions.Logging;

namespace PDM.Constants;

internal static class LogEventId
{
    internal static EventId NoLoggerProvided => new(0, nameof(NoLoggerProvided));

    internal static EventId MethodEntry => new(10, nameof(MethodEntry));
    internal static EventId MethodExit => new(11, nameof(MethodExit));

    internal static EventId FieldMissing => new(20, nameof(FieldMissing));

    internal static EventId ParsingField => new(30, nameof(ParsingField));
    internal static EventId ParseFieldResult => new(31, nameof(ParseFieldResult));
    internal static EventId ParseMessageResult => new(32, nameof(ParseMessageResult));

    internal static EventId BuildingMapping => new EventId(40, nameof(BuildingMapping));
    internal static EventId MappingBuilt => new EventId(41, nameof(MappingBuilt));
    internal static EventId NoFieldToRemove => new EventId(42, nameof(NoFieldToRemove));
    internal static EventId MappingRemoved => new EventId(43, nameof(MappingRemoved));

    internal static EventId FieldMappingProcessed => new EventId(50, nameof(FieldMappingProcessed));

    internal static EventId MessageFieldExported => new EventId(60, nameof(MessageFieldExported));
    internal static EventId InvalidMessageField => new EventId(61, nameof(InvalidMessageField));
}
