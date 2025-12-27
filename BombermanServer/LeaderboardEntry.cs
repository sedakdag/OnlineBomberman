namespace BombermanServer
{
    
    public class LeaderboardEntry
    {
        public string username { get; set; } = "";
        public int wins { get; set; }
        public int losses { get; set; }
        public int totalGames { get; set; }
    }
}
