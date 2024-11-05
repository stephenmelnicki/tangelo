namespace Tangelo.Data.Models;

public record Guild
{
    public ulong Id { get; set; }
    public required string Name { get; set; }

    public List<Usage> Usages { get; set; } = [];
}
