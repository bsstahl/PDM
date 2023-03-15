using Microsoft.Extensions.Configuration;
using PDM.Enums;
using Protot.Core.Enums;
using Protot.Core.Extensions;
using Protot.Core.Test.Builders;

namespace Protot.Core.Test;

[ExcludeFromCodeCoverage]
public class ConfigurationExtensions_BuildTransformations_Should
{
    [Fact]
    public void ThrowArgumentNullExceptionIfConfigurationIsNull()
    {
        var configuration = (IConfiguration)null!;
        var configKey = JsonConfigurationBuilder.ConfigKey;

        Assert.Throws<ArgumentNullException>(() => configuration.BuildTransformations(configKey));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ThrowArgumentExceptionIfConfigKeyIsNullOrWhiteSpace(string configKey)
    {
        var configuration = new JsonConfigurationBuilder().BuildAllTypes();

        Assert.Throws<ArgumentException>(() => configuration.BuildTransformations(configKey));
    }

    [Fact]
    public void ProperlyBuildsTransforms()
    {
        var configuration = new JsonConfigurationBuilder().BuildAllTypes();
        var configKey = JsonConfigurationBuilder.ConfigKey;

        var transformations = configuration.BuildTransformations(configKey);

        Assert.NotNull(transformations);
        Assert.NotEmpty(transformations);

        var first = transformations.ElementAt(0);
        Assert.Equal(TransformationType.InsertField, first.TransformationType);
        Assert.Equal(TransformationSubtype.Include, first.SubType);
        Assert.Equal("0", first.Value);

        var second = transformations.ElementAt(1);
        Assert.Equal(TransformationType.ReplaceField, second.TransformationType);
        Assert.Equal(TransformationSubtype.Renames, second.SubType);
        Assert.Equal("StringValue:StringValue", second.Value);

        var third = transformations.ElementAt(2);
        Assert.Equal(TransformationType.ReplaceField, third.TransformationType);
        Assert.Equal(TransformationSubtype.Renames, third.SubType);
        Assert.Equal("FloatValue:FloatValue", third.Value);

        var fourth = transformations.ElementAt(3);
        Assert.Equal(TransformationType.InsertField, fourth.TransformationType);
        Assert.Equal(TransformationSubtype.Static, fourth.SubType);
        Assert.Equal("IntegerValue:Int32:173559425", fourth.Value);
    }
}