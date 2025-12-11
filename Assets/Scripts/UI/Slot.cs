using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Item StoredItem { get; protected set; }
    public bool HasItem => StoredItem != null;

    public virtual void SetItem(Item item)
    {
        if (item == null) return;

        StoredItem = item;
        item.CurrentSlot = this;

        item.transform.SetParent(transform);
        var rt = item.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
    }

    public virtual void ClearItem()
    {
        if (StoredItem != null)
        {
            if (StoredItem.CurrentSlot == this)
                StoredItem.CurrentSlot = null;

            StoredItem = null;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<Item>();
        if (draggedItem == null) return;

        Slot previousSlot = draggedItem.CurrentSlot;

        // Viene de fuera de cualquier slot (por ejemplo, creado al vuelo)
        if (previousSlot == null)
        {
            if (HasItem)
            {
                Debug.Log("No se puede poner un item externo sobre un slot ocupado.");
                return;
            }

            SetItem(draggedItem);
            return;
        }

        // Mismo slot → solo recolocamos
        if (previousSlot == this)
        {
            SetItem(draggedItem);
            return;
        }

        // Hay algo en este slot → swap
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
