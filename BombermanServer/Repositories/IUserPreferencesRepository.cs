using System.Threading.Tasks;
using Contracts; 

namespace BombermanServer
{
    public interface IUserPreferencesRepository
    {
        Task<ThemeType> GetPreferredThemeAsync(string username);
        Task SetPreferredThemeAsync(string username, ThemeType theme);
    }
}
