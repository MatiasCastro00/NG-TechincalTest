using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [SerializeField] private Vector3 m_boxAreaDetector;
    [SerializeField] private float m_distanceToGround = 0.1f;
    [SerializeField] private LayerMask m_groundLayer;

    private bool m_inCoyoteTime;
    private float m_coyoteTime;

    public bool IsGrounded()
    {
        return Physics.BoxCast(transform.position, m_boxAreaDetector * 0.5f, -transform.up, Quaternion.identity, m_distanceToGround, m_groundLayer);

    }

    private void OnDrawGizmosSelected()
    {
        if (IsGrounded())
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Vector3 boxCenter = transform.position + -transform.up * (m_distanceToGround * 0.5f);
        Gizmos.DrawWireCube(boxCenter, m_boxAreaDetector);
        Gizmos.DrawLine(transform.position, transform.position - (transform.up * m_distanceToGround));
    }
    public bool IsOver(GameObject target)
    {
        RaycastHit hit;
        Vector3 halfExtents = m_boxAreaDetector * 0.5f;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.BoxCast(origin, halfExtents, Vector3.down, out hit, Quaternion.identity, m_distanceToGround))
        {
            return hit.collider != null && hit.collider.gameObject == target;
        }

        return false;
    }
}
