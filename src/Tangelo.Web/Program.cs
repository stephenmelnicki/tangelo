var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStatusCodePages();
app.UseStaticFiles();
app.MapRazorPages();
app.MapHealthChecks("/health");

app.Run();
