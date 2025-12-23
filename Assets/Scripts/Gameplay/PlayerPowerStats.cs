using System.Collections;
using UnityEngine;

public class PlayerPowerStats : MonoBehaviour
{
    [Header("Stats")]
    public int bombCount = 1;
    public int bombPower = 2;
    public float speed = 5f;

    [Header("Temporary Buffs")]
    public bool reinforcedOneHit;   // 🔥 BombPower buff flag

    public void AddBombCount(int v) => bombCount += v;
    public void AddBombPower(int v) => bombPower += v;
    public void AddSpeed(float v) => speed += v;
    
    public IEnumerator ReinforcedOneHitBuff(float duration)
    {
        reinforcedOneHit = true;
        yield return new WaitForSeconds(duration);
        reinforcedOneHit = false;
    }
}