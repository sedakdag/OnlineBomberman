using System;

namespace Contracts
{
    [Serializable]
    public class GameResult
    {
        // Şimdilik player'ı ConnectionId ile takip ediyoruz
        public string playerName { get; set; } = string.Empty;

        // true = kazandı, false = kaybetti
        public bool isWin { get; set; }
    }
}
