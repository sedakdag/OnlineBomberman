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
    public class PlayerState
    {
        public string playerId   { get; set; }
        public float  x          { get; set; }
        public float  y          { get; set; }
        public bool   alive      { get; set; }

        public float  speed      { get; set; }
        public int    bombCount  { get; set; }
        public int    bombPower  { get; set; }
    }

    [System.Serializable]
    public class BombData
    {
        public string bombId    { get; set; }
        public string ownerId   { get; set; }
        public float  x         { get; set; }
        public float  y         { get; set; }
        public int    power     { get; set; }
        public float  fuseTimeMs{ get; set; }
    }

    public static class NetworkEvents
    {
        public const string PlayerJoined      = "PlayerJoined";
        public const string PlayerMoved       = "PlayerMoved";
        public const string BombPlaced        = "BombPlaced";
        public const string BombExploded      = "BombExploded";
        public const string WallDestroyed     = "WallDestroyed";
        public const string PowerUpCollected  = "PowerUpCollected";
        public const string PlayerDied        = "PlayerDied";
        public const string MatchEnded        = "MatchEnded";
    }
}
