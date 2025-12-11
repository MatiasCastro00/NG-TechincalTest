using UnityEngine;

public class MoveComponent : MonoBehaviour, ISkillComponent, IMove
{
    private Rigidbody m_rb;
    private float m_walkSpeed;
    private float m_runSpeed;
    private PlayerInputManager m_input;

    public void InitializeFromSkill(Skill skill, GameObject owner)
    {
        m_rb = owner.GetComponent<Rigidbody>();
        m_input = owner.GetComponent<PlayerInputManager>();

        var moveSO = skill as MoveSO;
        if (moveSO != null)
        {
            m_walkSpeed = moveSO.walkSpeed;
            m_runSpeed = moveSO.runSpeed;
        }
    }

    public void Move(Vector3 direction, bool isSprinting)
    {
        float speed = isSprinting ? m_runSpeed : m_walkSpeed;
        Vector3 movement = direction * speed * Time.deltaTime;
        m_rb.MovePosition(m_rb.position + movement);
    }

    public void OnRemove()
    {
        return;
    }
}
