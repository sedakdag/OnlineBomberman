namespace BombermanServer;

public class MatchRecord
{
    public int Id { get; set; }
    public string WinnerPlayerId { get; set; } = "";
    public int DurationSeconds { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
