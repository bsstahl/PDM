using Microsoft.Extensions.DependencyInjection;
using PDM.Builders;
using Serilog;
using Xunit.Abstractions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class TransformationBuilder_AddTransformation_Should
{
    public TransformationBuilder_AddTransformation_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
                .WriteTo.Xunit(output)
                .MinimumLevel.Verbose()
                .CreateLogger();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowIfValueIsNotSupplied(string value)
    {
        var target = new TransformationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => target.AddTransformation(Enums.TransformationType.InsertField, "SubType", value));
    }
}
