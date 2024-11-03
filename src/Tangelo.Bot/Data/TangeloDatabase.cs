using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Tangelo.Bot.Data.Models;

namespace Tangelo.Bot.Data;

public class TangeloDatabase(IConfiguration configuration) : DbContext
{
    public DbSet<Command> Commands { get; set; }
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Usage> Usages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString(configuration));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // allow direct assignment of the guild id
        modelBuilder.Entity<Guild>()
            .Property(e => e.Id)
            .ValueGeneratedNever();

        // allow direct assignment of the user id
        modelBuilder.Entity<User>()
            .Property(e => e.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Command>()
            .HasMany(e => e.Usages)
            .WithOne(e => e.Command)
            .HasForeignKey(e => e.CommandId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired();

        modelBuilder.Entity<Guild>()
            .HasMany(e => e.Usages)
            .WithOne(e => e.Guild)
            .HasForeignKey(e => e.GuildId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasMany(e => e.Usages)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired();

        foreach (CommandName name in Enum.GetValues<CommandName>())
        {
            modelBuilder.Entity<Command>()
                .HasData(new Command { Id = (int)name, Name = name });
        }
    }

    private static string GetConnectionString(IConfiguration config)
    {
        SqlConnectionStringBuilder builder =
            new(config.GetConnectionString("DefaultConnection"))
            {
                Password = config["MSSQL_SA_PASSWORD"],
                DataSource = config["DOTNET_ENVIRONMENT"] == Environments.Development
                    ? "127.0.0.1,1433"
                    : "sqlserver,1433",
            };

        return builder.ConnectionString;
    }
}
