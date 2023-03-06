using Microsoft.Extensions.Configuration;
using PDM.Constants;
using PDM.Entities;

namespace PDM.Extensions;

public static class ConfigurationExtensions
{
	public static IEnumerable<Transformation> BuildTransformations(
		this IConfiguration configuration,
		string configKey)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		if (string.IsNullOrWhiteSpace(configKey))
		{
			throw new ArgumentException($"'{nameof(configKey)}' cannot be null or whitespace.", nameof(configKey));
		}

		var configSection = configuration.GetRequiredSection(configKey);
		var transformationConfigurations = new TransformationConfigurations();

		configSection.Bind(transformationConfigurations);

		return transformationConfigurations;
	}
}
