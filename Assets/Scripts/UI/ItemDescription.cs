using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text m_stat;
    [SerializeField] TMPro.TMP_Text m_description;
    [SerializeField] GameObject m_statsPivot;
    [SerializeField] GameObject m_descriptionPanel;
    [SerializeField] Animator m_animator;

    public void Show(string description, IEnumerable<string> stats)
    {
        CloseWindow();
        StopAllCoroutines();
        m_descriptionPanel.SetActive(true);
        m_description.text = description;

        // Crear stats
        foreach (string statText in stats)
        {
            var stat = Instantiate(m_stat, m_statsPivot.transform);
            stat.text = statText;
        }
    }

    public void HidePanel()
    {
        StopAllCoroutines();
        StartCoroutine(HidePanelCoroutine());
    }

    private IEnumerator HidePanelCoroutine()
    {
        m_animator.SetTrigger("Hide");
        yield return new WaitForSeconds(0.3f);
        CloseWindow();
    }
    private void CloseWindow()
    {
        m_descriptionPanel.SetActive(false);

        // Limpiar stats viejos
        foreach (Transform child in m_statsPivot.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
