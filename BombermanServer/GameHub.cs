// GameHub.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BombermanServer
{
    public class GameHub : Hub
    {
        private readonly AppDbContext _db;

        public GameHub(AppDbContext db)
        {
            _db = db;
        }

        // ----------------- Bağlantılar -----------------
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"[JOIN] {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[LEAVE] {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }

        // ----------------- Oyuna Katıl -----------------
        public Task JoinGame(string playerName)
        {
            Console.WriteLine($"[JOIN GAME] playerName={playerName}, connId={Context.ConnectionId}");
            return Task.CompletedTask;
        }

        // ----------------- Hareket / Bomba -----------------
        public async Task SendMove(MoveState state)
        {
            if (state == null)
            {
                Console.WriteLine("[SERVER] SendMove: state NULL geldi!");
                return;
            }

            Console.WriteLine(
                $"[SERVER] MOVE from {state.playerName} -> ({state.x}, {state.y}) dir={state.direction}"
            );

            await Clients.Others.SendAsync("PlayerMoved", state);
        }

        public async Task PlaceBomb(BombData data)
        {
            if (data == null)
            {
                Console.WriteLine("[SERVER] PlaceBomb: data NULL geldi!");
                return;
            }

            Console.WriteLine(
                $"[SERVER] BOMB PLACED id={data.bombId} owner={data.ownerId} pos=({data.x},{data.y}) power={data.power}"
            );

            await Clients.Others.SendAsync("BombPlaced", data);
        }

        public async Task NotifyBombExploded(BombData data)
        {
            if (data == null)
            {
                Console.WriteLine("[SERVER] NotifyBombExploded: data NULL geldi!");
                return;
            }

            Console.WriteLine(
                $"[SERVER] BOMB EXPLODED id={data.bombId} pos=({data.x},{data.y})"
            );

            await Clients.Others.SendAsync("BombExploded", data);
        }

        public async Task NotifyPlayerDied(string playerId)
        {
            Console.WriteLine($"[SERVER] PLAYER DIED: {playerId}");
            await Clients.All.SendAsync("PlayerDied", playerId);
        }

        // ----------------- GAME RESULT + LEADERBOARD -----------------
        // Unity: await connection.InvokeAsync("SubmitGameResult", playerName, didWin);
        public async Task SubmitGameResult(string playerName, bool didWin)
        {
            Console.WriteLine($"[GAME RESULT] user={playerName} win={didWin}");

            // 1) Kullanıcıyı username ile bul, stats ile birlikte al
            var user = await _db.Users
                .Include(u => u.Stats)
                .SingleOrDefaultAsync(u => u.Username == playerName);

            // 2) Yoksa kullanıcı + stats oluştur
            if (user == null)
            {
                user = new User
                {
                    Username = playerName,
                    CreatedAt = DateTime.UtcNow
                };

                user.Stats = new PlayerStats
                {
                    Wins = 0,
                    Losses = 0,
                    LastPlayedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
            }
            else if (user.Stats == null)
            {
                // Kullanıcı var ama istatistik kaydı yoksa
                user.Stats = new PlayerStats
                {
                    Wins = 0,
                    Losses = 0,
                    LastPlayedAt = DateTime.UtcNow
                };
            }

            var stats = user.Stats!;

            // 3) Win/Loss güncelle
            if (didWin)
                stats.Wins += 1;
            else
                stats.Losses += 1;

            stats.LastPlayedAt = DateTime.UtcNow;

            // 4) DB'ye yaz
            await _db.SaveChangesAsync();
        }

        // Unity: await connection.InvokeAsync("GetLeaderboard", 5);
        public async Task<List<LeaderboardEntry>> GetLeaderboard(int top = 10)
        {
            try
            {
                // PlayerStats tablosundan, User ile birlikte en çok kazananları çek
                var topPlayers = await _db.PlayerStats
                    .Include(ps => ps.User)
                    .OrderByDescending(ps => ps.Wins)
                    .ThenBy(ps => ps.Losses)
                    .Take(top)
                    .ToListAsync();

                var result = topPlayers
                    .Select(ps => new LeaderboardEntry
                    {
                        username = ps.User?.Username ?? "(unknown)",
                        wins = ps.Wins,
                        losses = ps.Losses,
                        totalGames = ps.Wins + ps.Losses
                    })
                    .ToList();

                Console.WriteLine($"[HUB] GetLeaderboard OK. count={result.Count}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HUB] GetLeaderboard ERROR: " + ex);
                // Unity tarafı hata logluyor zaten; burada boş liste döneriz
                return new List<LeaderboardEntry>();
            }
        }
    }

   
}
