using Microsoft.Extensions.DependencyInjection;
using PDM.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace PDM.Serializer.Test;

[ExcludeFromCodeCoverage]
internal static class Extensions
{
    internal static IProtobufWireFormatSerializer GetSerializer(this IServiceProvider services)
        => services.GetRequiredService<IProtobufWireFormatSerializer>();
}
