using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Refs")]
    public InventoryMenu InventoryMenu;
    public Skill Skill;
    public Slot CurrentSlot { get; set; }
    public Canvas Canvas;
    public Image image;

    [Header("UI References")]

    private string m_description;
    private List<string> m_stats = new();

    private RectTransform m_rectTransform;
    private CanvasGroup m_canvasGroup;

    private Transform m_originalParent;
    private Vector3 m_originalPosition;
    private Slot m_originalSlot;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();

        m_canvasGroup = GetComponent<CanvasGroup>();
        if (m_canvasGroup == null)
            m_canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (Canvas == null)
            Canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        if (Skill == null) return;

        m_description = Skill.Description;

        switch (Skill.Slot)
        {
            case SkillSlot.Movement:
                if (Skill is MoveSO move)
                {
                    m_stats.Add("Walk Speed: " + move.walkSpeed);
                    m_stats.Add("Run Speed: " + move.runSpeed);
                }
                break;

            case SkillSlot.Jump:
                if (Skill is JumpSO jump)
                {
                    m_stats.Add("Jump Height: " + jump.jumpHeight);
                }
                break;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryMenu == null || Skill == null)
            return;

        InventoryMenu.ItemDescription.Show(m_description, m_stats);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventoryMenu != null)
            InventoryMenu.ItemDescription.HidePanel();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click presionado sobre el item");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Click soltado sobre el item");
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Canvas == null)
        {
            Debug.LogWarning("Item: Canvas no asignado, no se puede arrastrar correctamente.");
            return;
        }

        m_originalParent = transform.parent;
        m_originalPosition = m_rectTransform.position;
        m_originalSlot = CurrentSlot;


        transform.SetParent(Canvas.transform);

        m_canvasGroup.blocksRaycasts = false;
        m_canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Canvas == null) return;

        if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            m_rectTransform.position = eventData.position;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Canvas.transform as RectTransform,
                eventData.position,
                Canvas.worldCamera,
                out Vector2 localPoint
            );
            m_rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Canvas == null) return;

        m_canvasGroup.blocksRaycasts = true;
        m_canvasGroup.alpha = 1f;
        if (transform.parent == Canvas.transform && CurrentSlot != null)
        {
            CurrentSlot.SetItem(this);
        }
    }
}
