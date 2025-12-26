using System;

namespace BombermanServer
{
    public class PlayerScore
    {
        public int Id { get; set; }                 // PK (auto increment)
        public string PlayerId { get; set; } = "";  // SignalR ConnectionId
        public string PlayerName { get; set; } = ""; // İstersen UI'dan isim alırsın

        public int Wins { get; set; }
        public int Losses { get; set; }

        public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
    }
}
