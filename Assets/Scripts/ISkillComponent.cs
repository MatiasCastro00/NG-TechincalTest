using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillComponent
{
    void InitializeFromSkill(Skill skill, GameObject owner);
    void OnRemove();
}
