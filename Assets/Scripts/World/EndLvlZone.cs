using UnityEngine;

public class EndLvlZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.RaiseEndLvl();
        }
    }
}
