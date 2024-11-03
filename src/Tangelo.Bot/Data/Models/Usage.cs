namespace Tangelo.Bot.Data.Models;

public class Usage
{
    public int Id { get; set; }

    public int CommandId { get; set; }
    public required Command Command { get; set; }

    public ulong GuildId { get; set; }
    public required Guild Guild { get; set; }

    public ulong UserId { get; set; }
    public required User User { get; set; }
}
