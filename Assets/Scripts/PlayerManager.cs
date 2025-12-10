using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private State m_baseSkill;
    [SerializeField] private GroundDetector m_groundDetector;
    [SerializeField] private Health m_health;
    [SerializeField] private PlayerInputManager m_inputManager;
    [SerializeField] private ParticleSystem m_spyral;
    [SerializeField] private ParticleSystem m_spawnParticle;

    private State m_playerCurrentState;
    private PlayerAnimatiorController m_playerModel;
    private List<State> m_skill = new();


    public State PlayerCurrentState { get => m_playerCurrentState; }
    public GroundDetector GroundDetector { get => m_groundDetector; }
    public Health Health { get => m_health; }
    public PlayerInputManager InputManager { get => m_inputManager; }
    public PlayerAnimatiorController AnimatorController { get => m_playerModel; }

    private void Awake()
    {

        m_health.OTakeDamage += PlayerTakesDamage;
        m_health.Dead += PlayerDead;
        m_inputManager.OnChangeState += ChangeSkill;

    }
    private void Start()
    {
        ChangeCurrentState(m_baseSkill);
        EventManager.Instance.RaisePlayerHealthUpdated(m_health.CurrentHealth);
        EventManager.Instance.PlayerGainedNewSkill += AddSkill;
    }
    private void PlayerDead()
    {
        EventManager.Instance.RaisePlayerDied();
    }

    private void PlayerTakesDamage(int currentHealth)
    {
        EventManager.Instance.RaisePlayerHealthUpdated(currentHealth);
        EventManager.Instance.RaisePlayerTookDamage();
    }
    private void AddSkill(State newSkill)
    {
        m_skill.Add(newSkill);
    }

    public void ChangeSkill(int i)
    {
        if (m_skill.Count - 1 >= i)
        {
            if (m_playerCurrentState == m_skill[i])
            {
                ChangeCurrentState(m_baseSkill);
            }
            else if (m_skill.Count != 0)
            {
                ChangeCurrentState(m_skill[Math.Clamp(i, 0, m_skill.Count - 1)]);
            }
        }
    }
    public void ChangeCurrentState(State playerNewState)
    {
        m_playerCurrentState = playerNewState;
        StartCoroutine(SpawnNewModel(playerNewState.model));
        EventManager.Instance.RaisePlayerStateChanged(playerNewState);
    }
    public IEnumerator SpawnNewModel(PlayerAnimatiorController player)
    {
        if (m_spyral!= null)
            m_spyral.Play();
        else
            Debug.LogWarning("No spiral particle assigned to PlayerManager");

        yield return new WaitForSeconds(.5f);

        if (m_spawnParticle != null)
            m_spawnParticle.Play();
        else
            Debug.LogWarning("No spawn particle assigned to PlayerManager");

        if (m_playerModel != null)
            Destroy(m_playerModel.gameObject);
        m_playerModel = Instantiate(player, transform);
        m_playerModel.Initialize(this);
    }
}
