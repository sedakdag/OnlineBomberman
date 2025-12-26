using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BombermanServer
{
    public class PlayerStats
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // ⭐ Varsayılan değerler burada veriliyor
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;

        [NotMapped]
        public int TotalGames => Wins + Losses;

        public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
    }
}
