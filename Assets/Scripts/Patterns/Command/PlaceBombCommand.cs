using UnityEngine;

public class PlaceBombCommand : ICommand
{
    private readonly BombSystem bombSystem;
    private readonly Transform playerTransform;
    private readonly Collider2D playerCollider;
    private readonly PlayerPowerStats playerStats;   // 🔥 EKLENDİ

    public PlaceBombCommand(
        BombSystem bombSystem,
        Transform playerTransform,
        Collider2D playerCollider,
        PlayerPowerStats playerStats   // 🔥 EKLENDİ
    )
    {
        this.bombSystem = bombSystem;
        this.playerTransform = playerTransform;
        this.playerCollider = playerCollider;
        this.playerStats = playerStats;
    }

    public void Execute()
    {
        if (bombSystem == null || playerTransform == null || playerCollider == null || playerStats == null)
            return;

        // 🔥 Artık stats bilgisi bombaya gidiyor
        bombSystem.TryPlaceBombAtWorld(
            playerTransform.position,
            playerCollider,
            playerStats
        );
    }
}