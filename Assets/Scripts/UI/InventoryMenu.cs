using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool m_isOpen = false;
    private Coroutine m_fadeRoutine;

    public ItemDescription ItemDescription => m_itemDescription;
    public bool IsOpen => m_isOpen;

    protected override void Awake()
    {
        base.Awake();

        foreach (var eq in m_equipmentSlots)
            if (eq != null) eq.OnSlotChanged += OnEquipmentSlotChanged;

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
        foreach (var eq in m_equipmentSlots)
            if (eq != null) eq.OnSlotChanged -= OnEquipmentSlotChanged;
    }

    public void Initialize(State playerBaseState)
    {
        if (m_slots.Count == 0 && m_slotPrefab != null && m_inventoryPanel != null)
        {
            for (int i = 0; i < m_maxSlots; i++)
            {
                Slot slot = Instantiate(m_slotPrefab, m_inventoryPanel.transform);
                m_slots.Add(slot);
            }
        }

        ClearAllSlots();

        foreach (Skill newSkill in playerBaseState.skills)
        {
            if (newSkill == null) continue;

            Item item = CreateItem(newSkill);
            if (item == null) continue;

            bool equipped = false;

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

            if (!equipped)
            {
                int idx = FoundNextFreeSlot();
                if (idx != -1) m_slots[idx].SetItem(item);
                else Destroy(item.gameObject);
            }
        }

        RaiseEquipmentChangedFromCurrentSlots();
    }

    public Item CreateItem(Skill skill)
    {
        if (m_item == null) return null;

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
        foreach (var eq in m_equipmentSlots)
            if (eq != null && eq.HasItem) eq.ClearItem();

        foreach (var slot in m_slots)
            if (slot != null && slot.HasItem) slot.ClearItem();
    }

    private void OnEquipmentSlotChanged(EquipmentSlot slot)
    {
        RaiseEquipmentChangedFromCurrentSlots();
    }

    private void RaiseEquipmentChangedFromCurrentSlots()
    {
        if (EventManager.Instance == null) return;

        List<Skill> equippedSkills = new List<Skill>();

        foreach (var eq in m_equipmentSlots)
            if (eq != null && eq.HasItem && eq.StoredItem.Skill != null)
                equippedSkills.Add(eq.StoredItem.Skill);

        EventManager.Instance.RaisePlayerEquipmentChanged(equippedSkills);
    }

    public void Open()
    {
        if (m_fadeRoutine != null) StopCoroutine(m_fadeRoutine);
        m_fadeRoutine = StartCoroutine(FadeCanvasGroup(1f));
        m_isOpen = true;
    }

    public void Close()
    {
        if (m_fadeRoutine != null) StopCoroutine(m_fadeRoutine);
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
