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

    public async Task<IEnumerable<SourceMessageField>> ParseAsync(byte[] message) 
        => await ParseAsync(message, string.Empty);

    private async Task<IEnumerable<SourceMessageField>> ParseAsync(byte[] message, string fieldPrefix)
    {
        _logger.LogMethodEntry(nameof(DefaultParser), nameof(ParseAsync));
        _logger.LogLargeData("Message (byte[])", Convert.ToHexString(message));

        var result = new List<SourceMessageField>();
        bool isValid = true;

        var i = 0;
        while (i < message.Length)
        {
            Tag? tag;

            var vint = Varint.Parse(message[i..]);
            tag = vint.WireLength == 0 
                ? null 
                : Tag.Parse(vint);

            if (tag is null)
            {
                // End processing of this message due to a bad tag
                // this usually occurs when we are attempting to parse
                // a LEN value as an Embedded Message when it doesn't actuall
                // represent an embedded message, but is instead a
                // string, byte[] or repeated field
                result.Clear();
                isValid = false;
                i = message.Length;
                _logger.LogUnableToParseField(i);
            }
            else
            {
                // TODO: Handle multiple instances of the same fieldNumber
                // as described here: https://protobuf.dev/programming-guides/encoding/#last-one-wins

                i += vint.WireLength;

                _logger.LogParsingField(tag);

                var fieldsToAdd = new List<SourceMessageField>();
                switch (tag.WireType)
                {
                    case Enums.WireType.VarInt:
                        var (varintField, wireLength) = message[i..].ParseVarint(tag);
                        if (wireLength > 0)
                        {
                            i += wireLength;
                            fieldsToAdd.Add(varintField);
                        }
                        break;
                    case Enums.WireType.I64:
                        if (i + 8 > message.Length)
                            i = message.Length;
                        else
                        {
                            var i64Payload = message[i..(i + 8)];
                            i += 8;
                            fieldsToAdd.Add(new SourceMessageField(tag.FieldNumber.GetFullyQualifiedKey(fieldPrefix), tag.WireType, i64Payload));
                        }
                        break;
                    case Enums.WireType.Len:
                        if (i + 2 <= message.Length)
                        {
                            var lenVarint = Varint.Parse(message[i..]);
                            if (lenVarint.Value <= int.MaxValue)
                            {
                                var len = Convert.ToInt32(lenVarint.Value);
                                i += lenVarint.WireLength;
                                if (i + len > message.Length)
                                    i = message.Length;
                                else
                                {
                                    var lenPayload = message[i..(i + len)];
                                    i += len;
                                    var fieldNumber = tag.FieldNumber.GetFullyQualifiedKey(fieldPrefix);
                                    fieldsToAdd.Add(new SourceMessageField(fieldNumber, tag.WireType, lenPayload));

                                    var childFields = await ParseAsync(lenPayload);
                                    foreach (var childField in childFields)
                                    {
                                        var fullKey = childField.Key.GetFullyQualifiedKey(fieldNumber);
                                        var fullyQualifiedChildField = new SourceMessageField(fullKey, childField.WireType, childField.Value);
                                        fieldsToAdd.Add(fullyQualifiedChildField);
                                    }
                                }
                            }
                        }
                        break;
                    case Enums.WireType.SGroup:
                    case Enums.WireType.EGroup:
                        break;
                    case Enums.WireType.I32:
                        if (i + 4 > message.Length)
                            i = message.Length;
                        else
                        {
                            var i32Payload = message[i..(i + 4)];
                            i += 4;
                            fieldsToAdd.Add(new SourceMessageField(tag.FieldNumber.GetFullyQualifiedKey(fieldPrefix), tag.WireType, i32Payload));
                        }
                        break;
                    default: // Invalid field -- end parsing
                        i = message.Length;
                        break;
                }

                foreach (var fieldToAdd in fieldsToAdd)
                {
                    result.Add(fieldToAdd);
                    _logger.LogParseFieldResult(fieldToAdd.Key, fieldToAdd.WireType, fieldToAdd.Value ?? "null");
                }
            }
        }

        if (isValid)
            _logger.LogParseMessageResult(result);
        else
            _logger.LogUnableToParseMessage(message);

        _logger.LogMethodExit(nameof(DefaultParser), nameof(ParseAsync));

        return result.AsEnumerable();
    }

}
