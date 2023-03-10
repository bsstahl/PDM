using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PDM.Entities;
using PDM.Interfaces;

namespace PDM.Core.Test.Extensions;

internal static class ServiceProviderExtensions
{
    internal static ProtobufMapper GetMapper(this IServiceProvider services)
        => services.GetMapper(null);

    internal static ProtobufMapper GetMapper(this IServiceProvider services, IEnumerable<Transformation>? transformations)
    {
        var logger = services.GetRequiredService<ILogger<ProtobufMapper>>();
        var parser = services.GetRequiredService<IWireFormatParser>();
        return services.GetMapper(logger, parser, transformations);
    }

    internal static ProtobufMapper GetMapper(this IServiceProvider _, ILogger<ProtobufMapper> logger, IWireFormatParser parser, IEnumerable<Transformation>? transformations)
        => new(logger, parser, transformations);
}
