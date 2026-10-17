using DG.Tweening; // DOTween 네임스페이스를 반드시 추가

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class CardSetter : MonoBehaviour//플레이어에게 카드 이동을 보여주기 위해 일하는 클래스
{
    [SerializeField] private GameObject hand;//내가 들고있는 카드를 이것의 자식으로 등록할것임.

    [SerializeField] private RectTransform graveIcon;  // 무덤 아이콘
    [SerializeField] private RectTransform deckIcon;   // 덱 아이콘

    [SerializeField] private RectTransform myCardLeft;//최대 좌측 위치
    [SerializeField] private RectTransform myCardRight;//최대 우측 위치

    [SerializeField] private RectTransform usedCardLeft;//가드존
    [SerializeField] private RectTransform usedCardRight;//가드존

    [SerializeField] private RectTransform otherCardLeft;//상대카드존
    [SerializeField] private RectTransform otherCardRight;//상대카드존


    [SerializeField] private RectTransform graveContent;//무덤 창에 들어갈 카드들의 부모
    [SerializeField] private RectTransform deckContent;//덱 창에 들어갈 카드들의 부모

    [SerializeField] GameObject Card;//손패에 실존하는 카드 넣음
    [SerializeField] GameObject viwerCard;//판넬(무덤창이나 덱창)창에 있을 카드 프리펩 넣음

    [SerializeField] CardDataManager cardDataManager;//카드가 보이는 것을 바꿔주는 정도이기에 매니저에 등록은 아님. 카드의 현 데이터 상태를 알기 위해 주입은 받음.

    [SerializeField] List<CardView> m_HandCards;//게임상에 진짜로 나와있는 카드들중 손패 카드들을 여기다 기록(저장)합니다
    [SerializeField] List<CardView> m_BlockCards;//방어 카드의 대기를 구현하기 위한 방어카드 대기 존 카드들을 여기다 둡니다.

    [SerializeField] CardView currSelectedCard=null;//현재 선택중인 카드 오브젝트를 저장하기 위해

    private void OnEnable()//이벤트를 구독한다
    {
        StartCoroutine(SubscribeEventDelay());//이 스크립트에서의 이벤트는, 마스터 매니저 경우해야하는데 awake보다 늦춰져야만 함. awake와 onenable은 섞일수 있으므로 한번 기다렸다가 이벤트 구독 
            
       
    }
    IEnumerator SubscribeEventDelay()
    {
        CardDataManager dataManager = null;

        while (MasterManager.Instance?.GetManager<CardDataManager>() == null)
            yield return null;
        dataManager = MasterManager.Instance.GetManager<CardDataManager>();
        dataManager.drawCards += e_PlayerTurnStarted;
        dataManager.trashCard += DeleteCardToGrave;



        EventBus.Subscribe<EventBus.EndPlayerTurnEvent>(e_EndPlayerTurn);

        EventBus.Subscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);

        EventBus.Subscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);


    }



    private void OnDisable()//이벤트를 구독 해제한다
    {
        if (MasterManager.Instance.GetManager<CardDataManager>() != null)
        {
            MasterManager.Instance.GetManager<CardDataManager>().drawCards -= e_PlayerTurnStarted;
        }
        EventBus.Unsubscribe<EventBus.EndPlayerTurnEvent>(e_EndPlayerTurn);

        EventBus.Unsubscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);

        EventBus.Unsubscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);
    }




    public Vector3 GetWorldPositionFromUI(RectTransform uiElement)//캔버스 좌표를 주면 월드 좌표상 어디로 가야 거기로 가는 것 처럼 보이는지 알려준다.
    {
        // Overlay는 스크린 좌표 = UI 좌표 그대로
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null,uiElement.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }


    
    void e_CardLeftClicked(EventBus.CardLeftClickedEvent e)//이벤트 버스에서 구독함.invoke() 하면 실행됨
    {
        if (e.card == null) return;
        
        if (currSelectedCard !=e.card)//눌린 카드가 새로운 카드일시.
        {
            if(currSelectedCard !=null) currSelectedCard.selected = false;//이전 카드는 선택을 안하도록 처리
        }
        currSelectedCard = e.card;//선택카드 갱신
        e.card.selected = true;//눌린 카드는 선택했다 처리.
     
    }
    void e_CardRightClicked(EventBus.CardRightClickedEvent e)
    {
        if (e.card == null) return;

      



    }


    void e_EndPlayerTurn(EventBus.EndPlayerTurnEvent e)//플레이어 턴 끝 이벤트 invoke() 하면 이게 실행되게 할 것이다.
    {
        DeleteAllCardToGrave();
    }

    void e_PlayerTurnStarted(List<CardInstance> newCards)//플레이어 턴 시작 이벤트invoke() 하면 이게 실행되게 할 것이다
    {
        DeleteAllCardToGrave();//드로우 전에 혹시 모르니까 일단 다시 정리합니다

        foreach (var card in newCards)
        {
            GameObject newCard = Instantiate(Card, hand.transform);//새 카드 오브젝트를 만들어냄.
            newCard.transform.position = deckIcon.transform.position;//그 카드는 덱 아이콘에 일단 배치시킴
            CardView cardcomp = newCard.GetComponent<CardView>();
            
            CardDataSO carddata = MasterManager.Instance.cardDB.GetCardSO(card.CardDataSO.cardId);

            cardcomp.Init(carddata, card);//카드so와 카드 인스턴스 클래스 정보 전달
            m_HandCards.Add(cardcomp);//관리하는 카드에 추가
        }



     

        for(int i=0;i< m_HandCards.Count;i++)
        {
            float gap = 0f;
            if (m_HandCards.Count - 1 != 0)
            {
                gap = MathF.Abs((myCardLeft.transform.position.x - myCardRight.transform.position.x) / (m_HandCards.Count - 1));
            }
            else
            {
                gap = MathF.Abs((myCardLeft.transform.position.x - myCardRight.transform.position.x) / 2);
            }

            Vector3 newPos = new Vector3(myCardLeft.transform.position.x + gap * (i), myCardLeft.transform.position.y, myCardLeft.transform.position.z);

            m_HandCards[i].transform.DOMove(newPos, 0.5f);
        }

        UpdateAllGraveViewer();
        UpdateAllDeckViewer();
    }

    void DeleteCardToGrave(CardInstance usedCard)//하나의 카드만 무덤으로 보낸다(피치)
    {
        CardView targetCard = null;

        foreach (CardView item in m_HandCards)
        {
            if (item.cardInstance == usedCard)
            {
                targetCard = item;
                break;
            }
        }

        if (targetCard != null)
        {


            targetCard.clickAble = false;
            DOTween.Kill(targetCard.transform);
            m_HandCards.Remove(targetCard);
            targetCard.transform.localScale = Vector3.one;
            targetCard.transform.DOScale(Vector3.zero, 0.7f);
            targetCard.transform.DOMove(graveIcon.transform.position, 0.7f)
                .OnComplete(() => Destroy(targetCard.gameObject));
            return;
        }

        Debug.Log("버릴 카드를 찾지 못했다?");
    }

    void DeleteAllCardToGrave()//기존 카드들을 무덤으로 보낸다.
    {
        foreach (var item in m_HandCards)
        {
            item.clickAble = false;
            item.transform.localScale = Vector3.one;
        }

        foreach (var item in m_HandCards)//내가 관리하는 카드들로 반복문을 돌림
        {
            item.transform.DOScale(new Vector3(0,0,0), 0.5f);
            
            item.transform.DOMove(graveIcon.transform.position, 0.5f).OnComplete(()=>Destroy(item.gameObject));
        }

        m_HandCards.Clear();
    }






    public void UpdateAllGraveViewer()
    {
        GameObject makedCard;
        CardDataSO cardSObuffer;

        if (cardDataManager == null||graveContent==null)
        {
            Debug.Log("뭔가 잘못됨");
            return;
        }

        for(int i=graveContent.childCount-1  ;  i>=0  ;  i--)//기존에 있던 카드들을 제거
        {
            Destroy(graveContent.GetChild(i).gameObject);
        }

        foreach (var item in cardDataManager.currCardData.graveCards)//카드 데이터 메니저를 참고해서 새로 만든다.
        {
           makedCard = Instantiate(viwerCard, graveContent);

           cardSObuffer = MasterManager.Instance.cardDB.GetCardSO(item.CardDataSO.cardId);

            makedCard.GetComponent<CardView>()?.Init(cardSObuffer, item);
          
        }



    }


    public void UpdateAllDeckViewer()
    {
        GameObject makedCard;
        CardDataSO cardSObuffer;

        if (cardDataManager == null || deckContent == null)
        {
            Debug.Log("뭔가 잘못됨");
            return;
        }

        for (int i = deckContent.childCount - 1; i >= 0; i--)//기존에 있던 카드들을 제거
        {
            Destroy(deckContent.GetChild(i).gameObject);
        }

        foreach (var item in cardDataManager.currCardData.DeckCards)//카드 데이터 메니저를 참고해서 새로 만든다.
        {
            makedCard = Instantiate(viwerCard, deckContent);

            cardSObuffer = MasterManager.Instance.cardDB.GetCardSO(item.CardDataSO.cardId);

            makedCard.GetComponent<CardView>()?.Init(cardSObuffer, item);
        }

    }
  
}
