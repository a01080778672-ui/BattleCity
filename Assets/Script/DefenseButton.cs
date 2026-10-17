using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventBus;

public class DefenseButton : MonoBehaviour
{
    // 버튼 
    //  PosX: -270   PosY: 350
    //  Width: 115 Height: 108
    [SerializeField] FSMManager fsmmanager;
    CardUseManager cardUseManager;
    [SerializeField] Button defenseButton;
    private Image image;
    private TMP_Text text;

    private void Start()
    {
        cardUseManager = FindAnyObjectByType<CardUseManager>();

        image = GetComponent<Image>();
        text = GetComponentInChildren<TMP_Text>();

        image.enabled = false;
        defenseButton.interactable = false;
        text.enabled = false;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.StartPlayerTryBlockPhaseEvent>(e_DefenseButtonChanger);
        EventBus.Subscribe<EventBus.CardLeftClickedEvent>(e_DefenseButtonChanger);
        EventBus.Subscribe<EventBus.StartEnemyMainPhaseEvent>(e_DefenseButtonChanger);

    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.StartPlayerTryBlockPhaseEvent>(e_DefenseButtonChanger);
        EventBus.Unsubscribe<EventBus.CardLeftClickedEvent>(e_DefenseButtonChanger);
        EventBus.Unsubscribe<EventBus.StartEnemyMainPhaseEvent>(e_DefenseButtonChanger);
    }
    //버튼 활성화
    private void e_DefenseButtonChanger(EventBus.StartPlayerTryBlockPhaseEvent e)
    {
        image.enabled = true;
        defenseButton.interactable = true;
        text.enabled = true;
        image.color = Color.red;
    }
    //버튼 텍스트 변경 : 여기서 바로 바꾸면 카드가 선택 되기 전에 이 함수가 실행되어 실제 값이 한번 더 클릭해야 바로 직전 클릭시의 값이 나와 coroutine을 사용하였습니다.
    private void e_DefenseButtonChanger(EventBus.CardLeftClickedEvent e)
    {
        StartCoroutine(changeText());
    }
    //버튼 비활성화
    private void e_DefenseButtonChanger(EventBus.StartEnemyMainPhaseEvent e)
    {
        image.enabled = false;
        defenseButton.interactable = false;
        text.enabled = false;
    }

    //버튼 텍스트 변경 코루틴
    private IEnumerator changeText()
    {
        yield return null;

        text.text = cardUseManager.CheckCurrSelectedBlockCard() == 0
            ? "방어 포기"
            : "방어 확정";

        image.color = cardUseManager.CheckBlockSuccess()
            ? Color.green
            : Color.red;
    }

    // 버튼 클릭시 함수
    public void DefenseButtonPutted()
    {
        if (fsmmanager.GetCurrState() is PlayerTryBlockPhaseState)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "방어 카드 사용 확정 버튼을 눌렀습니다." });
            EventBus.Publish(new EventBus.BlockButtonClicked { });
            Debug.Log("방어버튼 클릭됨");
        }
    }
}


