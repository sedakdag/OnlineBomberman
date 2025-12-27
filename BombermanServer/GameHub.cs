
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;                   
using Microsoft.AspNetCore.SignalR;

namespace BombermanServer
{
    public class GameHub : Hub
    {
        private readonly IPlayerStatsRepository _statsRepo;

        public GameHub(IPlayerStatsRepository statsRepo)
        {
            _statsRepo = statsRepo;
        }

        // bağlantı- template, when connected or disconnected calls for the methods that i overrode
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

        // oyuna katıl
        public Task JoinGame(string playerName)
        {
            Console.WriteLine($"[JOIN GAME] playerName={playerName}, connId={Context.ConnectionId}");
            return Task.CompletedTask;
        }

        // bomba ve hareket
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

        //GAME RESULT + LEADERBOARD 
        
        public async Task SubmitGameResult(string playerName, bool didWin)
        {
            Console.WriteLine($"[GAME RESULT] user={playerName} win={didWin}");

            try
            {
                await _statsRepo.SubmitGameResultAsync(playerName, didWin);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HUB] SubmitGameResult ERROR: " + ex);
                
                throw;
            }
        }

       
        public async Task<List<LeaderboardEntry>> GetLeaderboard(int top = 10)
        {
            try
            {
                var result = await _statsRepo.GetLeaderboardAsync(top);
                Console.WriteLine($"[HUB] GetLeaderboard OK. count={result.Count}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HUB] GetLeaderboard ERROR: " + ex);
                return new List<LeaderboardEntry>();
            }
        }
    }
}
