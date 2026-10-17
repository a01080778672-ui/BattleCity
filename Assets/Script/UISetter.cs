using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UISetter : MonoBehaviour//카드만 만지는 CardUISetter와 다르게 얘는 다른 UI를 업데이트 해줍니다.
{
    [SerializeField] Canvas canvas;

    [SerializeField] GameObject deckViewer;//덱 창

    [SerializeField] GameObject graveViewer;//무덤 창

    [SerializeField] GameObject logViewer;//로그창
    [SerializeField] TextMeshProUGUI logText;//로그 텍스트(임시)


    [SerializeField] TextMeshProUGUI[] alarmText;//여러개의 알람이 필요할때를 대비하여 배열로 받음
    [SerializeField] RectTransform alarmPos;
    [SerializeField] TextMeshProUGUI FSMuiText;


    [SerializeField] float gap=1.0f;
    [SerializeField] float alarmFadeTime = 1.0f;
    [SerializeField] float alarmInitTime = 2.0f;

    

    //호버시 보일 큰 카드
    private CardView currShowCard;

    [SerializeField] GameObject BigHoverUiCard;
    [SerializeField] TextMeshProUGUI CardNameText;
    [SerializeField] TextMeshProUGUI CardTypeText;
    [SerializeField] TextMeshProUGUI CardCostText;
    [SerializeField] TextMeshProUGUI CardEffectText;
    [SerializeField] TextMeshProUGUI CardBottomAttackNumberText;
    [SerializeField] TextMeshProUGUI CardBottomPowerNumberText;
    [SerializeField] GameObject m_cardIcon;
    [SerializeField] Image illustration;
    //호버시 보일 큰 카드



    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.AlarmText>(e_Alarm);
        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMTextChange);
        EventBus.Subscribe<EventBus.CardMouseIn>(e_CardMouseIn);
        EventBus.Subscribe<EventBus.CardMouseOut>(e_cardMouseOut);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.AlarmText>(e_Alarm);
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMTextChange);
        EventBus.Unsubscribe<EventBus.CardMouseIn>(e_CardMouseIn);
        EventBus.Unsubscribe<EventBus.CardMouseOut>(e_cardMouseOut);

    }

  
    void e_CardMouseIn(EventBus.CardMouseIn e)//어떤 카드에 마우스가 들어오면
    {
        if (e.card == null||e.card.isFront==false) return;


        


        BigHoverUiCard.gameObject.SetActive(true);
        currShowCard=e.card;
  

   


      
        if (e.card.cardInstance.isCardDataSOVaild()==true)
        {
            //CardCostText.text = cardso.cardCost[0].cost.ToString() + " 코스트";

            e.card.DisplayCost(e.card.cardInstance.GetCardCost()[0].cost, m_cardIcon);
            CardNameText.text = e.card.cardInstance.GetCardName();
            CardBottomAttackNumberText.text = e.card.cardInstance.GetAttack().ToString();
            CardBottomPowerNumberText.text = e.card.cardInstance.GetPower().ToString();


            CardEffectText.text = "";
            illustration.sprite = e.card.cardInstance.GetIllustration();

            foreach (var effect in e.card.cardInstance.GetHitEffects())
            {
                if (effect.conditions.Length != 0)
                {
                    foreach (var condition in effect.conditions)
                        CardEffectText.text += condition.GetEvaluateScript() + "\n";//조건이 있으면 먼저 적음
                }
                

            CardEffectText.text += effect.effects.GetCardScript() + "\n";//그 카드의 적중효과를 적음

              
            }
            if (e.card.cardInstance.GetCardType() == CardDataSO.CardType.Block)
            {
                CardEffectText.text += string.Format("{0}방어력", e.card.cardInstance.GetBlockPower()) + "\n";
            }
            switch (e.card.cardInstance.GetCardType())
            {
                case CardDataSO.CardType.Attack:
                    CardTypeText.text = "공격";
                    break;
                case CardDataSO.CardType.Block:
                    CardTypeText.text = "수비";
                    break;
                default:
                    CardTypeText.text = "타입없음";
                    break;
            }
        }
        else
        {
            Debug.Log("카드데이터가 안왔다");
        }
       


    }
    void e_cardMouseOut(EventBus.CardMouseOut e)
    {
        if (e.card == null|| currShowCard!=e.card) return;


        BigHoverUiCard.gameObject.SetActive(false);
    }

    void e_FSMTextChange(EventBus.FSMChanged e)
    {
        if (FSMuiText == null) return;
        switch (e.curr)
        {
            case PlayerMainPhaseState:
                FSMuiText.text = "플레이어 메인 페이즈";
                break;
            case PlayerSettingBlockPhaseState:
                FSMuiText.text = "플레이어 방어세팅 페이즈";
                break;
            case PlayerTryBlockPhaseState:
                FSMuiText.text = "플레이어 방어시도 페이즈";
                break;
            case EnemyMainPhaseState:
                FSMuiText.text = "적의 메인 페이즈";
                break;
            case EnemySettingBlockPhaseState:
                FSMuiText.text = "적의 방어세팅 페이즈";
                break;
            case EnemyTryBlockPhaseState:
                FSMuiText.text = "적의 방어시도 페이즈";
                break;
            case StartSettingState:
                FSMuiText.text = "게임시작 페이즈";
                break;


        }

    }


    void e_Alarm(EventBus.AlarmText e)
    {
        SetAlarm(e.alarmText);
    }
    void SetAlarm(string text)
    {
      
        foreach (var item in alarmText)//알람 텍스트들 중에서 반복문을 돌림
        {
            if (!item.gameObject.activeSelf)//그 텍스트 중에서 살아있지 않은 놈이라면
            {
                item.gameObject.SetActive(true);//다시 활성화 시킴
                item.text = text;//텍스트도 초기화
                item.rectTransform.position = alarmPos.position;//위치도 첫번째로
                PlayAlarmFade(item, alarmInitTime, alarmFadeTime);//두튜윈 재생 시작! 
             
                foreach (var item2 in alarmText)
                {
                    if(item2.gameObject.activeSelf==true&&item!=item2)
                    {
                        item2.rectTransform.position += new Vector3(0, gap, 0);
                    }
                }


                return;
            }
        }


        //여기 밑으로 갔다는 뜻은 쉬고있는 알림 텍스트가 없다는 뜻임.



        TextMeshProUGUI remindText = null;
        foreach(var item in alarmText)
        {
            if(remindText == null)
            {
                remindText = item;
                continue;
            }

            if(remindText.rectTransform.position.y<item.rectTransform.position.y)
            {
                remindText = item;
            }
        }
        remindText.text = text;//가장 위에 있던 놈을 가져와 텍스트 초기화
        remindText.rectTransform.position = alarmPos.position;//위치도 첫번째로
        PlayAlarmFade(remindText, alarmInitTime, alarmFadeTime);//두튜윈 재생 시작! 
        
        
        foreach (var item in alarmText)
        {
            if (item.gameObject.activeSelf == true && remindText != item)
            {
                item.rectTransform.position += new Vector3(0, gap, 0);
            }
        }

    }
    void PlayAlarmFade(TextMeshProUGUI text,float initTime,float fadeTime)//두튜윈을 이용해 만들어짐
    {
        DOTween.Kill(text);//이미 붙어있던 두튜윈을 제거해야함
        text.alpha = 1.0f;

        Sequence seq = DOTween.Sequence();
        seq.SetTarget(text);
        
       
        seq.AppendInterval(initTime); // 5초 대기

        seq.Append(
            text.DOFade(0f, fadeTime) // 1초 동안 페이드 아웃
        );

        seq.OnComplete(() =>
        {
            text.gameObject.SetActive(false);//전부 실행 완료시 비활성화함
        });
    }

    public void OpenDeck()//덱 버튼에 줄 예정
    {
        EventBus.Publish<EventBus.DeckOpenButtonClicked>(new EventBus.DeckOpenButtonClicked());
        deckViewer.transform.localScale = Vector3.one;
    }
    public void CloseDeck()//덱 닫기 버튼에 줄 예정
    {
        deckViewer.transform.localScale = Vector3.zero;
    }
    public void OpenGrave()//무덤 버튼에 줄 예정
    {
        EventBus.Publish<EventBus.GraveOpenButtonClicked>(new EventBus.GraveOpenButtonClicked());
        graveViewer.transform.localScale = Vector3.one;
    }
    public void CloseGrave()//무덤 닫기 버튼에 줄 예정
    {
        graveViewer.transform.localScale = Vector3.zero;
    }
    public void OpenLog()//로그 버튼에 줄 예정
    {
        EventBus.Publish<EventBus.LogOpenButtonClicked>(new EventBus.LogOpenButtonClicked());
        logViewer.transform.localScale = Vector3.one;
    }
    public void CloseLog()//로그 닫기 버튼에 줄 예정
    {
        logViewer.transform.localScale = Vector3.zero;
    }



}
