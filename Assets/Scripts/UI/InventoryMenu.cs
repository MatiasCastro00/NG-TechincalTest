using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryMenu : NullableSingleton<InventoryMenu>
{
    [SerializeField] private int m_maxSlots = 18;
    [SerializeField] private Slot m_slotPrefab;
    [SerializeField] private List<EquipmentSlot> m_equipmentSlots;
    [SerializeField] private List<Slot> m_slots = new();
    [SerializeField] private GameObject m_inventoryPanel;
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private ItemDescription m_itemDescription;
    [SerializeField] private Item m_item;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private float m_fadeDuration = 0.25f;

    private bool m_isOpen;
    private Coroutine m_fadeRoutine;
    private State m_currentState;
    private bool m_isLoading;

    public ItemDescription ItemDescription => m_itemDescription;
    public bool IsOpen => m_isOpen;

    protected override void Awake()
    {
        base.Awake();

        if (m_equipmentSlots != null)
        {
            foreach (EquipmentSlot eq in m_equipmentSlots)
            {
                if (eq != null)
                    eq.OnSlotChanged += OnEquipmentSlotChanged;
            }
        }

        if (m_canvasGroup != null)
        {
            m_canvasGroup.alpha = 0f;
            m_canvasGroup.interactable = false;
            m_canvasGroup.blocksRaycasts = false;
            m_isOpen = false;
        }
    }

    private void OnDestroy()
    {
        if (m_equipmentSlots != null)
        {
            foreach (EquipmentSlot eq in m_equipmentSlots)
            {
                if (eq != null)
                    eq.OnSlotChanged -= OnEquipmentSlotChanged;
            }
        }

        if (m_slots != null)
        {
            foreach (Slot slot in m_slots)
            {
                if (slot != null)
                    slot.OnSlotChanged -= OnInventorySlotChanged;
            }
        }
    }

    public void Initialize(State playerBaseState)
    {
        m_currentState = playerBaseState;

        if (m_slots == null)
            m_slots = new List<Slot>();

        if (m_slots.Count == 0 && m_slotPrefab != null && m_inventoryPanel != null)
        {
            for (int i = 0; i < m_maxSlots; i++)
            {
                Slot slot = Instantiate(m_slotPrefab, m_inventoryPanel.transform);
                m_slots.Add(slot);
                slot.OnSlotChanged += OnInventorySlotChanged;
            }
        }
        else
        {
            foreach (Slot slot in m_slots)
            {
                if (slot != null)
                {
                    slot.OnSlotChanged -= OnInventorySlotChanged;
                    slot.OnSlotChanged += OnInventorySlotChanged;
                }
            }
        }

        m_isLoading = true;

        ClearAllSlots();

        bool hasSavedInventory = playerBaseState.savedInventorySlots != null &&
                                 playerBaseState.savedInventorySlots.Count == m_maxSlots;
        bool hasSavedEquipment = playerBaseState.savedEquipmentSlots != null &&
                                 playerBaseState.savedEquipmentSlots.Count == m_equipmentSlots.Count;

        if (hasSavedInventory || hasSavedEquipment)
        {
            LoadFromState(playerBaseState);
        }
        else
        {
            foreach (Skill newSkill in playerBaseState.skills)
            {
                if (newSkill == null) continue;

                Item item = CreateItem(newSkill);
                if (item == null) continue;

                bool equipped = false;

                if (m_equipmentSlots != null)
                {
                    foreach (EquipmentSlot equipmentSlot in m_equipmentSlots)
                    {
                        if (equipmentSlot != null &&
                            equipmentSlot.RequiredSlotType == newSkill.Slot &&
                            !equipmentSlot.HasItem)
                        {
                            equipmentSlot.SetItem(item);
                            equipped = true;
                            break;
                        }
                    }
                }

                if (!equipped)
                {
                    int idx = FoundNextFreeSlot();
                    if (idx != -1) m_slots[idx].SetItem(item);
                    else Destroy(item.gameObject);
                }
            }
        }

        m_isLoading = false;

        StartCoroutine(DelayedRaiseEquipmentAndSave());
    }

    private IEnumerator DelayedRaiseEquipmentAndSave()
    {
        yield return null;
        RaiseEquipmentChangedFromCurrentSlots();
        SaveInventoryToState();
    }

    private void LoadFromState(State state)
    {
        if (state.savedEquipmentSlots != null && m_equipmentSlots != null)
        {
            int count = Mathf.Min(state.savedEquipmentSlots.Count, m_equipmentSlots.Count);
            for (int i = 0; i < count; i++)
            {
                Skill skill = state.savedEquipmentSlots[i];
                if (skill == null) continue;
                EquipmentSlot slot = m_equipmentSlots[i];
                if (slot == null) continue;

                Item item = CreateItem(skill);
                if (item == null) continue;
                slot.SetItem(item);
            }
        }

        if (state.savedInventorySlots != null && m_slots != null)
        {
            int count = Mathf.Min(state.savedInventorySlots.Count, m_slots.Count);
            for (int i = 0; i < count; i++)
            {
                Skill skill = state.savedInventorySlots[i];
                if (skill == null) continue;
                Slot slot = m_slots[i];
                if (slot == null) continue;

                Item item = CreateItem(skill);
                if (item == null) continue;
                slot.SetItem(item);
            }
        }
    }

    public void AddNewItem(Skill skill)
    {
        if (skill == null) return;
        Item item = CreateItem(skill);
        if (item == null) return;
        int idx = FoundNextFreeSlot();
        if (idx != -1)
        {
            m_slots[idx].SetItem(item);
        }
        else
        {
            Destroy(item.gameObject);
            Debug.LogWarning("Inventory is full! Cannot add new item: " + skill.name);
        }
    }

    public Item CreateItem(Skill skill)
    {
        if (m_item == null || m_inventoryPanel == null) return null;

        Item item = Instantiate(m_item, m_inventoryPanel.transform);
        item.Skill = skill;
        item.InventoryMenu = this;
        item.Canvas = m_canvas;

        if (skill != null)
            item.image.sprite = skill.SkillIcon;

        return item;
    }

    private int FoundNextFreeSlot()
    {
        for (int i = 0; i < m_slots.Count; i++)
            if (!m_slots[i].HasItem)
                return i;

        return -1;
    }

    private void ClearAllSlots()
    {
        if (m_equipmentSlots != null)
        {
            foreach (EquipmentSlot eq in m_equipmentSlots)
                if (eq != null && eq.HasItem) eq.ClearItem();
        }

        if (m_slots != null)
        {
            foreach (Slot slot in m_slots)
                if (slot != null && slot.HasItem) slot.ClearItem();
        }
    }

    private void OnEquipmentSlotChanged(Slot slot)
    {
        if (m_isLoading) return;
        RaiseEquipmentChangedFromCurrentSlots();
        SaveInventoryToState();
    }

    private void OnInventorySlotChanged(Slot slot)
    {
        if (m_isLoading) return;
        SaveInventoryToState();
    }

    private void RaiseEquipmentChangedFromCurrentSlots()
    {
        if (EventManager.Instance == null) return;

        List<Skill> equippedSkills = new List<Skill>();

        if (m_equipmentSlots != null)
        {
            foreach (EquipmentSlot eq in m_equipmentSlots)
                if (eq != null && eq.HasItem && eq.StoredItem.Skill != null)
                    equippedSkills.Add(eq.StoredItem.Skill);
        }

        EventManager.Instance.RaisePlayerEquipmentChanged(equippedSkills);
    }
    public void ResetInventoryData()
    {
        if (m_currentState == null)
        {
            Debug.LogWarning("InventoryMenu: Cannot reset inventory data because current state is null.");
            return;
        }

        m_isLoading = true;

        ClearAllSlots();

        if (m_currentState.savedInventorySlots == null)
            m_currentState.savedInventorySlots = new List<Skill>();
        else
            m_currentState.savedInventorySlots.Clear();

        if (m_currentState.savedEquipmentSlots == null)
            m_currentState.savedEquipmentSlots = new List<Skill>();
        else
            m_currentState.savedEquipmentSlots.Clear();

#if UNITY_EDITOR
        EditorUtility.SetDirty(m_currentState);
#endif

        RaiseEquipmentChangedFromCurrentSlots();

        m_isLoading = false;

        Debug.Log("InventoryMenu: Inventory save data cleared (lists set to size 0).");
    }


    private void EnsureSavedListsSize()
    {
        if (m_currentState.savedInventorySlots == null)
            m_currentState.savedInventorySlots = new List<Skill>();

        if (m_currentState.savedEquipmentSlots == null)
            m_currentState.savedEquipmentSlots = new List<Skill>();

        if (m_currentState.savedInventorySlots.Count != m_maxSlots)
        {
            m_currentState.savedInventorySlots.Clear();
            for (int i = 0; i < m_maxSlots; i++)
                m_currentState.savedInventorySlots.Add(null);
        }

        int equipmentCount = (m_equipmentSlots == null) ? 0 : m_equipmentSlots.Count;

        if (m_currentState.savedEquipmentSlots.Count != equipmentCount)
        {
            m_currentState.savedEquipmentSlots.Clear();
            for (int i = 0; i < equipmentCount; i++)
                m_currentState.savedEquipmentSlots.Add(null);
        }
    }

    private void SaveInventoryToState()
    {
        if (m_currentState == null) return;

        if (m_currentState.savedInventorySlots == null)
            m_currentState.savedInventorySlots = new List<Skill>();
        if (m_currentState.savedEquipmentSlots == null)
            m_currentState.savedEquipmentSlots = new List<Skill>();

        if (m_currentState.savedInventorySlots.Count != m_maxSlots)
        {
            m_currentState.savedInventorySlots.Clear();
            for (int i = 0; i < m_maxSlots; i++)
                m_currentState.savedInventorySlots.Add(null);
        }

        if (m_equipmentSlots == null)
        {
            m_currentState.savedEquipmentSlots.Clear();
        }
        else
        {
            if (m_currentState.savedEquipmentSlots.Count != m_equipmentSlots.Count)
            {
                m_currentState.savedEquipmentSlots.Clear();
                for (int i = 0; i < m_equipmentSlots.Count; i++)
                    m_currentState.savedEquipmentSlots.Add(null);
            }
        }

        for (int i = 0; i < m_slots.Count && i < m_currentState.savedInventorySlots.Count; i++)
        {
            Skill skill = null;
            if (m_slots[i] != null && m_slots[i].HasItem && m_slots[i].StoredItem.Skill != null)
                skill = m_slots[i].StoredItem.Skill;
            m_currentState.savedInventorySlots[i] = skill;
        }

        if (m_equipmentSlots != null)
        {
            for (int i = 0; i < m_equipmentSlots.Count && i < m_currentState.savedEquipmentSlots.Count; i++)
            {
                Skill skill = null;
                if (m_equipmentSlots[i] != null &&
                    m_equipmentSlots[i].HasItem &&
                    m_equipmentSlots[i].StoredItem.Skill != null)
                    skill = m_equipmentSlots[i].StoredItem.Skill;
                m_currentState.savedEquipmentSlots[i] = skill;
            }
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(m_currentState);
#endif
    }

    public void Open()
    {
        if (m_canvasGroup == null)
            return;

        if (m_fadeRoutine != null)
            StopCoroutine(m_fadeRoutine);

        m_fadeRoutine = StartCoroutine(FadeCanvasGroup(1f));
        m_isOpen = true;
    }

    public void Close()
    {
        if (m_canvasGroup == null)
            return;

        if (m_fadeRoutine != null)
            StopCoroutine(m_fadeRoutine);

        m_fadeRoutine = StartCoroutine(FadeCanvasGroup(0f));
        m_isOpen = false;
    }

    public void Toggle()
    {
        if (m_isOpen) Close();
        else Open();
    }

    private IEnumerator FadeCanvasGroup(float target)
    {
        float start = m_canvasGroup.alpha;
        float t = 0f;

        if (target > 0f)
        {
            m_canvasGroup.blocksRaycasts = true;
            m_canvasGroup.interactable = true;
        }

        while (t < m_fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, target, t / m_fadeDuration);
            m_canvasGroup.alpha = a;
            yield return null;
        }

        m_canvasGroup.alpha = target;

        if (target == 0f)
        {
            m_canvasGroup.blocksRaycasts = false;
            m_canvasGroup.interactable = false;
        }
    }
}
