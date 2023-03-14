using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;
using PDM.Serializer.Extensions;
using PDM.TestUtils;

namespace PDM.Serializer.Test;

[ExcludeFromCodeCoverage]
public class DefaultSerializer_Ctor_Should
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultSerializer_Ctor_Should(ITestOutputHelper output)
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
    public void UseDefaultLoggerIfNoLoggerIsSupplied()
    {
        _ = Trace.Listeners.Add(new SerilogTraceListener(Log.Logger));
        var target = new DefaultSerializer(null!);
    }

}