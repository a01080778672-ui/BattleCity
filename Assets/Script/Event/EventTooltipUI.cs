using UnityEngine;
using TMPro;

public class EventTooltipUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform tooltipPanel;
    [SerializeField] private TextMeshProUGUI eventNameText;
    [SerializeField] private TextMeshProUGUI eventDescriptionText;

    [Header("Tooltip Settings")]
    [SerializeField] private float maxWidth = 500f;
    [SerializeField] private float padding = 20f;
    [SerializeField] private float textSpacing = 10f;

    [Header("Tooltip Position")]
    [SerializeField] private Vector2 tooltipOffset = new Vector2(10f, 0f);

    private RectTransform canvasRect;

    private void Awake()
    {
        canvasRect = canvas.GetComponent<RectTransform>();

        // 툴팁 위치 기준: 왼쪽 중앙
        tooltipPanel.anchorMin = new Vector2(0, 0.5f);
        tooltipPanel.anchorMax = new Vector2(0, 0.5f);

        // 텍스트 위치 기준: 왼쪽 상단
        SetupText(eventNameText);
        SetupText(eventDescriptionText);

        // 툴팁이 마우스 이벤트를 가로채지 않도록 설정
        CanvasGroup group = tooltipPanel.GetComponent<CanvasGroup>();

        if (group == null)
            group = tooltipPanel.gameObject.AddComponent<CanvasGroup>();

        group.blocksRaycasts = false;
        group.interactable = false;

        Hide();
    }

    private void SetupText(TextMeshProUGUI text)
    {
        RectTransform rect = text.rectTransform;

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);

        text.enableWordWrapping = true;
        text.enableAutoSizing = false;
    }

    public void Show(EventData eventData, RectTransform targetButton, int eventIndex)
    {
        if (eventData == null || targetButton == null)
            return;

        tooltipPanel.gameObject.SetActive(true);
        tooltipPanel.SetAsLastSibling();

        eventNameText.text = eventData.eventName;
        eventDescriptionText.text = eventData.description;

        UpdateSize();
        UpdatePosition(targetButton, eventIndex);
    }

    private void UpdateSize()
    {
        // 화면 너비와 최대 너비를 고려
        float availableWidth = Mathf.Max(
            1f, canvasRect.rect.width - 20f
        );

        float widthLimit = Mathf.Min(maxWidth, availableWidth);
        float maxTextWidth = Mathf.Max(1f, widthLimit - padding * 2f);

        // 줄바꿈 없는 텍스트의 원래 너비 계산
        float nameWidth = eventNameText
            .GetPreferredValues(eventNameText.text, Mathf.Infinity, Mathf.Infinity).x;

        float descriptionWidth = eventDescriptionText
            .GetPreferredValues(eventDescriptionText.text, Mathf.Infinity, Mathf.Infinity).x;

        // 내용에 따라 가로 크기 결정 (최대 500)
        float textWidth = Mathf.Min(
            Mathf.Max(nameWidth, descriptionWidth),
            maxTextWidth
        );

        textWidth = Mathf.Max(1f, textWidth);

        // 결정된 너비에 맞춰 줄바꿈 및 높이 계산
        float nameHeight = eventNameText
            .GetPreferredValues(eventNameText.text, textWidth, Mathf.Infinity).y;

        float descriptionHeight = eventDescriptionText
            .GetPreferredValues(eventDescriptionText.text, textWidth, Mathf.Infinity).y;

        float panelWidth = textWidth + padding * 2f;
        float panelHeight =
            nameHeight + descriptionHeight +
            padding * 2f + textSpacing;

        // 패널 크기 설정
        tooltipPanel.sizeDelta = new Vector2(panelWidth, panelHeight);

        // 이름 텍스트
        eventNameText.rectTransform.sizeDelta =
            new Vector2(textWidth, nameHeight);

        eventNameText.rectTransform.anchoredPosition =
            new Vector2(padding, -padding);

        // 설명 텍스트
        eventDescriptionText.rectTransform.sizeDelta =
            new Vector2(textWidth, descriptionHeight);

        eventDescriptionText.rectTransform.anchoredPosition =
            new Vector2(padding, -padding - nameHeight - textSpacing);
    }

    private void UpdatePosition(
    RectTransform targetButton,
    int eventIndex)
    {
        // 버튼의 월드 좌표 모서리 가져오기
        Vector3[] corners = new Vector3[4];
        targetButton.GetWorldCorners(corners);

        // GetWorldCorners 순서:
        // 0 = 좌하단
        // 1 = 좌상단
        // 2 = 우상단
        // 3 = 우하단

        bool isLastEvent = eventIndex == 2;

        // 0, 1번은 버튼 우상단
        // 2번은 버튼 좌상단
        Vector3 targetCorner = isLastEvent
            ? corners[1]
            : corners[2];

        // Canvas 카메라
        Camera uiCamera =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        // 월드 좌표 → 스크린 좌표
        Vector2 screenPoint =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera, targetCorner
            );

        // 스크린 좌표 → Canvas 로컬 좌표
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            uiCamera,
            out Vector2 localPoint
        );

        // 툴팁 Pivot 설정
        // 0, 1번: 좌상단
        // 2번: 우상단
        tooltipPanel.pivot = isLastEvent
            ? new Vector2(1f, 1f)
            : new Vector2(0f, 1f);

        // 이벤트 인덱스에 따라 X 오프셋 방향 변경
        float xOffset = isLastEvent
            ? -tooltipOffset.x
            : tooltipOffset.x;

        Vector2 position = localPoint + new Vector2(
            xOffset,
            tooltipOffset.y
        );

        // 툴팁 배치
        // Canvas 기준 좌표를 사용하므로
        // Anchor를 왼쪽 아래 (0, 0)로 설정
        tooltipPanel.anchorMin = Vector2.zero;
        tooltipPanel.anchorMax = Vector2.zero;

        tooltipPanel.anchoredPosition =
            position - canvasRect.rect.min;
    }

    public void Hide()
    {
        tooltipPanel.gameObject.SetActive(false);
    }
}