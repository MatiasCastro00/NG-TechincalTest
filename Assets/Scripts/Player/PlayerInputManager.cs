using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerManager m_playerManager;
    [SerializeField] float m_stepInterval = 0.4f;

    private PlayerControl m_playerControl;
    private Vector3 m_direction;

    private Dictionary<string, List<IInputSkillComponent>> m_inputMap = new();
    private List<ISkillComponent> m_activeSkillComponents = new();
    private IMove m_mover;

    public PlayerControl PlayerControl => m_playerControl;
    public Vector3 Direction => m_direction;




    private void Start()
    {
        if (m_playerManager == null)
            m_playerManager = GetComponent<PlayerManager>();

        m_playerControl = new PlayerControl();
        m_playerControl.Player.Enable();

        m_playerControl.Player.Jump.started += OnJumpStarted;
        m_playerControl.Player.Jump.performed += OnJumpPerformed;

        m_playerControl.Player.Sprint.performed += ctx => ExecuteInput("Dash");

        m_playerControl.Player.Hability1.performed += OnHability1;
        m_playerControl.Player.Hability2.performed += OnHability2;

        if (m_playerControl.Player.OpenInventory != null)
            m_playerControl.Player.OpenInventory.performed += OnOpenInventory;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.PlayerStateChanged += OnPlayerStateChanged;
            EventManager.Instance.PlayerEquipmentChanged += OnPlayerEquipmentChanged;
        }
        
    }

    private void OnDestroy()
    {
        if (m_playerControl != null)
        {
            m_playerControl.Player.Jump.started -= OnJumpStarted;
            m_playerControl.Player.Jump.performed -= OnJumpPerformed;
            m_playerControl.Player.Hability1.performed -= OnHability1;
            m_playerControl.Player.Hability2.performed -= OnHability2;

            if (m_playerControl.Player.OpenInventory != null)
                m_playerControl.Player.OpenInventory.performed -= OnOpenInventory;
        }

        if (EventManager.Instance != null)
        {
            EventManager.Instance.PlayerStateChanged -= OnPlayerStateChanged;
            EventManager.Instance.PlayerEquipmentChanged -= OnPlayerEquipmentChanged;
        }
    }

    private void Update()
    {
        if (m_playerControl == null)
            return;

        if (m_playerControl.Player.enabled && m_mover != null)
        {
            Vector2 input = m_playerControl.Player.Movement.ReadValue<Vector2>();
            m_direction = new Vector3(input.x, 0, input.y);

            m_mover.Move(m_direction, IsSprinting());

        }
        else
        {
            m_direction = Vector3.zero;
        }

    }
    

    public bool IsSprinting()
    {
        return m_playerControl != null &&
               m_playerControl.Player.Sprint.phase == InputActionPhase.Performed;
    }

    private void ExecuteInput(string inputName)
    {
        if (m_inputMap.TryGetValue(inputName, out var skills))
        {
            foreach (var skill in skills)
                skill.OnInputPerformed();
        }
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
        m_playerManager?.ChangeSkill(0);
    }

    private void OnHability2(InputAction.CallbackContext ctx)
    {
        m_playerManager?.ChangeSkill(1);
    }

    private void OnOpenInventory(InputAction.CallbackContext ctx)
    {
        if (InventoryMenu.Instance != null)
            InventoryMenu.Instance.Toggle();
    }

    private void OnPlayerStateChanged(State newState)
    {
        if (newState == null)
            return;
        Debug.Log("PlayerInputManager received equipment changed.");
        RebuildAbilitiesFromSkills(newState.skills);
    }

    private void OnPlayerEquipmentChanged(List<Skill> equippedSkills)
    {
        RebuildAbilitiesFromSkills(equippedSkills);
    }

    private void RebuildAbilitiesFromSkills(IEnumerable<Skill> skills)
    {
        ClearAbilities();

        if (skills == null)
            return;

        foreach (Skill skill in skills)
        {
            if (skill == null)
                continue;

            ISkillComponent instance = skill.InstantiateComponent(gameObject);

            if (instance == null)
                continue;

            m_activeSkillComponents.Add(instance);

            if (instance is IInputSkillComponent inputSkill && skill is InputSkill inputSkillSO)
            {
                if (!m_inputMap.TryGetValue(inputSkillSO.ActionName, out var list))
                {
                    list = new List<IInputSkillComponent>();
                    m_inputMap[inputSkillSO.ActionName] = list;
                }

                list.Add(inputSkill);
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
                Destroy(comp);
        }

        m_activeSkillComponents.Clear();
        m_inputMap.Clear();
        m_mover = null;
    }

    public void OnModelInstantiated(PlayerAnimatiorController controller)
    {
    }

    public void SetInputEnabled(bool enabled)
    {
        if (m_playerControl == null)
            return;

        if (enabled)
            m_playerControl.Player.Enable();
        else
            m_playerControl.Player.Disable();

        if (!enabled)
            m_direction = Vector3.zero;
    }
}
