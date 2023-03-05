using Microsoft.Extensions.Logging;

namespace PDM;

internal class DefaultLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // Since is the default logger, you get it all
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

