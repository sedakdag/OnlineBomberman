namespace Contracts
{
    public class LeaderboardEntry
    {
        public string username { get; set; } = string.Empty;
        public int wins { get; set; }
        public int losses { get; set; }
        public int totalGames { get; set; }
    }
}
