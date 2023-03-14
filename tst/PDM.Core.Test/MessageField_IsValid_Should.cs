using PDM.Entities;
using Serilog;
using Xunit.Abstractions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class MessageField_IsValid_Should
{
    public MessageField_IsValid_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }

    [Fact]
    public void ReturnFalseIfKeyIsZero()
    {
        var key = new[] { 0 };
        var wireType = Enums.WireType.Len;
        var value = Int32.MaxValue.GetRandom();

        var actual = new TargetMessageField(key, wireType, value);
        Assert.False(actual.IsValid);
    }

    [Fact]
    public void ReturnFalseIfValueIsNull()
    {
        var key = new[] { 0 };
        var wireType = Enums.WireType.Len;
        object? value = null;

        var actual = new TargetMessageField(key, wireType, value);
        Assert.False(actual.IsValid);
    }
}

