using Microsoft.Extensions.Logging;

namespace PDM;

public sealed class DefaultLogger<T> : ILogger<T>
    where T : class
{
    public DefaultLogger()
        => System.Diagnostics.Trace.WriteLine("Default logger configured");

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => null;

    // Since this is the default logger, you get it all
    // Nobody should be using this thing anyway
    public bool IsEnabled(LogLevel logLevel)
        => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (formatter is null)
            throw new ArgumentNullException(nameof(formatter));
        var formattedMessage = formatter.Invoke(state, exception);
        var logMessage = $"{logLevel}: {formattedMessage}";
        System.Diagnostics.Trace.WriteLine(logMessage);
    }
}

