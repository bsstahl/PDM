using Google.Protobuf;

namespace PDM.Core.Test;

public static class MessageExtensions
{
    public static byte[] ToByteArray(this IMessage message)
    {
        // TODO: Use expandable Stream rather than fixed length output
        var flatArray = new byte[64];
        CodedOutputStream stream = new CodedOutputStream(flatArray);
        message.WriteTo(stream);
        return flatArray;
    }

    public static T ToMessage<T>(this byte[] data)
        where T : IMessage<T>
    {
        throw new NotImplementedException();
    }
}
