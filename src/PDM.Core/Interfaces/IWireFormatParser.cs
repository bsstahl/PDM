using PDM.Entities;

namespace PDM.Interfaces;

public interface IWireFormatParser
{
    Task<IEnumerable<SourceMessageField>> ParseAsync(byte[] message);
}
