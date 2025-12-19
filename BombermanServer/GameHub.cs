using Microsoft.AspNetCore.SignalR;
using Contracts; 

namespace BombermanServer
{
    public class GameHub : Hub
    {
        
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[BAĞLANTI] Yeni oyuncu geldi! ID: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[AYRILMA] Oyuncu çıktı. ID: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        

        
        public async Task JoinGame(string playerName)
        {
            Console.WriteLine($"[LOG] JoinGame İsteği: {playerName} ({Context.ConnectionId})");

            
            await Clients.Others.SendAsync(NetworkEvents.PlayerJoined, playerName);
        }

        
        public async Task MovePlayer(float x, float y)
        {
            
            Console.WriteLine($"[LOG] Hareket: {Context.ConnectionId} -> X:{x} Y:{y}");
        }
    }
}
