using Microsoft.Extensions.DependencyInjection;
using PDM.Builders;
using PDM.Interfaces;
using PDM.TestUtils.ProtoBuf;
using PDM.TestUtils.Extensions;
using Serilog;
using System.Diagnostics;
using Xunit.Abstractions;
using Moq;
using PDM.TestUtils;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
[Collection("MapperTests")]
public class ProtobufMapper_MapAsync_Should
{
    private readonly IServiceProvider _serviceProvider;

    public ProtobufMapper_MapAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .BuildServiceProvider();
    }

    [Fact]
    public async Task NotFailIfNoLoggerIsSupplied()
    {
        _ = Trace.Listeners.Add(new SerilogTraceListener(Log.Logger));
        var sourceMessage = Convert.FromHexString("2A08666538616230326155DD3841C878A4CEBAE404");
        var parser = Mock.Of<IWireFormatParser>();
        var target = _serviceProvider.GetMapper(null!, parser, null);
        _ = await target.MapAsync(sourceMessage);
    }

    [Fact]
    public async Task ThrowIfNoSourceMessageSupplied()
    {
        var sourceData = new TwoFields()
        {
            IntegerValue = Int32.MaxValue.GetRandom(),
            StringValue = String.Empty.GetRandom()
        };

        byte[]? sourceMessage = null;
        var targetMapping = new TransformationBuilder()
            .Build();

        var parser = Mock.Of<IWireFormatParser>();
        var logger = _serviceProvider.GetMapperLogger();
        var target = _serviceProvider.GetMapper(logger, parser, targetMapping);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => target.MapAsync(sourceMessage!));
    }

}
