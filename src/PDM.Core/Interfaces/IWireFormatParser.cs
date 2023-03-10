using PDM.Entities;

namespace PDM.Interfaces;

public interface IWireFormatParser
{
    Task<IEnumerable<MessageField>> ParseAsync(byte[] message);
}
