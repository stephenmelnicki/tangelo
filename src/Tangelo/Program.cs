using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tangelo.Data;
using Tangelo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddHealthChecks();

builder.Services.AddRazorPages();

builder.Services
    .AddDbContext<TangeloContext>()
    .AddSingleton<ILoggingService, LoggingService>()
    .AddSingleton<ITrackingService, TrackingService>()
    .AddSingleton(new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(serviceProvider => new InteractionService(serviceProvider.GetRequiredService<DiscordSocketClient>()))
    .AddHostedService<BotService>();

var app = builder.Build();

app.UseStatusCodePages();
app.UseStaticFiles();
app.MapRazorPages();

app.MapHealthChecks("/healthz");

app.Run();
