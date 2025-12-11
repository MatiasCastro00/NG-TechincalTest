using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    [SerializeField] int m_maxSlots = 18;
    [SerializeField] Slot m_slotPrefab;
    [SerializeField] GameObject m_inventoryPanel;
    [SerializeField] private List<Slot> m_slots= new();
    [SerializeField] ItemDescription m_itemDescription;
    [SerializeField] Item m_item;
    [SerializeField] Canvas m_canvas;
    [SerializeField] Skill skilltest;
    [SerializeField] Skill skilltest2;

    public ItemDescription ItemDescription { get => m_itemDescription; }

    private void Start()
    {
        for (int i = 0; i < m_maxSlots; i++)
        {
            Slot slot = Instantiate(m_slotPrefab, m_inventoryPanel.transform);
            m_slots.Add(slot);
        }
        CreateItem(skilltest);
        CreateItem(skilltest2);
        
    }

    public void CreateItem(Skill skill)
    {
        Item itemTest;
        itemTest = Instantiate(m_item);
        itemTest.Skill = skill;
        itemTest.InventoryMenu = this;
        itemTest.Canvas = m_canvas;
        itemTest.image.sprite = skill.SkillIcon;
        for (int i = 0; i < m_maxSlots; i++)
        {
            if (!m_slots[i].HasItem)
            {
                m_slots[i].SetItem(Instantiate(itemTest));
                return;
            }
        }
        Debug.LogError("full inventory");
            
    }
}
