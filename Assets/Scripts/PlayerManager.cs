using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager m_inputManager;
    [SerializeField] private State m_baseState;
    [SerializeField] private GroundDetector m_groundDetector;
    private State m_runtimeState;
    private State m_currentState;

    private float m_lastDirection = 1f;

    public GroundDetector GroundDetector { get => m_groundDetector;}

    private void Start()
    {
        m_runtimeState = ScriptableObject.Instantiate(m_baseState);
        m_runtimeState.skills = new List<Skill>(m_baseState.skills);

        ChangeCurrentState(m_runtimeState);
    }

    public void SetLastDirection(float direction)
    {
        m_lastDirection = direction;
    }

    private IEnumerator SpawnNewModel(Animator animator)
    {
        if (m_inputManager == null) yield break;

        yield return new WaitForEndOfFrame();

        animator.transform.SetParent(this.transform);
        animator.transform.localPosition = Vector3.zero;
        animator.transform.localScale = new Vector3(Mathf.Abs(animator.transform.localScale.x) * m_lastDirection, animator.transform.localScale.y, animator.transform.localScale.z);

        m_inputManager.OnModelInstantiated();
    }

    public void ChangeCurrentState(State playerNewState)
    {
        m_currentState = playerNewState;
        if (playerNewState.model != null)
        {
            Animator animator = playerNewState.model.GetComponent<Animator>();
            if (animator != null)
            {
                StartCoroutine(SpawnNewModel(animator));
            }
            else
            {
                Debug.LogWarning("The model does not have an Animator component");
            }
        }
        else
        {
            Debug.LogWarning("The model is null");
        }
        EventManager.Instance.RaisePlayerStateChanged(playerNewState);
    }

    public void EquipAbility(Skill newSkill, string abilityInput)
    {
        if (newSkill.Slot != SkillSlot.Ability)
        {
            Debug.LogWarning($"{newSkill.name} no es una habilidad Ability.");
            return;
        }

        m_runtimeState.skills.RemoveAll(s =>
        {
            if (s is InputSkill input)
                return input.ActionName == abilityInput;
            return false;
        });

        Skill runtimeCopy = ScriptableObject.Instantiate(newSkill);

        if (runtimeCopy is InputSkill inputCopy)
            inputCopy.OverrideActionName(abilityInput);

        m_runtimeState.skills.Add(runtimeCopy);

        EventManager.Instance.RaisePlayerStateChanged(m_runtimeState);
    }
}
