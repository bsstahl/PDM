using System.Diagnostics;

namespace PDM.Core.Test;

internal class SerilogTraceListener : TraceListener
{
    readonly Serilog.ILogger _logger;

    public SerilogTraceListener(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public override void Write(string? message)
    {
        _logger.Information("Logged to Trace: '{message}'", message);
    }

    public override void WriteLine(string? message)
    {
        this.Write(message);
    }
}

