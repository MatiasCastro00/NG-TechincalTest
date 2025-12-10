using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSkill : Skill
{
    [SerializeField] private string m_actionName;
    public string ActionName => m_actionName;

    public void OverrideActionName(string newAction)
    {
        m_actionName = newAction;
    }
}
