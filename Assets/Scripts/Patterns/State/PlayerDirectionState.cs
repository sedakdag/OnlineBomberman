using UnityEngine;

public interface IPlayerDirectionState
{
    Vector2Int Dir { get; }
    void Apply(PlayerDirectionContext ctx);
}

[System.Serializable]
public class DirectionSprites
{
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;
}

public class PlayerDirectionContext
{
    public readonly SpriteRenderer renderer;
    public readonly DirectionSprites sprites;

    public PlayerDirectionContext(SpriteRenderer renderer, DirectionSprites sprites)
    {
        this.renderer = renderer;
        this.sprites = sprites;
    }

    public void SetSprite(Sprite s)
    {
        if (renderer != null && s != null) renderer.sprite = s;
    }
}

public class UpState : IPlayerDirectionState
{
    public Vector2Int Dir => Vector2Int.up;
    public void Apply(PlayerDirectionContext ctx) => ctx.SetSprite(ctx.sprites.up);
}
public class DownState : IPlayerDirectionState
{
    public Vector2Int Dir => Vector2Int.down;
    public void Apply(PlayerDirectionContext ctx) => ctx.SetSprite(ctx.sprites.down);
}
public class LeftState : IPlayerDirectionState
{
    public Vector2Int Dir => Vector2Int.left;
    public void Apply(PlayerDirectionContext ctx) => ctx.SetSprite(ctx.sprites.left);
}
public class RightState : IPlayerDirectionState
{
    public Vector2Int Dir => Vector2Int.right;
    public void Apply(PlayerDirectionContext ctx) => ctx.SetSprite(ctx.sprites.right);
}