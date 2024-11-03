using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tangelo.Bot.Data;
using Tangelo.Bot.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services
    .AddSingleton(new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(serviceProvider => new InteractionService(serviceProvider.GetRequiredService<DiscordSocketClient>()))
    .AddSingleton<LoggingService>()
    .AddDbContext<TangeloDatabase>()
    .AddSingleton<DataService>()
    .AddHostedService<BotService>();

var host = builder.Build();
host.Run();
