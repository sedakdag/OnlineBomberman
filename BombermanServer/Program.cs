using BombermanServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<GameHub>("/gameHub");

Console.WriteLine("=============================================");
Console.WriteLine(" BOMBERMAN SERVER (SignalR) HAZIR!");
Console.WriteLine(" Dinleniyor: http://localhost:5100/gameHub"); // Yazıyı güncelledik
Console.WriteLine("=============================================");

// DİKKAT: Parantez içine yeni portu yazdık (5100)
app.Run("http://localhost:5100");
