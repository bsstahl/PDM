using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;
using PDM.Serializer.Extensions;
using PDM.TestUtils;
using TestHelperExtensions;
using PDM.Entities;

namespace PDM.Serializer.Test;

[ExcludeFromCodeCoverage]
public class DefaultSerializer_ToByteArrayAsync_Should
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultSerializer_ToByteArrayAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .UseDefaultSerializer()
            .BuildServiceProvider();
    }

    [Fact]
    public async void ReturnAnEmptyArrayIfANullTargetFieldCollectionIsSupplied()
    {
        Entities.TargetMessageField[]? targetFields = null;

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields!);

        Assert.Empty(actual);
    }

    [Fact]
    public async void ReturnAnEmptyArrayIfNoTargetFieldsAreSupplied()
    {
        var targetFields = Array.Empty<Entities.TargetMessageField>();

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        Assert.Empty(actual);
    }

    [Fact]
    public async void NotFailIfAnEmptyKeyArrayIsFoundInAMessageField()
    {
        var key = Array.Empty<int>();
        var wireType = Enums.WireType.VarInt;
        var value = Convert.ToUInt64(Int32.MaxValue.GetRandom(0));

        var targetFields = new List<TargetMessageField>()
            { new TargetMessageField(key, wireType, value) };

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        Assert.Empty(actual);
    }

    [Theory]
    [InlineData(25, 398086626, "C801E2A3E9BD01")]
    [InlineData(31, 398004704, "F801E0A3E4BD01")]
    public async void ProperlySerializeAVarintValue(int simpleKey, ulong value, string expectedHexString)
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { simpleKey };
        var wireType = Enums.WireType.VarInt;

        targetFields.Add(new TargetMessageField(key, wireType, value));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(25, "1F1AD6BA28915490", "C9011F1AD6BA28915490")]
    public async void ProperlySerializeAnI64ValueFromHexString(int simpleKey, string hexStringValue, string expectedHexString)
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { simpleKey };
        var wireType = Enums.WireType.I64;

        targetFields.Add(new TargetMessageField(key, wireType, hexStringValue));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(25, "1F1AD6BA28915490", "C9011F1AD6BA28915490")]
    public async void ProperlySerializeAnI64ValueFromByteArray(int simpleKey, string hexStringValue, string expectedHexString)
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { simpleKey };
        var wireType = Enums.WireType.I64;
        var value = Convert.FromHexString(hexStringValue);

        targetFields.Add(new TargetMessageField(key, wireType, value));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }
}