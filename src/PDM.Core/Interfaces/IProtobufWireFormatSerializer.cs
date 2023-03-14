using PDM.Entities;

namespace PDM.Interfaces;

public interface IProtobufWireFormatSerializer
{
    Task<byte[]> ToByteArrayAsync(IEnumerable<TargetMessageField> messageFields);
}
