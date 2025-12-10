using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : NullableSingleton<EventManager>
{
    public Action<State> PlayerStateChanged;
    public Action<int> PlayerHealthUpdated;
    public Action PlayerTookDamage;
    public Action PlayerDied;
    public Action EnemyDied;
    public Action<float, float> ManaUpdated;
    public Action<State> PlayerGainedNewSkill;


    public void RaisePlayerStateChanged(State s) => PlayerStateChanged?.Invoke(s);
    public void RaisePlayerHealthUpdated(int hp) => PlayerHealthUpdated?.Invoke(hp);
    public void RaisePlayerTookDamage() => PlayerTookDamage?.Invoke();
    public void RaisePlayerDied() => PlayerDied?.Invoke();
    public void RaiseEnemyDied() => EnemyDied?.Invoke();
    public void RaiseManaUpdated(float current, float max) => ManaUpdated?.Invoke(current, max);
    public void RaisePlayerGainedNewSkill(State s) => PlayerGainedNewSkill?.Invoke(s);
}
