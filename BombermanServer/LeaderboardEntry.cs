namespace BombermanServer
{
    /// <summary>
    /// Leaderboard için DTO.
    /// Unity tarafındaki LeaderboardEntry ile
    /// property isimleri uyuşuyor (username, wins, losses, totalGames).
    /// </summary>
    public class LeaderboardEntry
    {
        public string username { get; set; } = "";
        public int wins { get; set; }
        public int losses { get; set; }
        public int totalGames { get; set; }
    }
}
