using System.Reflection;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tangelo.Bot.Data.Models;

namespace Tangelo.Bot.Services;

public class BotService(
    IConfiguration config,
    IServiceProvider services,
    DiscordSocketClient client,
    InteractionService interactions,
    LoggingService logger,
    DataService data
) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await logger.LogAsync(new LogMessage(LogSeverity.Info, "StartAsync", "Tangelo bot starting..."));

        var token = config["DISCORD_TOKEN"];
        if (token == null)
        {
            await logger.LogAsync(new LogMessage(LogSeverity.Critical, "StartAsync", "Discord token missing. Exiting."));
            throw new Exception("Configuration value 'DISCORD_TOKEN' not found");
        }

        await RegisterEventHandlersAsync();
        await InstallModulesAsync();

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await logger.LogAsync(LogSeverity.Info, "StopAsync", "Disconnecting Tangelo Bot.");

        client.InteractionCreated -= InteractionCreatedAsync;
        await client.StopAsync();
        await client.DisposeAsync();

        await base.StopAsync(cancellationToken);

        await logger.LogAsync(new LogMessage(LogSeverity.Info, "StopAsync", "Disconnected."));
    }

    private async Task RegisterEventHandlersAsync()
    {
        await logger.LogAsync(new LogMessage(LogSeverity.Verbose, "SetupEventsAsync", "Registering event handlers."));

        interactions.SlashCommandExecuted += SlashCommandExecutedAsync;

        client.Ready += ClientReadyAsync;
        client.InteractionCreated += InteractionCreatedAsync;
    }

    private async Task InstallModulesAsync()
    {
        await logger.LogAsync(new LogMessage(LogSeverity.Verbose, nameof(InstallModulesAsync), "Installing command modules."));

        await interactions.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
    }

    private async Task ClientReadyAsync()
    {
        await logger.LogAsync(new LogMessage(LogSeverity.Verbose, "ClientReadyAsync", "Registering slash commands."));

        await interactions.RegisterCommandsGloballyAsync();
    }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        await logger.LogAsync(new LogMessage(LogSeverity.Verbose, "InteractionCreatedAsync", $"Interaction received: {interaction.Type}"));

        try
        {
            var context = new SocketInteractionContext(client, interaction);
            await interactions.ExecuteCommandAsync(context, services);
        }
        catch (Exception ex)
        {
            await logger.LogAsync(new LogMessage(LogSeverity.Error, "InteractionCreatedAsync", "An exception occurred during slash command execution", ex));

            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());

            // TODO: Handle other exceptions?
        }
    }

    private async Task SlashCommandExecutedAsync(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            await logger.LogAsync(new LogMessage(LogSeverity.Warning, "HandleSlashCommandFailureAsync", $"Command \"/{commandInfo.Name}\" failed execution: {result.ErrorReason}"));
            await HandleSlashCommandFailureAsync(commandInfo, context, result);
        }
        else
        {
            await logger.LogAsync(new LogMessage(LogSeverity.Verbose, "SlashCommandExecutedAsync", $"Command \"/{commandInfo.Name}\" executed successfully"));
            await HandleSlashCommandSuccessAsync(commandInfo, context);
        }
    }

    private async Task HandleSlashCommandFailureAsync(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        await logger.LogAsync(new LogMessage(LogSeverity.Debug, "HandleSlashCommandFailureAsync", $"Command: {commandInfo.Name}, Guild Id: {context.Guild.Id}, User Id: {context.User.Id}, Error: {result.Error}"));

        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                await context.Interaction.RespondAsync($"Unmet precondition: {result.ErrorReason}.", ephemeral: true);
                break;
            case InteractionCommandError.UnknownCommand:
                await context.Interaction.RespondAsync("Unknown command.", ephemeral: true);
                break;
            case InteractionCommandError.Unsuccessful:
                await context.Interaction.RespondAsync("Command could not be executed.", ephemeral: true);
                break;
            default:
                await context.Interaction.RespondAsync("Sorry! Something went wrong.", ephemeral: true);
                break;
        }
    }

    private async Task HandleSlashCommandSuccessAsync(ICommandInfo commandInfo, IInteractionContext context)
    {
        await logger.LogAsync(new LogMessage(LogSeverity.Verbose, "TrackUsageAsync", $"Command Name: {commandInfo.Name}, Guild Id: {context.Guild.Id}, Guild Name: {context.Guild.Name}, User Id: {context.User.Id}, User Name: {context.User.Username}"));

        Command? command = await data.FindCommandAsync(commandInfo.Name);
        Guild guild = await data.FindGuildAsync(context.Guild.Id) ?? await data.CreateGuildAsync(context.Guild.Id, context.Guild.Name);
        User user = await data.FindUserAsync(context.User.Id) ?? await data.CreateUserAsync(context.User.Id, context.User.Username);

        await data.CreateUsageAsync(command!, guild, user);
    }
}
