using Discord.Interactions;

namespace Tangelo.Bot.Commands.Ping;

public class PingCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Ping")]
    public async Task PingAsync()
    {
        await RespondAsync($":ping_pong:  It took {Context.Client.Latency}ms to respond.", ephemeral: true);
    }
}
