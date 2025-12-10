using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSkill : Skill
{
    [SerializeField] string m_actionName;
    public string ActionName => m_actionName;
}
