using Microsoft.Extensions.DependencyInjection;
using PDM.Builders;
using PDM.TestUtils.ProtoBuf;
using PDM.TestUtils.Extensions;
using PDM.Parser.Extensions;
using PDM.Serializer.Extensions;
using Serilog;
using Xunit.Abstractions;

namespace PDM.DefaultProviders.Test;

[ExcludeFromCodeCoverage]
[Collection("MapperTests")]
public class ProtobufMapper_MapAsync_ShouldInsertAStaticValue
{
    const string _longText = "A very small stage in a vast cosmic arena hundreds of thousands stirred by starlight bits of moving fluff the sky calls to us preserve and cherish that pale blue dot? Sea of Tranquility network of wormholes as a patch of light citizens of distant epochs with pretty stories for which there's little good evidence inconspicuous motes of rock and gas. Made in the interiors of collapsing stars vanquish the impossible a mote of dust suspended in a sunbeam vastness is bearable only through love from which we spring not a sunrise but a galaxyrise and billions upon billions upon billions upon billions upon billions upon billions upon billions.";
    const string _longBytes = "41207665727920736D616C6C20737461676520696E2061207661737420636F736D6963206172656E612068756E6472656473206F662074686F7573616E6473207374697272656420627920737461726C696768742062697473206F66206D6F76696E6720666C7566662074686520736B792063616C6C7320746F20757320707265736572766520616E64206368657269736820746861742070616C6520626C756520646F743F20536561206F66205472616E7175696C697479206E6574776F726B206F6620776F726D686F6C65732061732061207061746368206F66206C6967687420636974697A656E73206F662064697374616E742065706F6368732077697468207072657474792073746F7269657320666F722077686963682074686572652773206C6974746C6520676F6F642065766964656E636520696E636F6E73706963756F7573206D6F746573206F6620726F636B20616E64206761732E204D61646520696E2074686520696E746572696F7273206F6620636F6C6C617073696E672073746172732076616E71756973682074686520696D706F737369626C652061206D6F7465206F6620647573742073757370656E64656420696E20612073756E6265616D20766173746E657373206973206265617261626C65206F6E6C79207468726F756768206C6F76652066726F6D20776869636820776520737072696E67206E6F7420612073756E726973652062757420612067616C6178797269736520616E642062696C6C696F6E732075706F6E2062696C6C696F6E732075706F6E2062696C6C696F6E732075706F6E2062696C6C696F6E732075706F6E2062696C6C696F6E732075706F6E2062696C6C696F6E732075706F6E2062696C6C696F6E732E";

    private readonly IServiceProvider _serviceProvider;

    public ProtobufMapper_MapAsync_ShouldInsertAStaticValue(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .UseDefaultParser()
            .UseDefaultSerializer()
            .AddSingleton<ProtobufMapper>()
            .BuildServiceProvider();
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(1066)]
    [InlineData(0)]
    [InlineData(-1066)]
    [InlineData(int.MinValue)]
    public async Task VarintAsInt32(int expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(1000, Enums.WireType.VarInt, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.Int32Value);
    }

    [Theory]
    [InlineData(Int64.MaxValue)]
    [InlineData(1066L)]
    [InlineData(0L)]
    [InlineData(-1066L)]
    [InlineData(Int64.MinValue)]
    public async Task VarintAsInt64(Int64 expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(1100, Enums.WireType.VarInt, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.Int64Value);
    }

    [Theory]
    [InlineData(UInt32.MaxValue)]
    [InlineData(1066U)]
    [InlineData(0)]
    public async Task VarintAsUInt32(uint expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(1200, Enums.WireType.VarInt, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.UInt32Value);
    }

    [Theory]
    [InlineData(UInt64.MaxValue)]
    [InlineData(1066UL)]
    [InlineData(0UL)]
    public async Task VarintAsUInt64(ulong expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(1300, Enums.WireType.VarInt, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.UInt64Value);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(1066)]
    [InlineData(0)]
    [InlineData(-1066)]
    [InlineData(int.MinValue)]
    public async Task VarintAsSInt32(int expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticFieldSignedVarint(1400, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.SInt32Value);
    }

    [Theory]
    [InlineData(Int64.MaxValue)]
    [InlineData(1066L)]
    [InlineData(0L)]
    [InlineData(-1066L)]
    [InlineData(Int64.MinValue)]
    public async Task VarintAsSInt64(long expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticFieldSignedVarint(1500, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.SInt64Value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task VarintAsBool(bool expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(1600, Enums.WireType.VarInt, expected)
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.BoolValue);
    }

    [Theory]
    [InlineData(SampleEnum.SampleValue0)]
    [InlineData(SampleEnum.SampleValue1)]
    [InlineData(SampleEnum.SampleValue2)]
    public async Task VarintAsEnum(SampleEnum expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(1700, Enums.WireType.VarInt, ((byte)expected).ToString())
            .Build();

        var sourceMessage = Array.Empty<byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.EnumValue);
    }

