using Microsoft.Extensions.Logging;
using PDM.Entities;
using PDM.Extensions;
using PDM.Interfaces;
using PDM.Parser.Extensions;
using PDM.WireTypes;

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
        => await ParseAsync(message ?? Array.Empty<byte>(), string.Empty).ConfigureAwait(false);

    private async Task<IEnumerable<SourceMessageField>> ParseAsync(byte[] message, string fieldPrefix)
    {
        _logger.LogMethodEntry(nameof(DefaultParser), nameof(ParseAsync));
        _logger.LogLargeData("Message (byte[])", Convert.ToHexString(message));

        var result = new List<SourceMessageField>();
        bool isValid = true;

        var i = 0;
        while (i < message.Length)
        {
            var vint = Varint.Parse(message[i..]);
            var tag = vint.WireLength == 0
                ? Tag.Empty
                : Tag.Parse(vint);

            if (tag.FieldNumber == Tag.Empty.FieldNumber)
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
                        var i64Value = I64.Parse(message[i..]);
                        if (i64Value.WireLength == 0)
                            i = message.Length;
                        else
                        {
                            i += i64Value.WireLength;
                            fieldsToAdd.Add(new SourceMessageField(tag.FieldNumber.GetFullyQualifiedKey(fieldPrefix), tag.WireType, i64Value.GetRawData()));
                        }
                        break;
                    case Enums.WireType.Len:
                        var lenValue = Len.Parse(message[i..]);
                        if (lenValue.WireLength == 0)
                            i = message.Length;
                        else
                        {
                            i += lenValue.WireLength;
                            var fieldNumber = tag.FieldNumber.GetFullyQualifiedKey(fieldPrefix);
                            fieldsToAdd.Add(new SourceMessageField(fieldNumber, tag.WireType, lenValue.Value));

                            var childFields = await this
                                .ParseAsync(lenValue.GetRawData())
                                .ConfigureAwait(false);

                            foreach (var childField in childFields)
                            {
                                var fullKey = childField.Key.GetFullyQualifiedKey(fieldNumber);
                                var fullyQualifiedChildField = new SourceMessageField(fullKey, childField.WireType, childField.Value);
                                fieldsToAdd.Add(fullyQualifiedChildField);
                            }
                        }
                        break;
                    case Enums.WireType.SGroup:
                    case Enums.WireType.EGroup:
                        break;
                    case Enums.WireType.I32:
                        var i32Value = I32.Parse(message[i..]);
                        if (i32Value.WireLength == 0)
                            i = message.Length;
                        else
                        {
                            i += i32Value.WireLength;
                            fieldsToAdd.Add(new SourceMessageField(tag.FieldNumber.GetFullyQualifiedKey(fieldPrefix), tag.WireType, i32Value.GetRawData()));
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
