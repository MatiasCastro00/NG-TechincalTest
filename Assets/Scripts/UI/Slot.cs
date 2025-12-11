using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Item StoredItem { get; protected set; }
    public bool HasItem => StoredItem != null;

    public event Action<Slot> OnSlotChanged;

    public virtual void SetItem(Item item)
    {
        if (item == null) return;

        StoredItem = item;
        item.CurrentSlot = this;

        item.transform.SetParent(transform);
        RectTransform rect = item.transform as RectTransform;
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
        }

        OnSlotChanged?.Invoke(this);
    }

    public virtual void ClearItem()
    {
        if (StoredItem != null)
        {
            if (StoredItem.CurrentSlot == this)
                StoredItem.CurrentSlot = null;

            StoredItem = null;
            OnSlotChanged?.Invoke(this);
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        Item draggedItem = eventData.pointerDrag?.GetComponent<Item>();
        if (draggedItem == null) return;

        Slot previousSlot = draggedItem.CurrentSlot;

        if (previousSlot == this)
            return;

        if (previousSlot == null)
        {
            if (!HasItem)
            {
                SetItem(draggedItem);
            }
            return;
        }

        if (HasItem)
        {
            Item otherItem = StoredItem;
            previousSlot.SetItem(otherItem);
        }
        else
        {
            previousSlot.ClearItem();
        }

        SetItem(draggedItem);
    }
}
