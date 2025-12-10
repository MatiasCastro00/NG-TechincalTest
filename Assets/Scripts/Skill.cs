using UnityEngine;
using System;

public abstract class Skill : ScriptableObject
{
    [SerializeField] private string m_name;

    // Nombre de la clase del componente que se va a instanciar
    [SerializeField] private string m_className;

    // Slot de esta skill (Movement / Jump / Dash / Ability / None)
    [SerializeField] private SkillSlot m_slot = SkillSlot.None;
    public SkillSlot Slot => m_slot;
    public string Name => m_name;

#if UNITY_EDITOR
    // Para poder arrastrar el script en el inspector y rellenar m_className solo
    [SerializeField] private UnityEditor.MonoScript m_script;

    private void OnValidate()
    {
        if (m_script != null)
        {
            var type = m_script.GetClass();
            if (type != null)
            {
                // AssemblyQualifiedName asegura que Type.GetType lo encuentre bien
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

        // Si es un MonoBehaviour, lo agregamos como componente al GameObject
        if (typeof(MonoBehaviour).IsAssignableFrom(type))
        {
            var mb = owner.AddComponent(type) as MonoBehaviour;
            skillComponent = mb as ISkillComponent;
        }
        else
        {
            // Si no es MonoBehaviour, lo instanciamos vía Activator
            skillComponent = Activator.CreateInstance(type) as ISkillComponent;
        }

        if (skillComponent == null)
        {
            Debug.LogError($"El tipo {type.Name} no implementa ISkillComponent.");
            return null;
        }

        // Esta línea es la importante que mencionaste
        skillComponent.InitializeFromSkill(this, owner);

        return skillComponent;
    }
}
