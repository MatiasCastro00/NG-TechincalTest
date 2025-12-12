using System;
using UnityEngine;

public class StepSoundController : MonoBehaviour
{
    PlayerAnimatiorController m_playerAnimatiorController;

    public Action<int> OnChangeState;
    float m_stepTimer;
    System.Random m_rng = new System.Random();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer==LayerMask.NameToLayer("Ground"))
        {
            PlayRandomStep();
        }
    }

    public void PlayRandomStep()
    {
        int r = m_rng.Next(0, 3);

        if (r == 0)
            EventManager.Instance?.RaisePlayerStep();
        else if (r == 1)
            EventManager.Instance?.RaisePlayerStep2();
        else
            EventManager.Instance?.RaisePlayerStep3();
    }

}
