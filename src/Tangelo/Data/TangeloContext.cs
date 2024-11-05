using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Tangelo.Data.Models;

namespace Tangelo.Data;

public class TangeloContext(IConfiguration configuration) : DbContext
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

        modelBuilder.Entity<Usage>()
            .Property(e => e.Created)
            .HasDefaultValueSql("getutcdate()");

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

        foreach (var name in Enum.GetValues<CommandName>())
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
                DataSource = IsDevelopment(config)
                        ? "127.0.0.1,1433"
                        : "sqlserver,1433",
            };

        return builder.ConnectionString;
    }

    private static bool IsDevelopment(IConfiguration config) =>
        config["ASPNETCORE_ENVIRONMENT"] == Environments.Development ||
        config["DOTNET_ENVIRONMENT"] == Environments.Development;
}
