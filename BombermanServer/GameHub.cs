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
            
            UserRepository repo = new UserRepository();
            repo.AddPlayer(playerName);

            
            await Clients.Others.SendAsync(NetworkEvents.PlayerJoined, playerName);
        }

        
       // Unity'den gelen x, y ve yön bilgisini alır, diğer oyunculara dağıtır
        public async Task MovePlayer(float x, float y, int direction)
        {
            Console.WriteLine($"[HAREKET] {Context.ConnectionId} -> X:{x} Y:{y}");
            // 1. Veriyi paketle (Az önce oluşturduğumuz MoveState sınıfı)
            var moveData = new MoveState
            {
                PlayerName = Context.ConnectionId, // Kim hareket etti? (Bağlantı ID'si)
                X = x,
                Y = y,
                Direction = direction
            };

            // 2. Bu paketi, gönderen kişi HARİÇ diğer herkese (Others) yolla
            // "PlayerMoved" etiketiyle gönderiyoruz, Unity tarafında bu etiketi dinleyeceğiz.
            await Clients.Others.SendAsync("PlayerMoved", moveData);
        }
    }
}
