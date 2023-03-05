using Microsoft.Extensions.Logging;

namespace PDM.Extensions;

internal static class LoggerExtensions
{
    internal static void LogMethodEntry(this ILogger logger, string objectName, string methodName)
#pragma warning disable CA1848 // TODO: Use the LoggerMessage delegates
        => logger.LogDebug("Entered {ObjectName}.{MethodName}", objectName, methodName);
#pragma warning restore CA1848 // Use the LoggerMessage delegates

    internal static void LogMethodExit(this ILogger logger, string objectName, string methodName)
#pragma warning disable CA1848 // TODO: Use the LoggerMessage delegates
        => logger.LogDebug("Exiting {ObjectName}.{MethodName}", objectName, methodName);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
}
