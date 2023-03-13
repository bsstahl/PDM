using Microsoft.Extensions.DependencyInjection;
using PDM.Interfaces;

namespace PDM.Parser.Test;

[ExcludeFromCodeCoverage]
internal static class Extensions
{
    internal static IWireFormatParser GetParser(this IServiceProvider services)
        => services.GetRequiredService<IWireFormatParser>();
}
