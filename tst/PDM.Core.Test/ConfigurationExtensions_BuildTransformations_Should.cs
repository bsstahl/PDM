using Microsoft.Extensions.Configuration;
using PDM.Core.Test.Builders;
using PDM.Enums;
using PDM.Extensions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class ConfigurationExtensions_BuildTransformations_Should
{
	[Fact]
	public void ThrowArgumentNullExceptionIfConfigurationIsNull()
	{
		var configuration = (IConfiguration)null!;
		var configKey = TransformationConfigurationBuilder.ConfigKey;

		Assert.Throws<ArgumentNullException>(() => configuration.BuildTransformations(configKey));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	public void ThrowArgumentExceptionIfConfigKeyIsNullOrWhiteSpace(string configKey)
	{
		var configuration = new TransformationConfigurationBuilder().BuildAllTypes();

		Assert.Throws<ArgumentException>(() => configuration.BuildTransformations(configKey));
	}

	[Fact]
	public void ProperlyBuildsTransforms()
	{
		var configuration = new TransformationConfigurationBuilder().BuildAllTypes();
		var configKey = TransformationConfigurationBuilder.ConfigKey;

		var transformations = configuration.BuildTransformations(configKey);

		Assert.NotNull(transformations);
		Assert.NotEmpty(transformations);

		var first = transformations.ElementAt(0);
		Assert.Equal(TransformationType.InsertField, first.TransformationType);
		Assert.Equal("include", first.SubType);
		Assert.Equal("0", first.Value);

		var second = transformations.ElementAt(1);
		Assert.Equal(TransformationType.ReplaceField, second.TransformationType);
		Assert.Equal("renames", second.SubType);
		Assert.Equal("3000:5", second.Value);

		var third = transformations.ElementAt(2);
		Assert.Equal(TransformationType.ReplaceField, third.TransformationType);
		Assert.Equal("renames", third.SubType);
		Assert.Equal("4200:10", third.Value);

		var fourth = transformations.ElementAt(3);
		Assert.Equal(TransformationType.InsertField, fourth.TransformationType);
		Assert.Equal("static", fourth.SubType);
		Assert.Equal("15:VarInt:173559425", fourth.Value);
	}
}
