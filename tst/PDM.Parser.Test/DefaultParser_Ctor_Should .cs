using PDM.Parser.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Serilog;
using System.Diagnostics;

namespace PDM.Parser.Test;

[ExcludeFromCodeCoverage]
public class DefaultParser_Ctor_Should
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultParser_Ctor_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .UseDefaultParser()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task UseDefaultLoggerIfNoLoggerIsSupplied()
    {
        _ = Trace.Listeners.Add(new SerilogTraceListener(Log.Logger));
        var sourceMessage = Array.Empty<byte>();

        var target = new DefaultParser(null!);
        var actual = await target.ParseAsync(sourceMessage);

        Assert.NotNull(actual);
    }

}