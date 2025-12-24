using System.Collections;
using System;
using UnityEngine;

public class PlayerPowerStats : MonoBehaviour
{
    public event Action Changed;

    public int bombCount = 1;
    public int bombPower = 2;
    public float speed = 5f;
    public bool reinforcedOneHit;

    Coroutine _bombPowerBuffRoutine;
    Coroutine _reinforcedRoutine;

    public void AddBombCount(int v) { bombCount += v; Changed?.Invoke(); }
    public void AddSpeed(float v)   { speed += v; Changed?.Invoke(); }

    // 🔥💣 3 saniyelik bomb power buff
    public void TriggerBombPowerBuff(int v, float duration)
    {
        if (_bombPowerBuffRoutine != null) StopCoroutine(_bombPowerBuffRoutine);
        _bombPowerBuffRoutine = StartCoroutine(BombPowerTemp(v, duration));
    }

    IEnumerator BombPowerTemp(int v, float duration)
    {
        bombPower += v;
        Changed?.Invoke();

        yield return new WaitForSeconds(duration);

        bombPower -= v;
        if (bombPower < 1) bombPower = 1; // istersen kaldırabilirim
        Changed?.Invoke();

        _bombPowerBuffRoutine = null;
    }

    // 3 saniyelik reinforced buff (refresh eder)
    public void StartReinforcedOneHitBuff(float duration)
    {
        if (_reinforcedRoutine != null) StopCoroutine(_reinforcedRoutine);
        _reinforcedRoutine = StartCoroutine(ReinforcedTemp(duration));
    }

    IEnumerator ReinforcedTemp(float duration)
    {
        reinforcedOneHit = true;
        Changed?.Invoke();

        yield return new WaitForSeconds(duration);

        reinforcedOneHit = false;
        Changed?.Invoke();

        _reinforcedRoutine = null;
    }
}
