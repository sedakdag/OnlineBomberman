using System.Threading.Tasks;
using Contracts; // ThemeType

namespace BombermanServer
{
    public interface IUserPreferencesRepository
    {
        Task<ThemeType> GetPreferredThemeAsync(string username);
        Task SetPreferredThemeAsync(string username, ThemeType theme);
    }
}
