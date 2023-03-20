using System.Diagnostics;

namespace PDM.TestUtils;

[ExcludeFromCodeCoverage]
public class SerilogTraceListener : TraceListener
{
    readonly Serilog.ILogger _logger;

    public SerilogTraceListener(Serilog.ILogger logger) 
        => _logger = logger;

    public override void Write(string? message) 
        => _logger.Information("Logged to Trace: {message}", message);

    public override void WriteLine(string? message) 
        => this.Write(message);
}

