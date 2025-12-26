using System;

namespace BombermanServer
{
    public class User
    {
        public int Id { get; set; }

        // Unity'den gelen oyuncu ismi (ör: "Unity_Test_Oyuncusu")
        public string Username { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 1-1 ilişkiler
        public PlayerStats? Stats { get; set; }
        public UserPreference? Preferences { get; set; }
    }
}
