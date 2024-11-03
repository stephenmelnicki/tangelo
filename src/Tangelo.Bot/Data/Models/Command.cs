namespace Tangelo.Bot.Data.Models;

public enum CommandName
{
    Ping = 1,
}

// TODO: See if there's a better way to define the commands.
// we're trying to track the usage of the various commands.
public record Command
{
    public int Id { get; init; }
    public CommandName Name { get; init; }

    public List<Usage> Usages { get; } = [];
}

