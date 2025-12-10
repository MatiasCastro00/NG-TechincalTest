using UnityEngine;

public class PlayerAnimatiorController : MonoBehaviour
{
    [SerializeField] Animator m_animator;
    PlayerManager m_playerManager;

    public void Initialize(PlayerManager playerManager)
    {
        m_playerManager = playerManager;
    }
    private void Update()
    {
        /*
        if (m_playerManager == null) return;
        Vector3 direction = m_playerManager.InputManager.Direction;
        bool isGrounded = m_playerManager.GroundDetector.IsGrounded();
        bool isMoving = direction.x != 0;
        float speed = m_playerManager.InputManager.IsSprinting() ? 1f : 0f;
        float verticalVelocity = m_playerManager.GetComponent<Rigidbody>()?.linearVelocity.y ?? 0f;
        bool isFalling = !isGrounded && verticalVelocity < 0f;

        m_animator.SetBool("IsGrounded", isGrounded);
        m_animator.SetBool("IsMoving", isMoving);
        m_animator.SetBool("IsFalling", isFalling);

        m_animator.SetFloat("Speed", speed);

        if (direction.x != 0)
        {
            float yRotation = direction.x > 0 ? -90f : 90f;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        */
    }

    public void Jump()
    {
        m_animator.SetTrigger("IsJumping");
    }
    public void Dash()
    {
        m_animator.SetTrigger("IsDashing");
    }
}
