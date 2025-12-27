using System.Collections.Generic;
using System.Threading.Tasks;
//domain ile veri arasinda soyut -repo
namespace BombermanServer
{
    
    
    public interface IPlayerStatsRepository
    {
               Task SubmitGameResultAsync(string playerName, bool didWin);

        
        Task<List<LeaderboardEntry>> GetLeaderboardAsync(int top);
    }
}
