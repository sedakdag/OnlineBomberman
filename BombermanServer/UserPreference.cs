using Contracts; // ThemeType burada

namespace BombermanServer
{
    public class UserPreference
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Desert / Forest / City (ThemeType enum'u)
        public ThemeType Theme { get; set; } = ThemeType.Desert;
    }
}
