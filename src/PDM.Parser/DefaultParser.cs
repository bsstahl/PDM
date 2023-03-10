using Microsoft.Extensions.Logging;
using PDM.Entities;
using PDM.Extensions;
using PDM.Interfaces;
using PDM.Parser.Extensions;

namespace PDM.Parser;

public class DefaultParser : IWireFormatParser
{
    private readonly ILogger _logger;

    public DefaultParser(ILogger<DefaultParser> logger)
    {
        _logger = logger ?? new DefaultLogger<DefaultParser>();
        if (logger is null)
            _logger.LogNoLoggerProvided();
    }

    public Task<IEnumerable<MessageField>> ParseAsync(byte[] message)
    {
        _logger.LogMethodEntry(nameof(DefaultParser), nameof(ParseAsync));

        var result = new List<MessageField>();

        var i = 0;
        while (i < message.Length)
        {
            var vint = Varint.Parse(message[i..]);
            var tag = Tag.Parse(vint);
            i += vint.WireLength;

            _logger.LogParsingField(tag);

            MessageField? currentField = null;
            switch (tag.WireType)
            {
                case Enums.WireType.VarInt:
                    var (varintField, wireLength) = message[i..].ParseVarint(tag);
                    i += wireLength;
                    currentField = varintField;
                    break;
                case Enums.WireType.I64:
                    var i64Payload = message[i..(i + 8)];
                    i += 8;
                    currentField = new MessageField(tag.FieldNumber, tag.WireType, i64Payload);
                    break;
                case Enums.WireType.Len:
                    var lenVarint = Varint.Parse(message[i..]);
                    var len = Convert.ToInt32(lenVarint.Value);
                    i += lenVarint.WireLength;
                    var lenPayload = message[i..(i + len)];
                    i += len;
                    currentField = new MessageField(tag.FieldNumber, tag.WireType, lenPayload);
                    break;
                case Enums.WireType.SGroup:
                case Enums.WireType.EGroup:
                    break;
                case Enums.WireType.I32:
                    var i32Payload = message[i..(i + 4)];
                    i += 4;
                    currentField = new MessageField(tag.FieldNumber, tag.WireType, i32Payload);
                    break;
                default:
                    throw new InvalidOperationException("Unreachable code reached");
            }

            if (currentField is not null)
            {
                result.Add(currentField);
                _logger.LogParseFieldResult(tag, currentField.Value ?? "null");
            }

        }
        _logger.LogParseMessageResult(result);

        _logger.LogMethodExit(nameof(DefaultParser), nameof(ParseAsync));
        return Task.FromResult(result.AsEnumerable());
    }


}
