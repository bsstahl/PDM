using Microsoft.Extensions.DependencyInjection;
using PDM.Interfaces;

namespace PDM.Serializer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseDefaultSerializer(this IServiceCollection services)
    {
        return services
            .AddTransient<IProtobufWireFormatSerializer, DefaultSerializer>();
    }
}
