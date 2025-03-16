using UnityEngine;
using UnityEngine.EventSystems;

public class CustomizableButton : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform m_rectTransform;
    [SerializeField] private ButtonsSettingsView m_buttonSettingsContent;
    [SerializeField] private GameObject m_selectedHighlight;
    [SerializeField] private ControlMode m_controlMode = ControlMode.CombatMode;

    [SerializeField, ReadOnly] private int m_buttonId = -1;

    private Vector2 m_defaultPosition;
    private Vector2 m_defaultSize;

    public Vector2 ButtonDefaultSize => m_defaultSize;

    public RectTransform ButtonRectTransform => m_rectTransform;

    public ControlMode ButtonControlMode => m_controlMode;

    public int ButtonId => m_buttonId;

    public void SetButtonId(int id) => m_buttonId = id;

    public void OnDrag(PointerEventData eventData) => m_rectTransform.anchoredPosition += eventData.delta;

    public void OnPointerDown(PointerEventData eventData) => m_buttonSettingsContent.SelectButton(this);

    public void SetSelected(bool isSelected) => m_selectedHighlight.SetActive(isSelected);

    public void ChangeSize(float newSize) => m_rectTransform.sizeDelta = new Vector2(newSize, newSize);

    public void ResetToDefault()
    {
        m_rectTransform.anchoredPosition = m_defaultPosition;
        m_rectTransform.sizeDelta = m_defaultSize;
    }

    protected void Awake()
    {
        m_defaultPosition = m_rectTransform.anchoredPosition;
        m_defaultSize = m_rectTransform.sizeDelta;
    }
}
