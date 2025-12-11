using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : Slot
{
    /*
    [SerializeField] private SkillSlot m_requiredSlotType;

    public SkillSlot RequiredSlotType => m_requiredSlotType;

    public event Action<EquipmentSlot> OnSlotChanged;

    public override void SetItem(Item item)
    {
        base.SetItem(item);
        OnSlotChanged?.Invoke(this);
    }

    public override void ClearItem()
    {
        base.ClearItem();
        OnSlotChanged?.Invoke(this);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<Item>();
        if (draggedItem == null) return;
        if (draggedItem.Skill == null) return;

        if (draggedItem.Skill.Slot != m_requiredSlotType)
        {
            return;
        }

        base.OnDrop(eventData);
    }
    */
}
