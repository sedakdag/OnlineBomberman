using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BombermanServer
{
    public class PlayerStats
    {
        public int Id { get; set; }  // PK

        // FK → User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int Wins { get; set; }
        public int Losses { get; set; }

        // TotalGames = Wins + Losses (DB'de kolon yok, sadece hesaplanan property)
        [NotMapped]
        public int TotalGames => Wins + Losses;

        public DateTime LastPlayedAt { get; set; }
    }
}
