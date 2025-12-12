using UnityEngine;

public class SkillPickup : MonoBehaviour
{
    [SerializeField] Skill m_skill;
    [SerializeField] SpriteRenderer m_spriteRenderer;
    [SerializeField] float m_floatAmplitude = 0.25f;
    [SerializeField] float m_floatSpeed = 2f;
    [SerializeField] float m_rotationSpeed = 45f;
    [SerializeField] Interactable m_interactable;

    Vector3 m_startPos;

    void Awake()
    {
        if (m_interactable == null)
            m_interactable = GetComponent<Interactable>();
    }

    void OnEnable()
    {
        if (m_interactable != null)
            m_interactable.OnInteract += HandleInteract;
    }

    void OnDisable()
    {
        if (m_interactable != null)
            m_interactable.OnInteract -= HandleInteract;
    }

    void Start()
    {
        m_startPos = transform.position;

        if (m_spriteRenderer != null && m_skill != null && m_skill.SkillIcon != null)
            m_spriteRenderer.sprite = m_skill.SkillIcon;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * m_floatSpeed) * m_floatAmplitude;
        transform.position = m_startPos + new Vector3(0, y, 0);

        transform.Rotate(Vector3.forward, m_rotationSpeed * Time.deltaTime);
    }

    void HandleInteract()
    {
        if (InventoryMenu.Instance != null && m_skill != null)
            InventoryMenu.Instance.AddNewItem(m_skill);

        Destroy(gameObject);
    }
}
