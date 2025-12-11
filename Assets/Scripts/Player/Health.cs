using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int m_maxhHealth;
    [SerializeField] GroundDetector m_groundDetector;
    [SerializeField] private float m_invulnerabilityTime = 1.0f;

    private int m_currentHealth;
    private bool m_canTakeDamage = true;

    public bool IsDead => m_currentHealth == 0;

    public GroundDetector GroundDetector { get => m_groundDetector; }
    public int CurrentHealth { get => m_currentHealth; }

    public Action Dead;
    public Action<int> OTakeDamage;

    private void Awake()
    {
        m_currentHealth = m_maxhHealth;
    }

    public bool TakeDamage(int damage)
    {
        if (m_canTakeDamage)
        {
            m_currentHealth -= damage;
            OTakeDamage?.Invoke(m_currentHealth);
            StartCoroutine(InvulnerabilityCoroutine());
        }
        if (IsDead)
            Dead?.Invoke();
        return IsDead;
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        m_canTakeDamage = false;
        yield return new WaitForSeconds(m_invulnerabilityTime);
        m_canTakeDamage = true;
    }
}
