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
    .AddDbContext<TangeloContext>()
    .AddSingleton<LoggingService>()
    .AddSingleton<TrackingService>()
    .AddSingleton(new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(serviceProvider => new InteractionService(serviceProvider.GetRequiredService<DiscordSocketClient>()))
    .AddHostedService<BotService>();

var host = builder.Build();
host.Run();
