using Microsoft.Extensions.Logging;

namespace PDM.Constants;

public static class LogEventId
{
    public static EventId NoLoggerProvided => new(0, nameof(NoLoggerProvided));

    public static EventId MethodEntry => new(10, nameof(MethodEntry));
    public static EventId MethodExit => new(11, nameof(MethodExit));

    public static EventId FieldMissing => new(20, nameof(FieldMissing));

    public static EventId ParsingField => new(30, nameof(ParsingField));
    public static EventId ParseFieldResult => new(31, nameof(ParseFieldResult));
    public static EventId UnableToParseField => new(32, nameof(UnableToParseField));
    public static EventId ParseMessageResult => new(33, nameof(ParseMessageResult));
    public static EventId UnableToParseMessage => new(34, nameof(UnableToParseMessage));

    public static EventId BuildingMapping => new EventId(40, nameof(BuildingMapping));
    public static EventId MappingBuilt => new EventId(41, nameof(MappingBuilt));
    public static EventId NoFieldToRemove => new EventId(42, nameof(NoFieldToRemove));
    public static EventId MappingRemoved => new EventId(43, nameof(MappingRemoved));
    public static EventId InvalidTransformation => new EventId(44, nameof(InvalidTransformation));
    public static EventId SourceFieldNotFound => new EventId(45, nameof(SourceFieldNotFound));

    public static EventId FieldMappingProcessed => new EventId(50, nameof(FieldMappingProcessed));

    public static EventId MessageFieldExported => new EventId(60, nameof(MessageFieldExported));
    public static EventId InvalidMessageField => new EventId(61, nameof(InvalidMessageField));
}
