using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "State", menuName = "State/State", order = 1)]
public class State : ScriptableObject
{
    public string stateName;

    public List<Skill> skills;
    public PlayerAnimatiorController model;
}
