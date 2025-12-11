using UnityEngine;
using UnityEngine.Events;

public class NpcDialogue : MonoBehaviour
{
    [SerializeField] Unity.Cinemachine.CinemachineCamera m_dialogueCamera;
    [SerializeField] Interactable m_interactable;
    [SerializeField] string m_npcName;
    [SerializeField] string[] m_dialoguePages;
    [SerializeField] UnityEvent m_onDialogueFinishedGiveReward;

    bool m_inConversation;

    void Awake()
    {
        if (m_interactable == null)
            m_interactable = GetComponentInChildren<Interactable>();

        if (m_interactable != null)
            m_interactable.OnInteract += OnInteract;

        if (m_dialogueCamera != null)
            m_dialogueCamera.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.DialogueFinished += OnDialogueFinished;
    }

    void OnDisable()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.DialogueFinished -= OnDialogueFinished;

        if (m_interactable != null)
            m_interactable.OnInteract -= OnInteract;
    }

    void OnInteract()
    {
        if (m_inConversation)
            return;

        if (InventoryMenu.Instance != null)
            InventoryMenu.Instance.Close();

        m_inConversation = true;

        if (m_dialogueCamera != null)
            m_dialogueCamera.gameObject.SetActive(true);

        DialogueManager.Instance.StartDialogue(m_dialoguePages, m_npcName);
    }

    void OnDialogueFinished()
    {
        if (!m_inConversation)
            return;

        m_inConversation = false;

        if (m_dialogueCamera != null)
            m_dialogueCamera.gameObject.SetActive(false);

        m_onDialogueFinishedGiveReward?.Invoke();
    }
}
