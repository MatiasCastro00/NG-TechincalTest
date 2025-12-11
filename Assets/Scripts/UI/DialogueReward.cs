using UnityEngine;

public class DialogueReward : MonoBehaviour
{
    [SerializeField] Skill m_item;

    public void GiveReward()
    {
        Debug.Log("Reward added: " + m_item.name);
        InventoryMenu.Instance.AddNewItem(m_item);
    }
}
