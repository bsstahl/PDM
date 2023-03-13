using Microsoft.Extensions.Logging;

namespace PDM;

internal sealed class DefaultLogger : ILogger
{
    public DefaultLogger() 
        => System.Diagnostics.Trace.WriteLine("Default logger configured");

    // Since this object is internal only, as long
    // as we never use scopes inside of PDM, we won't need this.
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull 
        => null;

    // Since this is the default logger, you get it all
    // Nobody should be using this thing anyway
    public bool IsEnabled(LogLevel logLevel)
        => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var formattedMessage = formatter.Invoke(state, exception);
        var logMessage = $"{logLevel}: {formattedMessage}";
        System.Diagnostics.Trace.WriteLine(logMessage);
    }
}

