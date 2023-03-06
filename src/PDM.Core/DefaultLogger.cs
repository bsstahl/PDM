using Microsoft.Extensions.Logging;

namespace PDM;

internal class DefaultLogger : ILogger
{
    public DefaultLogger()
    {
        System.Diagnostics.Trace.WriteLine("Default logger configured");
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        // Since this object is scoped internally, as long
        // as we never use scopes inside of PDM, we won't need this.
        // If we ever configure scopes, we may need to implement this method
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // Since this is the default logger, you get it all
        // Nobody should be using this thing anyway
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var formattedMessage = formatter.Invoke(state, exception);
        var logMessage = $"{logLevel}: {formattedMessage}";
        System.Diagnostics.Trace.WriteLine(logMessage);
    }
}

