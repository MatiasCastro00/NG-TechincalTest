using UnityEngine;
using System;

public abstract class Skill : ScriptableObject
{
    [SerializeField] private string m_name;

    [SerializeField] private string m_className;

#if UNITY_EDITOR
    [SerializeField] private UnityEditor.MonoScript m_script;

    private void OnValidate()
    {
        if (m_script != null)
        {
            var type = m_script.GetClass();

            if (type != null)
            {
                m_className = type.AssemblyQualifiedName;
            }
        }
    }
#endif

    public Type GetSkillType()
    {
        return Type.GetType(m_className);
    }

    public ISkillComponent InstantiateComponent(GameObject owner)
    {
        var type = GetSkillType();

        if (type == null)
        {
            Debug.LogError($"No se pudo cargar la clase: {m_className}");
            return null;
        }

        ISkillComponent skillComponent = null;

        if (typeof(MonoBehaviour).IsAssignableFrom(type))
        {
            var mb = owner.AddComponent(type) as MonoBehaviour;
            skillComponent = mb as ISkillComponent;
        }
        else
        {

            skillComponent = Activator.CreateInstance(type) as ISkillComponent;
        }


        if (skillComponent == null)
        {
            Debug.LogError($"El tipo {type.Name} no implementa ISkillComponent.");
            return null;
        }

        skillComponent.InitializeFromSkill(this, owner);

        return skillComponent;
    }

}
