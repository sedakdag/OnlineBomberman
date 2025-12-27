using System;

namespace BombermanServer
{
    public class User
    {
        public int Id { get; set; }

                public string Username { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public PlayerStats? Stats { get; set; }
        public UserPreference? Preferences { get; set; }
    }
}
