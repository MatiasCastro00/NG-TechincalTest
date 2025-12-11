using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "State/State", order = 1)]
public class State : ScriptableObject
{
    public string stateName;
    public List<Skill> skills;
    public List<Skill> savedInventorySlots;
    public List<Skill> savedEquipmentSlots;
    public PlayerAnimatiorController model;
}
