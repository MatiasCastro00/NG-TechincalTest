using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Interactable : MonoBehaviour
{
    [SerializeField] float m_radius = 2f;
    [SerializeField] LayerMask m_playerLayer;
    [SerializeField] InputActionReference m_interactAction;
    [SerializeField] Canvas m_worldCanvas;
    [SerializeField] TextMeshProUGUI m_promptLabel;
    [SerializeField] string m_actionDescription = "Interact";
    [SerializeField] bool m_oneShot = false;

    public Action OnInteract;

    bool m_playerInside;
    bool m_hasInteracted;

    void OnEnable()
    {
        if (m_interactAction != null && m_interactAction.action != null)
        {
            m_interactAction.action.performed += OnInteractPerformed;
            if (!m_interactAction.action.enabled)
                m_interactAction.action.Enable();
        }

        UpdatePromptText();

        if (m_worldCanvas != null)
            m_worldCanvas.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (m_interactAction != null && m_interactAction.action != null)
            m_interactAction.action.performed -= OnInteractPerformed;
    }

    void Update()
    {
        if (m_oneShot && m_hasInteracted)
            return;

        bool wasInside = m_playerInside;
        m_playerInside = Physics.CheckSphere(transform.position, m_radius, m_playerLayer);

        if (m_worldCanvas != null && wasInside != m_playerInside)
            m_worldCanvas.gameObject.SetActive(m_playerInside);

        if (m_worldCanvas != null)
            m_worldCanvas.transform.position = transform.position + Vector3.up * 2f;
    }

    void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (!m_playerInside)
            return;

        if (m_oneShot && m_hasInteracted)
            return;

        m_hasInteracted = true;
        OnInteract?.Invoke();

        if (m_oneShot && m_worldCanvas != null)
            m_worldCanvas.gameObject.SetActive(false);
    }

    void UpdatePromptText()
    {
        if (m_promptLabel == null || m_interactAction == null || m_interactAction.action == null)
            return;

        string binding = m_interactAction.action.GetBindingDisplayString();
        m_promptLabel.text = binding + " - " + m_actionDescription;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, m_radius);

        if (m_interactAction != null && m_interactAction.action != null)
        {
            string binding = m_interactAction.action.GetBindingDisplayString();
            Vector3 worldPos = transform.position + Vector3.up * 2f;
            Handles.Label(worldPos, binding + " - " + m_actionDescription);
        }
    }
#endif
}
