using UnityEngine;
using UnityEngine.EventSystems;

public class EventButtonUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private EventManager eventManager;
    [SerializeField] private EventTooltipUI tooltipUI;

    [Header("Event")]
    [SerializeField] private int eventIndex;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventData selectedEvent =
            eventManager.GetSelectedEvent(eventIndex);

        if (selectedEvent == null)
            return;

        tooltipUI.Show(selectedEvent, rectTransform, eventIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI.Hide();
    }

    private void OnDisable()
    {
        if (tooltipUI != null)
            tooltipUI.Hide();
    }
}