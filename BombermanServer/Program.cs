// Program.cs
using BombermanServer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// SignalR
builder.Services.AddSignalR();

// EF Core context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=bomberman.db"));

// 🔹 Repository Pattern: PlayerStats repository kaydı
builder.Services.AddScoped<IPlayerStatsRepository, PlayerStatsRepository>();
builder.Services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();
var app = builder.Build();

// DB yoksa oluştur
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapHub<GameHub>("/gameHub");

Console.WriteLine("=============================================");
Console.WriteLine(" BOMBERMAN SERVER (SignalR + EF Core) HAZIR!");
Console.WriteLine(" Dinleniyor: http://localhost:5100/gameHub");
Console.WriteLine("=============================================");

app.Run("http://localhost:5100");
