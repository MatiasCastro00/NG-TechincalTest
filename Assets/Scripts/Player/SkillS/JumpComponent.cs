using UnityEngine;

public class JumpComponent : MonoBehaviour, IInputSkillComponent, IJump
{
    private Rigidbody m_rigidbody;
    private GroundDetector m_groundDetector;
    private float m_jumpHeight;

    public void InitializeFromSkill(Skill skill, GameObject owner)
    {
        m_rigidbody = owner.GetComponent<Rigidbody>();
        m_groundDetector = owner.GetComponent<PlayerManager>()?.GroundDetector;

        var jumpSO = skill as JumpSO;
        if (jumpSO != null)
        {
            m_jumpHeight = jumpSO.jumpHeight;
        }
    }

    public void OnInputPerformed()
    {
        if (m_groundDetector != null && m_groundDetector.IsGrounded())
        {
            Jump();
        }
    }

    public void Jump()
    {
        Vector3 velocity = m_rigidbody.linearVelocity;
        velocity.y = 0f;
        m_rigidbody.linearVelocity = velocity;

        float jumpForce = Mathf.Sqrt(2 * Physics.gravity.magnitude * m_jumpHeight) * m_rigidbody.mass;
        m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void OnRemove()
    {
        return;
    }
}
