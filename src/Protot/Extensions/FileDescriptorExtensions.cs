using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Protot.Extensions;

internal static class FileDescriptorExtensions
{
    internal static FileDescriptorProto? ExtractFileInfo(this string fileDescriptorFile)
    {
        using var stream = File.OpenRead(fileDescriptorFile);
        FileDescriptorSet descriptorSet = FileDescriptorSet.Parser.ParseFrom(stream);
        var byteStrings = descriptorSet.File.Select(f => f.ToByteString()).ToList();
        var descriptors = FileDescriptor.BuildFromByteStrings(byteStrings);
        var fileDescriptor = descriptors.FirstOrDefault();
        return fileDescriptor?.ToProto();
    }
}