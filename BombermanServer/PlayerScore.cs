using System;

namespace BombermanServer
{
    public class PlayerScore
    {
        public int Id { get; set; }                
        public string PlayerId { get; set; } = "";  
        public string PlayerName { get; set; } = ""; 

        public int Wins { get; set; }
        public int Losses { get; set; }

        public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
    }
}
