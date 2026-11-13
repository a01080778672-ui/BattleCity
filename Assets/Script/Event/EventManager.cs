using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [Header("Event Data")]
    [SerializeField] private EventData[] eventDatabase;

    [Header("Event UI")]
    [SerializeField] private Image[] eventImages;

    // 선택된 이벤트 3개의 데이터
    private EventData[] selectedEvents;

    private void Start()
    {
        GenerateRandomEvents();
    }

    public void GenerateRandomEvents()
    {
        // 이벤트 수 확인
        if (eventDatabase.Length < eventImages.Length)
        {
            Debug.LogError("이벤트 데이터가 부족합니다.");
            return;
        }

        // 전체 이벤트 목록 복사
        List<EventData> availableEvents =
            new List<EventData>(eventDatabase);

        selectedEvents = new EventData[eventImages.Length];

        for (int i = 0; i < eventImages.Length; i++)
        {
            // 랜덤 인덱스 선택
            int randomIndex = Random.Range(0, availableEvents.Count);

            // 이벤트 데이터 저장
            EventData selectedEvent = availableEvents[randomIndex];
            selectedEvents[i] = selectedEvent;

            // UI 이미지 변경
            eventImages[i].sprite = selectedEvent.eventImage;

            // 선택된 이벤트를 목록에서 제거 (중복 방지)
            availableEvents.RemoveAt(randomIndex);
        }
    }

    // 선택한 이벤트 데이터 반환
    public EventData GetSelectedEvent(int index)
    {
        if (selectedEvents == null ||
            index < 0 ||
            index >= selectedEvents.Length)
        {
            Debug.LogError("잘못된 이벤트 인덱스입니다.");
            return null;
        }

        return selectedEvents[index];
    }

    public void SelectEvent(int index)
    {
        EventData selectedEvent = GetSelectedEvent(index);

        if (selectedEvent == null)
        {
            Debug.LogError("이벤트 데이터가 null입니다.");
            return;
        }

        Debug.Log($"이벤트 이름: {selectedEvent.eventName}");
        Debug.Log($"이벤트 설명: {selectedEvent.description}");
    }

    public string GetEventDescription(int index)
    {
        EventData selectedEvent = GetSelectedEvent(index);

        if (selectedEvent == null)
            return "";

        return selectedEvent.description;
    }
}