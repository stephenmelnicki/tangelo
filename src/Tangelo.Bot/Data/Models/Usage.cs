namespace Tangelo.Bot.Data.Models;

public class Usage
{
    public int Id { get; set; }
    public DateTime Created { get; set; }

    public int CommandId { get; set; }
    public Command Command { get; set; } = null!;

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public ulong UserId { get; set; }
    public User User { get; set; } = null!;
}
