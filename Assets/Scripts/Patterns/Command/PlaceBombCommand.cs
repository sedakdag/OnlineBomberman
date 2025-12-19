using UnityEngine;

public class PlaceBombCommand : ICommand
{
    private readonly BombSystem bombSystem;
    private readonly Transform playerTransform;
    private readonly Collider2D playerCollider;

    public PlaceBombCommand(BombSystem bombSystem, Transform playerTransform, Collider2D playerCollider)
    {
        this.bombSystem = bombSystem;
        this.playerTransform = playerTransform;
        this.playerCollider = playerCollider;
    }

    public void Execute()
    {
        if (bombSystem == null || playerTransform == null || playerCollider == null) return;
        bombSystem.TryPlaceBombAtWorld(playerTransform.position, playerCollider);
    }
}