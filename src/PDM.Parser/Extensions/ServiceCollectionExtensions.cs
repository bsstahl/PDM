using Microsoft.Extensions.DependencyInjection;
using PDM.Interfaces;

namespace PDM.Parser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseDefaultParser(this IServiceCollection services)
    {
        return services
            .AddTransient<IWireFormatParser, DefaultParser>();
    }
}
