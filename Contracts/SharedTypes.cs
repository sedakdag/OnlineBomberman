namespace Contracts
{
    
    public enum ThemeType 
    { 
        Desert, 
        Forest, 
        City 
    }

   
    public enum WallType 
    { 
        Unbreakable, 
        Breakable, 
        Hard 
    }

    
    public enum PowerUpType 
    { 
        BombCount, 
        BombPower, 
        SpeedBoost 
    }

    
    [System.Serializable]
    public struct PlayerState
    {
        public string PlayerId;
        public float X;
        public float Y;
        public bool IsAlive;
        public float Speed;
        public int BombCount;
        public int BombPower;
    }

    
    [System.Serializable]
    public struct BombData
    {
        public string BombId;
        public string OwnerId;
        public float X;
        public float Y;
        public int Power;
        public float FuseTimeMs; 
    }

    
    public static class NetworkEvents
    {
        public const string PlayerJoined = "PlayerJoined";
        public const string PlayerMoved = "PlayerMoved";
        public const string BombPlaced = "BombPlaced";
        public const string BombExploded = "BombExploded";
        public const string WallDestroyed = "WallDestroyed";
        public const string PowerUpCollected = "PowerUpCollected";
        public const string PlayerDied = "PlayerDied";
        public const string MatchEnded = "MatchEnded";
    }
}