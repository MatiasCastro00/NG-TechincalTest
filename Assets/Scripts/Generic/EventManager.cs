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
    public Action PlayerStep1;
    public Action PlayerStep2;
    public Action PlayerStep3;
    public Action OpenGate;
    public Action Talk;
    public Action EndLvl;

    public Action<List<Skill>> PlayerEquipmentChanged;

    public void RaisePlayerStateChanged(State s) => PlayerStateChanged?.Invoke(s);
    public void RaisePlayerDied() => PlayerDied?.Invoke();
    public void RaisePlayerStep() => PlayerStep1?.Invoke();
    public void RaisePlayerStep2() => PlayerStep2?.Invoke();
    public void RaisePlayerStep3() => PlayerStep3?.Invoke();
    public void RaiseOpenGate() => OpenGate?.Invoke();
    public void RaiseTalk() => Talk?.Invoke();
    public void RaiseEndLvl() => EndLvl?.Invoke();

    public void RaisePlayerEquipmentChanged(List<Skill> equippedSkills)
        => PlayerEquipmentChanged?.Invoke(equippedSkills);
}
