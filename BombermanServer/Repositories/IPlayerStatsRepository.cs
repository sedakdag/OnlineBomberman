using System.Collections.Generic;
using System.Threading.Tasks;

namespace BombermanServer
{
    /// <summary>
    /// Player istatistikleri için Repository arayüzü.
    /// </summary>
    public interface IPlayerStatsRepository
    {
        /// <summary>
        /// Oyunun sonucuna göre ilgili kullanıcının
        /// istatistiklerini günceller (win/loss + last played).
        /// Gerekirse User + PlayerStats kaydını oluşturur.
        /// </summary>
        Task SubmitGameResultAsync(string playerName, bool didWin);

        /// <summary>
        /// En çok kazanan oyuncuların leaderboard’unu getirir.
        /// </summary>
        Task<List<LeaderboardEntry>> GetLeaderboardAsync(int top);
    }
}
