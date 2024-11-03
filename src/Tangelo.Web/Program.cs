using Tailwind;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStatusCodePages();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    _ = app.RunTailwind("tailwind:watch", "./");
}

app.Run();
