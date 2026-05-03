using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FinBookeAPI.AppConfig.Documentation;

// TODO: Refactor to delete these functions

/// <summary>
/// This class provides function to log and throw exceptions
/// </summary>
public static class LoggingDeprecated
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowAndLogInformation(ILogger logger, int logEvent, Exception exception)
    {
        logger.LogInformation(
            new EventId(logEvent),
            "{Exception}: {Message}",
            exception.GetType(),
            exception.Message
        );

        throw exception;
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowAndLogWarning(ILogger logger, int logEvent, Exception exception)
    {
        logger.LogWarning(
            new EventId(logEvent),
            "{Exception}: {Message}",
            exception.GetType(),
            exception.Message
        );

        throw exception;
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowAndLogError(ILogger logger, int logEvent, Exception exception)
    {
        logger.LogError(
            new EventId(logEvent),
            "{Exception}: {Message}",
            exception.GetType(),
            exception.Message
        );

        throw exception;
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowAndLogCritical(ILogger logger, int logEvent, Exception exception)
    {
        logger.LogError(
            new EventId(logEvent),
            "{Exception}: {Message}",
            exception.GetType(),
            exception.Message
        );

        throw exception;
    }
}
