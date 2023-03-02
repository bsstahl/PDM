using PDM.Extensions;
using Serilog;
using Xunit.Abstractions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class ObjectExtensions_GetByteArrayLength_Should
{
    public ObjectExtensions_GetByteArrayLength_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }

    [Fact]
    public void ReturnZeroIfValueIsNull()
    {
        object? value = null;
        Assert.Equal(0, value!.GetByteArrayLength());
    }

    [Fact]
    public void ReturnZeroIfValueIsScalar()
    {
        object? value = Int32.MaxValue.GetRandom();
        Assert.Equal(0, value.GetByteArrayLength());
    }

    [Fact]
    public void ReturnZeroIfValueIsAnEmptyByteArray()
    {
        object? value = Array.Empty<byte>();
        Assert.Equal(0, value.GetByteArrayLength());
    }

    [Fact]
    public void ReturnTheCorrectLengthIfValueIsAPopulatedByteArray()
    {
        object? value = new byte[] { 1, 2, 3 };
        Assert.Equal(3, value.GetByteArrayLength());
    }

}

