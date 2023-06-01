using Microsoft.Extensions.Configuration;
using Protot.Core.Entities;

namespace Protot.Core.Extensions;

public static class ConfigurationExtensions
{
    public static ProtoTransformationConfig BuildTransformations(
        this IConfiguration configuration,
        string configKey)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (string.IsNullOrWhiteSpace(configKey))
        {
            throw new ArgumentException($"'{nameof(configKey)}' cannot be null or whitespace.", nameof(configKey));
        }

        var configSection = configuration.GetRequiredSection(configKey);
        var transformationConfigurations = new ProtoTransformationConfig();

        configSection.Bind(transformationConfigurations);

        return transformationConfigurations;
    }
}
