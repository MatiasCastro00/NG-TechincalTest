using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerControl m_playerControl;

    private Dictionary<string, List<IInputSkillComponent>> m_inputMap;
    private IMove m_mover = null;

    private PlayerManager m_playerManager;

    private Vector2 m_moveInput;

      
    private void OnEnable()
    {
        if (m_playerControl != null) m_playerControl.Enable();
    }

    private void OnDisable()
    {
        if (m_playerControl != null) m_playerControl.Disable();
    }

    private void Awake()
    {
        m_playerControl = new PlayerControl();
        m_inputMap = new Dictionary<string, List<IInputSkillComponent>>();
        Initialize();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.PlayerStateChanged -= OnPlayerStateChanged;

        m_playerControl.Player.Jump.performed -= OnJump;
        m_playerControl.Player.Sprint.performed -= OnDash;
        m_playerControl.Player.Movement.performed -= OnMove;
        m_playerControl.Player.Movement.canceled -= OnMove;

        m_playerControl.Player.Hability1.performed -= OnHability1;
        m_playerControl.Player.Hability2.performed -= OnHability2;
    }

    public void Initialize()
    {
        m_playerManager = GetComponent<PlayerManager>();

        EventManager.Instance.PlayerStateChanged += OnPlayerStateChanged;

        m_playerControl.Player.Jump.performed += OnJump;
        m_playerControl.Player.Sprint.performed += OnDash;
        m_playerControl.Player.Movement.performed += OnMove;
        m_playerControl.Player.Movement.canceled += OnMove;

        m_playerControl.Player.Hability1.performed += OnHability1;
        m_playerControl.Player.Hability2.performed += OnHability2;

        m_playerControl.Enable();
    }
    private void Update()
    {
        if (m_mover != null)
        {
            Vector3 dir = new Vector3(m_moveInput.x, m_moveInput.y, 0f);
            bool isPressed = dir.magnitude > 0.01f;

            m_mover.Move(dir, isPressed);
        }
    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        m_moveInput = ctx.ReadValue<Vector2>();
    }
    public void OnModelInstantiated()
    {
        
    }

    private void OnPlayerStateChanged(State newState)
    {
        ClearAbilities();
        SetupSkills(newState.skills);
    }

    private void SetupSkills(List<Skill> skills)
    {
        foreach (var skill in skills)
        {
            var component = skill.InstantiateComponent(gameObject);

            if (component is IInputSkillComponent inputSkill)
            {
                string actionName = ((InputSkill)skill).ActionName;
                if (!m_inputMap.ContainsKey(actionName))
                    m_inputMap[actionName] = new List<IInputSkillComponent>();

                m_inputMap[actionName].Add(inputSkill);
            }

            if (component is IMove mover)
                m_mover = mover;
        }
    }

    private void ClearAbilities()
    {
        foreach (var kvp in m_inputMap)
        {
            foreach (var comp in kvp.Value)
                comp.OnRemove();
        }

        m_inputMap.Clear();

        m_mover = null;

        var components = GetComponents<MonoBehaviour>();
        foreach (var comp in components)
        {
            if (comp is IInputSkillComponent || comp is IMove)
                Destroy(comp as Component);
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        ExecuteInput("Jump");
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        ExecuteInput("Dash");
    }

    private void OnHability1(InputAction.CallbackContext ctx)
    {
        ExecuteInput("Hability1");
    }

    private void OnHability2(InputAction.CallbackContext ctx)
    {
        ExecuteInput("Hability2");
    }

    private void ExecuteInput(string actionName)
    {
        if (!m_inputMap.ContainsKey(actionName)) return;

        foreach (var skill in m_inputMap[actionName])
            skill.OnInputPerformed();
    }

}
