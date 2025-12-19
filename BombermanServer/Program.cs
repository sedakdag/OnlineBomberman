using BombermanServer;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSignalR();

var app = builder.Build();

// Unity'nin bağlanacağı adresi belirliyoruz: http://localhost:5000/gameHub
app.MapHub<GameHub>("/gameHub");


Console.WriteLine("=============================================");
Console.WriteLine(" BOMBERMAN SERVER (SignalR) HAZIR!");
Console.WriteLine(" Dinleniyor: http://localhost:5000/gameHub");
Console.WriteLine("=============================================");

app.Run();
