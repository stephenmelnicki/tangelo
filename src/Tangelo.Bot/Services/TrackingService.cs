using Microsoft.EntityFrameworkCore;

using Discord;

using Tangelo.Bot.Data;
using Tangelo.Bot.Data.Models;

namespace Tangelo.Bot.Services;

public interface ITrackingService
{
    Task TrackUsageAsync(string commandName, IGuild guild, IUser user);
}

public class TrackingService : ITrackingService
{
    private readonly TangeloContext _db;

    public TrackingService(IServiceScopeFactory scopeFactory)
    {
        // We can't inject the database context directly because it's scoped and 
        // this is a singleton service.
        _db = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TangeloContext>();

        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
        }
    }

    public async Task TrackUsageAsync(string commandName, IGuild guild, IUser user)
    {
        var command = await FindCommandAsync(commandName);
        if (command == null)
        {
            return;
        }

        var guildResult = await FindGuildAsync(guild.Id) ?? await CreateGuildAsync(guild.Id, guild.Name);
        var userResult = await FindUserAsync(user.Id) ?? await CreateUserAsync(user.Id, user.Username);

        await CreateUsageAsync(command, guildResult, userResult);
        await _db.SaveChangesAsync();
    }

    private async ValueTask<Command?> FindCommandAsync(string name)
    {
        var command = Enum.TryParse<CommandName>(name, true, out var commandName)
            ? await _db.Commands.FirstOrDefaultAsync(c => c.Name == commandName)
            : null;

        return command;
    }

    private ValueTask<Guild?> FindGuildAsync(ulong id)
    {
        return _db.Guilds.FindAsync(id);
    }

    private async ValueTask<Guild> CreateGuildAsync(ulong id, string name)
    {
        Guild guild = new() { Id = id, Name = name };

        await _db.Guilds.AddAsync(guild);
        return guild;
    }

    private ValueTask<User?> FindUserAsync(ulong id)
    {
        return _db.Users.FindAsync(id);
    }

    private async ValueTask<User> CreateUserAsync(ulong id, string username)
    {
        User user = new() { Id = id, Username = username };

        await _db.Users.AddAsync(user);
        return user;
    }

    private async ValueTask<Usage> CreateUsageAsync(Command command, Guild guild, User user)
    {
        Usage usage = new()
        {
            CommandId = command.Id,
            Command = command,
            GuildId = guild.Id,
            Guild = guild,
            UserId = user.Id,
            User = user
        };

        await _db.Usages.AddAsync(usage);
        return usage;
    }

}
