using System;
using System.Threading.Tasks;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace BombermanServer
{
    public class UserPreferencesRepository : IUserPreferencesRepository
    {
        private readonly AppDbContext _db;

        public UserPreferencesRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ThemeType> GetPreferredThemeAsync(string username)
        {
            // User + Preferences join
            var user = await _db.Users
                .Include(u => u.Preferences)   // birazdan User'a nav ekleyeceğiz
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.Preferences == null)
                return ThemeType.Desert; // default

            return user.Preferences.Theme;
        }

        public async Task SetPreferredThemeAsync(string username, ThemeType theme)
        {
            var user = await _db.Users
                .Include(u => u.Preferences)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                user = new User
                {
                    Username = username,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            if (user.Preferences == null)
            {
                user.Preferences = new UserPreference
                {
                    UserId = user.Id,
                    Theme = theme
                };
                _db.UserPreferences.Add(user.Preferences);
            }
            else
            {
                user.Preferences.Theme = theme;
            }

            await _db.SaveChangesAsync();
        }
    }
}
