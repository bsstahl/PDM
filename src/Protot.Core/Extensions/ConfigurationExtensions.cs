using Microsoft.Extensions.Configuration;
using Protot.Core.Entities;

namespace Protot.Core.Extensions;

public static class ConfigurationExtensions
{
    public static IEnumerable<ProtoTransformation> BuildTransformations(
        this IConfiguration configuration,
        string configKey)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (string.IsNullOrWhiteSpace(configKey))
        {
            throw new ArgumentException($"'{nameof(configKey)}' cannot be null or whitespace.", nameof(configKey));
        }

        var configSection = configuration.GetRequiredSection(configKey);
        var transformationConfigurations = new ProtoTransformations();

        configSection.Bind(transformationConfigurations);

        return transformationConfigurations;
    }
}
