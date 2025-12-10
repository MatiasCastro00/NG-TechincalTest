using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerManager m_playerManager;

    private PlayerControl m_playerControl;
    private Vector3 m_direction;


    private Dictionary<string, List<IInputSkillComponent>> m_inputMap = new();
    private List<ISkillComponent> m_activeSkillComponents = new();
    private IMove m_mover;

    public PlayerControl PlayerControl { get => m_playerControl; }
    public Vector3 Direction { get => m_direction; }

    public Action<int> OnChangeState;

    private void Start()
    {
        m_playerControl = new PlayerControl();
        m_playerControl.Player.Enable();

        m_playerControl.Player.Jump.performed += OnJumpPerformed;
        m_playerControl.Player.Jump.started += OnJumpStarted;

        m_playerControl.Player.Sprint.performed += ctx => ExecuteInput("Dash");

        m_playerControl.Player.Hability1.performed += OnHability1;
        m_playerControl.Player.Hability2.performed += OnHability2;


        EventManager.Instance.PlayerStateChanged += OnPlayerStateChanged;
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        m_playerManager?.AnimatorController?.Jump();
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        ExecuteInput("Jump");
    }
    private void OnHability1(InputAction.CallbackContext ctx)
    {
        ExecuteInput("Hability1");
    }

    private void OnHability2(InputAction.CallbackContext ctx)
    {
        ExecuteInput("Hability2");
    }

    private void OnDestroy()
    {
        if (m_playerControl != null)
        {
            m_playerControl.Player.Jump.started -= OnJumpStarted;
            m_playerControl.Player.Jump.performed -= OnJumpPerformed;
            m_playerControl.Player.Hability1.performed -= OnHability1;
            m_playerControl.Player.Hability2.performed -= OnHability2;
        }

        if (EventManager.Instance != null)
        {
            EventManager.Instance.PlayerStateChanged -= OnPlayerStateChanged;
        }
    }



    private void Update()
    {
        if (m_mover != null)
        {
            Vector2 input = m_playerControl.Player.Movement.ReadValue<Vector2>();
            m_direction = new Vector3(input.x, 0, input.y);
            m_mover.Move(m_direction, IsSprinting());
        }
    }

    public bool IsSprinting()
    {
        return m_playerControl.Player.Sprint.phase == InputActionPhase.Performed;
    }

    private void ExecuteInput(string inputName)
    {
        if (m_inputMap.TryGetValue(inputName, out var skills))
        {
            foreach (var skill in skills)
                skill.OnInputPerformed();
        }
    }

    private void OnPlayerStateChanged(State newState)
    {
        ClearAbilities();

        foreach (Skill skill in newState.skills)
        {
            ISkillComponent instance = skill.InstantiateComponent(gameObject);

            if (instance == null)
                continue;

            m_activeSkillComponents.Add(instance);

            if (instance is IInputSkillComponent inputSkill && skill is InputSkill inputSkillSO)
            {
                if (!m_inputMap.ContainsKey(inputSkillSO.ActionName))
                    m_inputMap[inputSkillSO.ActionName] = new List<IInputSkillComponent>();

                m_inputMap[inputSkillSO.ActionName].Add(inputSkill);
            }

            if (instance is IMove move)
                m_mover = move;
        }
    }

    private void ClearAbilities()
    {
        foreach (ISkillComponent skill in m_activeSkillComponents)
        {
            skill.OnRemove();
            if (skill is Component comp)
            {
                Destroy(comp);
            }
        }

        m_activeSkillComponents.Clear();
        m_inputMap.Clear();
        m_mover = null;
    }
}
