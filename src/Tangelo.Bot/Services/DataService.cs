using Microsoft.EntityFrameworkCore;

using Tangelo.Bot.Data;
using Tangelo.Bot.Data.Models;

namespace Tangelo.Bot.Services;

public class DataService
{
    private readonly TangeloDatabase _db;

    public DataService(IServiceScopeFactory scopeFactory)
    {
        // Can't inject the database context directly because it's scoped and this 
        // is a singleton service.
        _db = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TangeloDatabase>();

        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
        }
    }

    public async ValueTask<Command?> FindCommandAsync(string name)
    {
        Command? command = Enum.TryParse<CommandName>(name, true, out var commandName)
            ? await _db.Commands.FirstOrDefaultAsync(c => c.Name == commandName)
            : null;

        return command;
    }

    public ValueTask<Guild?> FindGuildAsync(ulong id)
    {
        return _db.Guilds.FindAsync(id);
    }

    public async ValueTask<Guild> CreateGuildAsync(ulong id, string name)
    {
        Guild guild = new() { Id = id, Name = name };

        await _db.Guilds.AddAsync(guild);
        await _db.SaveChangesAsync();

        return guild;
    }

    public ValueTask<User?> FindUserAsync(ulong id)
    {
        return _db.Users.FindAsync(id);
    }

    public async ValueTask<User> CreateUserAsync(ulong id, string username)
    {
        User user = new() { Id = id, Username = username };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return user;
    }

    public async ValueTask<Usage> CreateUsageAsync(Command command, Guild guild, User user)
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
        await _db.SaveChangesAsync();

        return usage;
    }

}
