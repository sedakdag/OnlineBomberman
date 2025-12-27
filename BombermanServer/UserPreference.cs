using Contracts; 
//theme ile alakalı ama ui da yok 
namespace BombermanServer
{
    public class UserPreference
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

       
        public ThemeType Theme { get; set; } = ThemeType.Desert;
    }
}
