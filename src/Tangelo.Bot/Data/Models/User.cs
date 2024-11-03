namespace Tangelo.Bot.Data.Models;

public record User
{
    public ulong Id { get; set; }
    public required string Username { get; set; }

    public List<Usage> Usages { get; } = [];
}
