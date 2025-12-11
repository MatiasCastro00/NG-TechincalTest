using System;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class MissionManager : MonoBehaviour
{
    [Serializable]
    public class MissionEntry
    {
        public Interactable interactable;
        public Light light;
        public TextMeshProUGUI label;
        public bool completed;
    }

    [SerializeField] private MissionEntry[] m_missions;
    [SerializeField] private Color m_completedColor = Color.green;

    public UnityEvent OnAllMissionsCompleted;

    void Start()
    {
        if (m_missions == null) return;

        for (int i = 0; i < m_missions.Length; i++)
        {
            int index = i;
            if (m_missions[i].interactable != null)
                m_missions[i].interactable.OnInteract += () => CompleteMission(index);
        }
    }

    public void CompleteMission(int index)
    {
        if (m_missions == null || index < 0 || index >= m_missions.Length)
            return;

        MissionEntry mission = m_missions[index];
        if (mission.completed)
            return;

        mission.completed = true;

        if (mission.light != null)
            mission.light.color = m_completedColor;

        if (mission.label != null)
            mission.label.color = m_completedColor;

        if (AllMissionsCompleted())
            OnAllMissionsCompleted?.Invoke();
    }

    private bool AllMissionsCompleted()
    {
        if (m_missions == null || m_missions.Length == 0)
            return false;

        foreach (var m in m_missions)
            if (!m.completed)
                return false;

        return true;
    }
}
