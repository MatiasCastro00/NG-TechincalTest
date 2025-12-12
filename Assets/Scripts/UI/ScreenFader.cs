using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup m_fadeCanvasGroup;
    [SerializeField] private float m_fadeInDuration = 0.35f;
    [SerializeField] private float m_fadeOutDuration = 0.35f;

    [Header("End Screen")]
    [SerializeField] private CanvasGroup m_endScreenGroup;
    [SerializeField] private Button m_restartButton;
    [SerializeField] private Button m_clearDataButton;


    private Coroutine m_routine;

    private void Awake()
    {
        if (m_endScreenGroup != null)
        {
            m_endScreenGroup.alpha = 0f;
            m_endScreenGroup.interactable = false;
            m_endScreenGroup.blocksRaycasts = false;
        }

        SetEndButtonsInteractable(false);
    }

    private void Start()
    {
        if (m_fadeCanvasGroup == null)
        {
            Debug.LogWarning("ScreenFader: Fade CanvasGroup is not assigned.");
            return;
        }

        m_fadeCanvasGroup.alpha = 1f;
        m_fadeCanvasGroup.blocksRaycasts = true;
        m_fadeCanvasGroup.interactable = true;

        FadeIn();
        EventManager.Instance.EndLvl += FadeOutAndShowEndScreen;
        m_restartButton.onClick.AddListener(RestartLevel);
        m_clearDataButton.onClick.AddListener(ClearData);
    }

    public void FadeIn()
    {
        StartFadeTo(0f, m_fadeInDuration, blockRaycasts: false);
    }

    public void FadeOut()
    {
        StartFadeTo(1f, m_fadeOutDuration, blockRaycasts: true);
    }

    public void FadeOutAndShowEndScreen()
    {
        StopRunningRoutine();
        m_routine = StartCoroutine(FadeOutAndShowEndScreenRoutine());
    }

    private IEnumerator FadeOutAndShowEndScreenRoutine()
    {
        yield return FadeToRoutine(1f, m_fadeOutDuration, blockRaycasts: true);

        if (m_endScreenGroup != null)
        {
            m_endScreenGroup.alpha = 1f;
            m_endScreenGroup.interactable = true;
            m_endScreenGroup.blocksRaycasts = true;
        }

        SetEndButtonsInteractable(true);

        yield return FadeToRoutine(0f, m_fadeInDuration, blockRaycasts: false);
    }

    public void FadeOutAndRestartLevel()
    {
        StopRunningRoutine();
        m_routine = StartCoroutine(FadeOutAndRestartRoutine());
    }

    private IEnumerator FadeOutAndRestartRoutine()
    {
        yield return FadeToRoutine(1f, m_fadeOutDuration, blockRaycasts: true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevel()
    {
        FadeOutAndRestartLevel();
    }

    public void ClearData()
    {
        try
        {
            InventoryMenu.Instance?.ResetInventoryData();
            Debug.Log("ScreenFader: Clear data requested.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"ScreenFader: ClearData failed. Exception: {ex}");
        }
    }

    private void StartFadeTo(float targetAlpha, float duration, bool blockRaycasts)
    {
        if (m_fadeCanvasGroup == null)
            return;

        StopRunningRoutine();
        m_routine = StartCoroutine(FadeToRoutine(targetAlpha, duration, blockRaycasts));
    }

    private IEnumerator FadeToRoutine(float targetAlpha, float duration, bool blockRaycasts)
    {
        if (m_fadeCanvasGroup == null)
            yield break;

        float start = m_fadeCanvasGroup.alpha;

        m_fadeCanvasGroup.blocksRaycasts = blockRaycasts;
        m_fadeCanvasGroup.interactable = blockRaycasts;

        if (duration <= 0f)
        {
            m_fadeCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            m_fadeCanvasGroup.alpha = Mathf.Lerp(start, targetAlpha, k);
            yield return null;
        }

        m_fadeCanvasGroup.alpha = targetAlpha;
    }

    private void SetEndButtonsInteractable(bool value)
    {
        if (m_restartButton != null) m_restartButton.interactable = value;
        if (m_clearDataButton != null) m_clearDataButton.interactable = value;
    }

    private void StopRunningRoutine()
    {
        if (m_routine != null)
        {
            StopCoroutine(m_routine);
            m_routine = null;
        }
    }
}
