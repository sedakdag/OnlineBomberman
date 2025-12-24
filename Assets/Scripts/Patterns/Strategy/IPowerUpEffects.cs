public interface IPowerUpEffect
{
    void Apply(PlayerPowerStats stats);
}

public class BombCountEffect : IPowerUpEffect
{
    public void Apply(PlayerPowerStats stats) => stats.AddBombCount(1);
}

public class BombPowerEffect : IPowerUpEffect
{
    public void Apply(PlayerPowerStats stats)
    {
        // 3 saniyelik reinforced tek-hit buff
        stats.StartCoroutine(stats.ReinforcedOneHitBuff(3f));
    }
}

public class SpeedBoostEffect : IPowerUpEffect
{
    public void Apply(PlayerPowerStats stats) => stats.AddSpeed(1.5f);
}

public static class PowerUpEffectFactory
{
    public static IPowerUpEffect Create(PowerUpType type)
    {
        return type switch
        {
            PowerUpType.BombCount => new BombCountEffect(),
            PowerUpType.BombPower => new BombPowerEffect(),
            PowerUpType.SpeedBoost => new SpeedBoostEffect(),
            _ => null
        };
    }
}
