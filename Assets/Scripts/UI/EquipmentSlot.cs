using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : Slot
{
    [SerializeField] private SkillSlot m_requiredSlotType;

    public SkillSlot RequiredSlotType => m_requiredSlotType;

    public override void OnDrop(PointerEventData eventData)
    {
        Item draggedItem = eventData.pointerDrag?.GetComponent<Item>();
        if (draggedItem == null) return;
        if (draggedItem.Skill == null) return;
        if (draggedItem.Skill.Slot != m_requiredSlotType) return;

        base.OnDrop(eventData);
    }
}
