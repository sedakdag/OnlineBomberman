// Repositories/PlayerStatsRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;                  // LeaderboardEntry burada
using Microsoft.EntityFrameworkCore;

namespace BombermanServer
{
    /// <summary>
    /// Oyuncu istatistiklerini (Wins/Losses/LastPlayedAt) yöneten repository.
    /// Ayrı Leaderboard tablosu YOK; PlayerStats + Users üzerinden hesaplıyoruz.
    /// </summary>
    public class PlayerStatsRepository : IPlayerStatsRepository
    {
        private readonly AppDbContext _db;

        public PlayerStatsRepository(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// İç helper: Kullanıcıyı ve stats kaydını getirir; yoksa oluşturur.
        /// </summary>
        private async Task<PlayerStats> GetOrCreateStatsInternalAsync(string username)
        {
            // User + Stats birlikte çek
            var user = await _db.Users
                .Include(u => u.Stats)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                // Yeni user
                user = new User
                {
                    Username = username,
                    CreatedAt = DateTime.UtcNow
                };

                var stats = new PlayerStats
                {
                    User = user,
                    Wins = 0,
                    Losses = 0,
                    LastPlayedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                _db.PlayerStats.Add(stats);
                await _db.SaveChangesAsync();

                return stats;
            }

            // User var ama Stats yoksa oluştur
            if (user.Stats == null)
            {
                var stats = new PlayerStats
                {
                    UserId = user.Id,
                    Wins = 0,
                    Losses = 0,
                    LastPlayedAt = DateTime.UtcNow
                };

                _db.PlayerStats.Add(stats);
                await _db.SaveChangesAsync();

                return stats;
            }

            return user.Stats;
        }

        // === IPlayerStatsRepository IMPLEMENTATION ===

        /// <summary>
        /// Interface’in istediği imza:
        /// SubmitGameResultAsync(string playerName, bool didWin)
        /// </summary>
        public async Task SubmitGameResultAsync(string username, bool didWin)
        {
            var stats = await GetOrCreateStatsInternalAsync(username);

            if (didWin)
                stats.Wins += 1;
            else
                stats.Losses += 1;

            stats.LastPlayedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Interface’in istediği imza:
        /// GetLeaderboardAsync(int topN)
        /// </summary>
        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync(int topN)
        {
            var query = _db.PlayerStats
                .OrderByDescending(p => p.Wins)
                .ThenBy(p => p.Losses)
                .Take(topN)
                .Join(
                    _db.Users,
                    stats => stats.UserId,
                    user => user.Id,
                    (stats, user) => new LeaderboardEntry
                    {
                        username   = user.Username,
                        wins       = stats.Wins,
                        losses     = stats.Losses,
                        totalGames = stats.TotalGames
                    }
                );

            return await query.ToListAsync();
        }
    }
}
