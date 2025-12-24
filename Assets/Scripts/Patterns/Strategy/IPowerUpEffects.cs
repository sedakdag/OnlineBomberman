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
        // ✅ Süreli +1 Bomb Power (3 saniye sonra geri alır)
        stats.TriggerBombPowerBuff(1, 3f);

        // ✅ Reinforced One Hit buff (3 saniye, refresh eder)
        stats.StartReinforcedOneHitBuff(3f);
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
            PowerUpType.BombCount  => new BombCountEffect(),
            PowerUpType.BombPower  => new BombPowerEffect(),
            PowerUpType.SpeedBoost => new SpeedBoostEffect(),
            _ => null
        };
    }
}