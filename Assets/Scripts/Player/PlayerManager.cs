using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Base State")]
    [SerializeField] private State m_baseSkill;

    [Header("Components")]
    [SerializeField] private GroundDetector m_groundDetector;
    [SerializeField] private Health m_health;
    [SerializeField] private PlayerInputManager m_inputManager;

    [Header("VFX")]
    [SerializeField] private ParticleSystem m_spyral;
    [SerializeField] private ParticleSystem m_spawnParticle;

    private State m_playerCurrentState;
    private PlayerAnimatiorController m_playerModel;

    public State BaseState => m_baseSkill;
    public State CurrentState => m_playerCurrentState;
    public GroundDetector GroundDetector => m_groundDetector;
    public Health Health => m_health;
    public PlayerInputManager InputManager => m_inputManager;
    public PlayerAnimatiorController AnimatorController => m_playerModel;

    private void Start()
    {
        if (m_inputManager == null)
            m_inputManager = GetComponent<PlayerInputManager>();

        if (m_health == null)
            m_health = GetComponent<Health>();

        if (m_baseSkill != null)
        {

            ChangeCurrentState(m_baseSkill);

            if (InventoryMenu.Instance != null)
            {
                InventoryMenu.Instance.Initialize(m_baseSkill);
            }
        }
        else
        {
            Debug.LogWarning("PlayerManager: no se asignó el State base (m_baseSkill).");
        }
    }

    public void ChangeCurrentState(State newState)
    {
        if (newState == null)
        {
            Debug.LogWarning("PlayerManager.ChangeCurrentState llamado con newState nulo.");
            return;
        }

        m_playerCurrentState = newState;

        if (m_spyral != null)
            m_spyral.Play();


        if (m_playerCurrentState.model != null)
        {
            StartCoroutine(SwitchModel(m_playerCurrentState.model));
        }

        if (EventManager.Instance != null)
        {
            EventManager.Instance.RaisePlayerStateChanged(m_playerCurrentState);
        }
    }


    public void ChangeSkill(int index)
    {

        Debug.Log($"PlayerManager.ChangeSkill({index}) llamado (aún sin lógica específica).");
    }


    private IEnumerator SwitchModel(PlayerAnimatiorController playerPrefab)
    {

        yield return new WaitForSeconds(0.5f);

        if (m_spawnParticle != null)
            m_spawnParticle.Play();
        else
            Debug.LogWarning("No spawn particle assigned to PlayerManager");

        if (m_playerModel != null)
            Destroy(m_playerModel.gameObject);

        m_playerModel = Instantiate(playerPrefab, transform);
        m_playerModel.Initialize(this);
    }
}