    [Theory]
    [InlineData(uint.MaxValue)]
    [InlineData(1U)]
    [InlineData(0U)]
    public async Task I32AsFixed32(uint expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(4000, Enums.WireType.I32, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.Fixed32Value);
    }

    [Theory]
    [InlineData(Int32.MaxValue)]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(Int32.MinValue)]
    public async Task I32AsSFixed32(int expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(4100, Enums.WireType.I32, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.SFixed32Value);
    }

    [Theory]
    [InlineData(float.MaxValue)]
    [InlineData(10234.3874f)]
    [InlineData(10234.0f)]
    [InlineData(0.0f)]
    [InlineData(-10234.3874f)]
    [InlineData(-10234.0f)]
    [InlineData(float.MinValue)]
    public async Task I32AsFloat(float expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(4200, Enums.WireType.I32, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.FloatValue);
    }

    [Theory]
    [InlineData(ulong.MaxValue)]
    [InlineData(1066UL)]
    [InlineData(0UL)]
    public async Task I64AsFixed64(ulong expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(2000, Enums.WireType.I64, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.Fixed64Value);
    }

    [Theory]
    [InlineData(Int64.MaxValue)]
    [InlineData(1L)]
    [InlineData(0L)]
    [InlineData(-1L)]
    [InlineData(Int64.MinValue)]
    public async Task I64AsSFixed64(long expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(2100, Enums.WireType.I64, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.SFixed64Value);
    }

    [Theory]
    [InlineData(double.MaxValue)]
    [InlineData(10234.3874d)]
    [InlineData(10234d)]
    [InlineData(0.0d)]
    [InlineData(-10234d)]
    [InlineData(-10234.3874d)]
    [InlineData(double.MinValue)]
    public async Task I64AsDouble(double expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(2200, Enums.WireType.I64, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.DoubleValue);
    }

    [Theory]
    [InlineData(_longText)]
    [InlineData("")]
    [InlineData("Short text")]
    public async Task LenAsString(string expected)
    {
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(3000, Enums.WireType.Len, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.StringValue);
    }

    [Theory]
    [InlineData(_longBytes)]
    [InlineData("00")]
    [InlineData("")]
    [InlineData("41424344")]
    public async Task LenAsBytes(string expectedHexBytes)
    {
        var expected = Convert.FromHexString(expectedHexBytes);

        var targetMapping = new TransformationBuilder()
            .InsertStaticField(3100, Enums.WireType.Len, expected)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.BytesValue);
    }

    [Theory]
    [InlineData(1066, "PDM is AWESOME!")]
    [InlineData(0, "A")]
    [InlineData(int.MaxValue, "B")]
    [InlineData(int.MinValue, "C")]
    [InlineData(100, "")]
    public async Task LenAsEmbeddedMessage(int expectedIntValue, string expectedStringValue)
    {
        var embeddedMessageValue = new SampleEmbeddedMessage()
        {
            EmbeddedInt32Value = expectedIntValue,
            EmbeddedStringValue = expectedStringValue
        };

        var embeddedBytes = embeddedMessageValue.ToByteArray(); 
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(3200, Enums.WireType.Len, embeddedBytes)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expectedIntValue, actualData.EmbeddedMessageValue.EmbeddedInt32Value);
        Assert.Equal(expectedStringValue, actualData.EmbeddedMessageValue.EmbeddedStringValue);
    }

    [Theory]
    [InlineData("00", 0)]
    [InlineData("AA08", 1066)]
    [InlineData("D6F7FFFFFFFFFFFFFF01", -1066)]
    [InlineData("007DFA01F702F403", 0, 125, 250, 375, 500)]
    [InlineData("0083FFFFFFFFFFFFFFFF0186FEFFFFFFFFFFFFFF0189FDFFFFFFFFFFFFFF018CFCFFFFFFFFFFFFFF01", 0, -125, -250, -375, -500)]
    public async Task LenAsRepeatedField(string hexString, params int[] expected)
    {
        var bytes = Convert.FromHexString(hexString);
        var targetMapping = new TransformationBuilder()
            .InsertStaticField(3300, Enums.WireType.Len, bytes)
            .Build();

        var sourceMessage = Array.Empty<Byte>();

        var target = _serviceProvider.GetMapper(targetMapping);
        var actual = await target.MapAsync(sourceMessage);

        var actualData = AllTypes.Parser.ParseFrom(actual);

        Assert.Equal(expected, actualData.RepeatedInt32Value.ToArray());
    }
}
