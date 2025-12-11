using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : NullableSingleton<DialogueManager>
{
    [SerializeField] GameObject m_panel;
    [SerializeField] TextMeshProUGUI m_text;
    [SerializeField] TextMeshProUGUI m_nameText;
    [SerializeField] float m_wordDelay = 0.05f;
    [SerializeField] InputActionReference m_nextPageAction;
    [SerializeField] GameObject m_continueIcon;
    [SerializeField] TextMeshProUGUI m_continueText;
    [SerializeField] PlayerInputManager m_playerInput;

    string[] m_pages;
    int m_currentPage;
    bool m_isTyping;
    bool m_isActive;

    public Action DialogueFinished;

    protected override void Awake()
    {
        base.Awake();

        if (m_panel != null)
            m_panel.SetActive(false);

        if (m_continueIcon != null)
            m_continueIcon.SetActive(false);
    }

    void OnEnable()
    {
        if (m_nextPageAction != null)
        {
            m_nextPageAction.action.performed += OnNextPagePerformed;
            m_nextPageAction.action.Enable();
        }

        UpdateContinueHint();
    }

    void OnDisable()
    {
        if (m_nextPageAction != null)
        {
            m_nextPageAction.action.performed -= OnNextPagePerformed;
            m_nextPageAction.action.Disable();
        }
    }

    void OnNextPagePerformed(InputAction.CallbackContext ctx)
    {
        if (!m_isActive)
            return;

        HandleNextInput();
    }

    public void StartDialogue(string[] pages, string npcName)
    {
        if (pages == null || pages.Length == 0)
            return;

        m_pages = pages;
        m_currentPage = 0;
        m_isActive = true;
        m_isTyping = false;

        if (m_panel != null)
            m_panel.SetActive(true);

        if (m_nameText != null)
            m_nameText.text = npcName;

        if (m_playerInput != null)
            m_playerInput.SetInputEnabled(false);

        UpdateContinueHint();
        ShowContinueIcon(false);

        StopAllCoroutines();
        StartCoroutine(TypeCurrentPage());
    }

    IEnumerator TypeCurrentPage()
    {
        m_isTyping = true;

        if (m_text == null)
            yield break;

        ShowContinueIcon(false);

        m_text.text = "";
        string page = m_pages[m_currentPage];
        string[] words = page.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (!m_isTyping)
            {
                m_text.text = page;
                break;
            }

            if (i > 0)
                m_text.text += " ";

            m_text.text += words[i];
            yield return new WaitForSeconds(m_wordDelay);
        }

        m_isTyping = false;
        ShowContinueIcon(true);
    }

    void HandleNextInput()
    {
        if (m_isTyping)
        {
            m_isTyping = false;
            StopAllCoroutines();

            if (m_text != null)
                m_text.text = m_pages[m_currentPage];

            ShowContinueIcon(true);
            return;
        }

        m_currentPage++;

        if (m_currentPage >= m_pages.Length)
        {
            EndDialogue();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(TypeCurrentPage());
        }
    }

    void EndDialogue()
    {
        m_isActive = false;

        if (m_panel != null)
            m_panel.SetActive(false);

        if (m_playerInput != null)
            m_playerInput.SetInputEnabled(true);

        ShowContinueIcon(false);

        DialogueFinished?.Invoke();
    }

    void UpdateContinueHint()
    {
        if (m_continueText == null || m_nextPageAction == null || m_nextPageAction.action == null)
            return;

        string binding = m_nextPageAction.action.GetBindingDisplayString();
        m_continueText.text = binding;
    }

    void ShowContinueIcon(bool show)
    {
        if (m_continueIcon != null)
            m_continueIcon.SetActive(show);
    }
}
