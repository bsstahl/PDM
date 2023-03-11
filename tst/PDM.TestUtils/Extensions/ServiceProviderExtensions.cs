using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PDM.Entities;
using PDM.Interfaces;

namespace PDM.TestUtils.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceProviderExtensions
{
    public static ProtobufMapper GetMapper(this IServiceProvider services)
        => services.GetMapper(null);

    public static ProtobufMapper GetMapper(this IServiceProvider services, IEnumerable<Transformation>? transformations)
    {
        var logger = services.GetRequiredService<ILogger<ProtobufMapper>>();
        var parser = services.GetRequiredService<IWireFormatParser>();
        return services.GetMapper(logger, parser, transformations);
    }

    public static ProtobufMapper GetMapper(this IServiceProvider _, ILogger<ProtobufMapper> logger, IWireFormatParser parser, IEnumerable<Transformation>? transformations)
        => new(logger, parser, transformations);

    public static ILogger<ProtobufMapper> GetMapperLogger(this IServiceProvider services)
        => services.GetLogger<ProtobufMapper>();

    public static ILogger<T> GetLogger<T>(this IServiceProvider services)
        => services.GetRequiredService<ILogger<T>>();
}
