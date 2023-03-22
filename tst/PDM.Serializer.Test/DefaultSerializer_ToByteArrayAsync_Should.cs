using Microsoft.Extensions.DependencyInjection;
using PDM.Entities;
using PDM.Serializer.Extensions;
using PDM.TestUtils.ProtoBuf;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using TestHelperExtensions;
using Xunit.Abstractions;

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

    [Fact]
    public async void ProperlySerializeAVarintValue()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 25 };
        var wireType = Enums.WireType.VarInt;
        ulong value = 398086626; // 7-bit encoded from E2A3E9BD01

        var expectedPrefix = "C801";
        var expectedValue = "E2A3E9BD01";
        var expectedHexString = expectedPrefix + expectedValue;

        targetFields.Add(new TargetMessageField(key, wireType, value));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void ProperlySerializeAnI64ValueFromHexString()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 25 };
        var wireType = Enums.WireType.I64;
        var hexStringValue = "1F1AD6BA28915490";

        var expectedPrefix = "C901";
        var expectedHexString = expectedPrefix + hexStringValue;

        targetFields.Add(new TargetMessageField(key, wireType, hexStringValue));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void ProperlySerializeAnI64ValueFromByteArray()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 17 };
        var wireType = Enums.WireType.I64;
        var hexStringValue = "1F1AD6BA28915490";
        var value = Convert.FromHexString(hexStringValue);

        var expectedPrefix = "8901";
        var expectedHexString = expectedPrefix + hexStringValue;

        targetFields.Add(new TargetMessageField(key, wireType, value));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void ProperlySerializeALenValueAsAString()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 3000 };
        var wireType = Enums.WireType.Len;
        var expected = "The beast at Tanagra";

        targetFields.Add(new TargetMessageField(key, wireType, expected));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var actualData = AllTypes.Parser.ParseFrom(actual);
        Assert.Equal(expected, actualData.StringValue);
    }

    [Fact]
    public async void ProperlySerializeALenValueAsBytes()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 3100 };
        var wireType = Enums.WireType.Len;
        var expected = Enumerable
            .Range(1, 255)
            .Select(b => Convert.ToByte(b))
            .ToArray();

        targetFields.Add(new TargetMessageField(key, wireType, expected));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var actualData = AllTypes.Parser.ParseFrom(actual);
        Assert.Equal(expected, actualData.BytesValue);
    }

    [Fact]
    public async void ProperlySerializeALenValueAsAHexString()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 3100 };
        var wireType = Enums.WireType.Len;
        var expected = Enumerable
            .Range(1, 255)
            .Select(b => Convert.ToByte(b))
            .ToArray();

        var fieldValue = Convert.ToHexString(expected);
        targetFields.Add(new TargetMessageField(key, wireType, fieldValue));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var actualData = AllTypes.Parser.ParseFrom(actual);
        Assert.Equal(expected, actualData.BytesValue);
    }

    [Fact]
    public async void ProperlySerializeALenValueInARepeatedField()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 3300 };
        var wireType = Enums.WireType.Len;
        var expected = Enumerable
            .Range(15, 35)
            .ToArray();

        var fieldValue = expected.Select(e => (byte)e).ToArray();
        targetFields.Add(new TargetMessageField(key, wireType, fieldValue));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var actualData = AllTypes.Parser.ParseFrom(actual);
        Assert.Equal(expected, actualData.RepeatedInt32Value);
    }

    [Fact]
    public async void ProperlySerializeALenValueWithMultiByteValuesInARepeatedField()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 3300 };
        var wireType = Enums.WireType.Len;
        var expected = new int[] { 32550, 3831961 };

        var fieldValue = new byte[] { 166, 254, 1, 153, 241, 233, 1 };
        targetFields.Add(new TargetMessageField(key, wireType, fieldValue));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var actualData = AllTypes.Parser.ParseFrom(actual);
        Assert.Equal(expected, actualData.RepeatedInt32Value.ToArray());
    }

    [Fact]
    public async void ProperlySerializeAnI32ValueFromHexString()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 25 };
        var wireType = Enums.WireType.I32;
        var hexStringValue = "1F1AD6BA";

        var expectedPrefix = "CD01";
        var expectedHexString = expectedPrefix + hexStringValue;

        targetFields.Add(new TargetMessageField(key, wireType, hexStringValue));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void ProperlySerializeAnI32ValueFromByteArray()
    {
        var targetFields = new List<Entities.TargetMessageField>();

        var key = new[] { 17 };
        var wireType = Enums.WireType.I32;
        var hexStringValue = "1F1AD6BA28915490";
        var value = Convert.FromHexString(hexStringValue);

        var expectedPrefix = "8D01";
        var expectedHexString = expectedPrefix + hexStringValue;

        targetFields.Add(new TargetMessageField(key, wireType, value));

        var target = _serviceProvider.GetSerializer();
        var actual = await target.ToByteArrayAsync(targetFields);

        var expected = Convert.FromHexString(expectedHexString);
        Assert.Equal(expected, actual);
    }
}