using PDM.Extensions;
using Serilog;
using Xunit.Abstractions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class ObjectExtensions_IsByteArray_Should
{
    public ObjectExtensions_IsByteArray_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }

    [Fact]
    public void ReturnTrueIfValueIsNull()
    {
        object? value = null;
        Assert.True(value!.IsByteArray());
    }

    [Fact]
    public void ReturnTrueIfValueIsAnEmptyByteArray()
    {
        object? value = Array.Empty<byte>();
        Assert.True(value.IsByteArray());
    }

    [Fact]
    public void ReturnTrueIfValueIsAPopulatedByteArray()
    {
        object? value = new byte[] { 1, 2, 3 };
        Assert.True(value.IsByteArray());
    }

    [Fact]
    public void ReturnFalseIfValueIsScalar()
    {
        object? value = Int32.MaxValue.GetRandom();
        Assert.False(value.IsByteArray());
    }
}

