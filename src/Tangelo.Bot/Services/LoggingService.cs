using Discord;
using Discord.WebSocket;
using Discord.Interactions;

namespace Tangelo.Bot.Services;

public class LoggingService
{
    private readonly ILogger _logger;

    public LoggingService(ILogger<LoggingService> logger, DiscordSocketClient client, InteractionService interactions)
    {
        _logger = logger;

        client.Log += LogAsync;
        interactions.Log += LogAsync;
    }

    public Task LogAsync(LogSeverity severity, string source, string message, Exception? exception = null)
    {
        return LogAsync(new LogMessage(severity, source, message, exception));
    }

    public Task LogAsync(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical("{Source} {Message} {Error}", message.Source, message.Message, message.Exception);
                break;
            case LogSeverity.Error:
                _logger.LogError("{Source} {Message} {Error}", message.Source, message.Message, message.Exception);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("{Source} {Message} {Error}", message.Source, message.Message, message.Exception);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{Source} {Message}", message.Source, message.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace("{Source} {Message}", message.Source, message.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug("{Source} {Message}", message.Source, message.Message);
                break;
            default:
                break;
        }

        return Task.CompletedTask;
    }
}
