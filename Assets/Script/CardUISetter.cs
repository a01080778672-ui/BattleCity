using DG.Tweening; // DOTween 네임스페이스를 반드시 추가

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class CardUISetter : MonoBehaviour//플레이어에게 카드 이동을 보여주기 위해 일하는 클래스 
    // 덱 창과 무덤 창도 일단은 여기서 관리하는데 분리할수도 있고 아닐수도 있고..
{
    [SerializeField] GameObject deckViewer;//덱 창

    [SerializeField] GameObject graveViewer;//무덤 창


    [SerializeField] private GameObject hand;//내가 들고있는 카드를 이것의 자식으로 등록할것임.

    [SerializeField] private RectTransform graveIcon;  // 무덤 아이콘
    [SerializeField] private RectTransform deckIcon;   // 덱 아이콘

    [SerializeField] private RectTransform enemyGraveIcon;  // 무덤 아이콘
    [SerializeField] private RectTransform enemyDeckIcon;   // 덱 아이콘



    [SerializeField] private RectTransform enemyHandCardLeft;//적손패카드존
    [SerializeField] private RectTransform enemyHandCardRight;//적손패카드존

    [SerializeField] private RectTransform enemyBlockCardLeft;//적가드존
    [SerializeField] private RectTransform enemyBlockCardRight;//적가드존

    [SerializeField] private RectTransform PlayerBlockCardLeft;//플레이어가드존
    [SerializeField] private RectTransform PlayerBlockCardRight;//플레이어가드존

    [SerializeField] private RectTransform PlayerHandCardLeft;//플레이어 손패 최대 좌측 위치
    [SerializeField] private RectTransform PlayerHandCardRight;//플레이어 손패 최대 우측 위치

    [SerializeField] private RectTransform PlayerAttackZone;//중앙에 있는 공격카드 위치
    [SerializeField] private RectTransform EnemyAttackZone;


    [SerializeField] private RectTransform graveContent;//무덤 창에 들어갈 카드들의 부모
    [SerializeField] private RectTransform deckContent;//덱 창에 들어갈 카드들의 부모

    [SerializeField] GameObject Card;//손패에 실존하는 카드 넣음(instantiate를 위해서)
    [SerializeField] GameObject viwerCard;//판넬(무덤창이나 덱창)창에 있을 카드 프리펩 넣음(instantiate를 위해서)

    [SerializeField] FSMManager fsmManager;

    List<CardView> playerHandCards;//게임상에 진짜로 나와있는 카드들중 손패 카드들을 여기다 기록(저장)합니다
    List<CardView> playerBlockCards;//플레이어 방어존 표현
    List<CardView> enemyHandCards;//플레이어 방어존 표현
    List<CardView> enemyBlockCards;//플레이어 방어존 표현
    
    public CardView attackCard;//중앙에 있는 공격카드 표현

    CardView currSelectedCard=null;//현재 선택중인 카드 오브젝트를 저장하기 위해


    private void Awake()
    {
        playerHandCards = new List<CardView>();
        playerBlockCards = new List<CardView>();
        enemyHandCards=new List<CardView>();
        enemyBlockCards = new List<CardView>();
    }
    private void OnEnable()//이벤트를 구독한다
    {


        EventBus.Subscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);

        EventBus.Subscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);



        EventBus.Subscribe<EventBus.UpdateDeck>(e_UpdateAllDeckViewer);

        EventBus.Subscribe<EventBus.UpdateGrave>(e_UpdateAllGraveViewer);



        EventBus.Subscribe<EventBus.RelocateCardUI>(e_RelocateUICard);

        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMchanged);
    }




    private void OnDisable()//이벤트를 구독 해제한다
    {
    


        EventBus.Unsubscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);

        EventBus.Unsubscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);



        EventBus.Unsubscribe<EventBus.UpdateDeck>(e_UpdateAllDeckViewer);

        EventBus.Unsubscribe<EventBus.UpdateGrave>(e_UpdateAllGraveViewer);



        EventBus.Unsubscribe<EventBus.RelocateCardUI>(e_RelocateUICard);

        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMchanged);
    }


    void e_FSMchanged(EventBus.FSMChanged e)
    {
        currSelectedCard = null;
    }


    void e_RelocateUICard(EventBus.RelocateCardUI e)//하나의 카드를 어디에서 어디로 옮겨주는 것
    {

        CardView bufferCard = null;
        GameObject newCard = null;


          switch (e.from)//어디서 이동하는지 스위치 문 돌립니다.
            {
                case CommonClass.ZoneType.None:
                    //시작이 none이면 그냥 아무것도 안함
                    return;
                case CommonClass.ZoneType.PlayerHandZone:
                {
                    foreach (var item in playerHandCards)
                    {
                        if (item.cardInstance == e.card)
                        {
                            bufferCard = item;
                           
                            break;
                        }
                    }
                    if (bufferCard == null)
                    {
                        Debug.Log("뭔가 이상하다");
                    }
                    playerHandCards.Remove(bufferCard);
                    ArrangeCardAsFan(playerHandCards, PlayerHandCardLeft, PlayerHandCardRight);
                }
                    break;
                case CommonClass.ZoneType.PlayerBlockZone:
                {
                    foreach (var item in playerBlockCards)
                    {
                        if (item.cardInstance == e.card)
                        {
                            bufferCard = item;
                            break;
                        }
                    }
                    if (bufferCard == null)
                    {
                        Debug.Log("뭔가 이상하다");
                    }
                    playerBlockCards.Remove(bufferCard);
                    ArrangeCardAsList(playerBlockCards, PlayerBlockCardLeft, PlayerBlockCardRight);
                }
                    break;
                case CommonClass.ZoneType.PlayerDeckZone:
                {
                    newCard = Instantiate(Card, hand.transform);//새 카드 오브젝트를 만들어냄.
                    newCard.transform.position = deckIcon.transform.position;//그 카드는 덱 아이콘에 일단 배치시킴
                    CardView cardcomp = newCard.GetComponent<CardView>();
                    cardcomp.Init(e.card);//카드so와 카드 인스턴스 클래스 정보 전달
                    bufferCard= cardcomp;
                }
                break;
                case CommonClass.ZoneType.PlayerGraveZone:
                {
                    newCard = Instantiate(Card, hand.transform);//새 카드 오브젝트를 만들어냄.
                    newCard.transform.position = graveIcon.transform.position;//그 카드는 무덤 아이콘에 일단 배치시킴
                    CardView cardcomp = newCard.GetComponent<CardView>();
                    cardcomp.Init(e.card);//카드so와 카드 인스턴스 클래스 정보 전달
                    bufferCard = cardcomp;
                }
                    break;
                case CommonClass.ZoneType.EnemyHandZone:
                {
                    foreach (var item in enemyHandCards)
                    {
                        if (item.cardInstance == e.card)
                        {
                            bufferCard = item;
                            break;
                        }
                    }
                    if (bufferCard == null)
                    {
                        Debug.Log("뭔가 이상하다");
                    }
                    enemyHandCards.Remove(bufferCard);
                    ArrangeCardAsFan(enemyHandCards, enemyHandCardLeft, enemyHandCardRight);
                }
                    break;
                case CommonClass.ZoneType.EnemyBlockZone:
                {
                    foreach (var item in enemyBlockCards)
                    {
                        if (item.cardInstance == e.card)
                        {
                            bufferCard = item;
                            break;
                        }
                    }
                    if (bufferCard == null)
                    {
                        Debug.LogWarning("뭔가 이상하다");
                    }
                    enemyBlockCards.Remove(bufferCard);
                    ArrangeCardAsList(enemyBlockCards, enemyBlockCardLeft, enemyBlockCardRight);
                }
                    break;
                case CommonClass.ZoneType.EnemyDeckZone:
                {
                    newCard = Instantiate(Card, hand.transform);//새 카드 오브젝트를 만들어냄.
                    newCard.transform.position = enemyDeckIcon.transform.position;
                    CardView cardcomp = newCard.GetComponent<CardView>();
                    cardcomp.Init(e.card);//카드so와 카드 인스턴스 클래스 정보 전달
                    bufferCard = cardcomp;
                }
                break;
                case CommonClass.ZoneType.EnemyGraveZone:
                {
                    newCard = Instantiate(Card, hand.transform);//새 카드 오브젝트를 만들어냄.
                    newCard.transform.position = enemyGraveIcon.transform.position;
                    CardView cardcomp = newCard.GetComponent<CardView>();
                    cardcomp.Init(e.card);//카드so와 카드 인스턴스 클래스 정보 전달
                    bufferCard = cardcomp;
                }
                break;
            case CommonClass.ZoneType.PlayerAttackZone:
                {
                    if (attackCard?.cardInstance == e.card)
                    {
                        bufferCard = attackCard;
                        attackCard = null;
                    }
                    else
                    {
                        Debug.LogWarning("뭔가이상하다");
                    }
              
                }
                break;
            case CommonClass.ZoneType.EnemyAttackZone:
                {
                    if (attackCard?.cardInstance == e.card)
                    {
                        bufferCard = attackCard;
                        attackCard = null;
                    }
                    else
                    {
                        Debug.LogWarning("뭔가이상하다");
                    }

                }
                break;
        }
        
        if (bufferCard == null)
        {
            return;
        }
        bufferCard.selected = false;

            switch (e.to)//도착지가 어디인가
            {
                case CommonClass.ZoneType.None:
                    //도착이 none이면 그냥 아무것도 안함
                    return;
                case CommonClass.ZoneType.PlayerHandZone:
                    {
                    bufferCard.reduction = false;
                    bufferCard.isFront = true;
                        playerHandCards.Add(bufferCard);
                        ArrangeCardAsFan(playerHandCards, PlayerHandCardLeft, PlayerHandCardRight);
                    }
                    break;
                case CommonClass.ZoneType.PlayerBlockZone:
                    {
                    bufferCard.reduction = true;
                    bufferCard.isFront = true;
                    playerBlockCards.Add(bufferCard);
                        ArrangeCardAsList(playerBlockCards, PlayerBlockCardLeft, PlayerBlockCardRight);
                    }
                    break;
                case CommonClass.ZoneType.PlayerDeckZone:
                    {
                        bufferCard.selected = false;
                    bufferCard.isFront = true;
                    bufferCard.clickAble = false;
                        DOTween.Kill(bufferCard.transform);
                        bufferCard.transform.localScale = Vector3.one;
                        bufferCard.transform.DOScale(Vector3.zero, 0.7f);
                        bufferCard.transform.DOMove(deckIcon.transform.position, 0.7f)
                            .OnComplete(() => Destroy(bufferCard.gameObject));
                    }
                    break;
                case CommonClass.ZoneType.PlayerGraveZone:
                    {
                        bufferCard.selected = false;
                    bufferCard.isFront = true;
                    bufferCard.clickAble = false;
                        DOTween.Kill(bufferCard.transform);
                        bufferCard.transform.localScale = Vector3.one;
                        bufferCard.transform.DOScale(Vector3.zero, 0.7f);
                        bufferCard.transform.DOMove(graveIcon.transform.position, 0.7f)
                            .OnComplete(() => Destroy(bufferCard.gameObject));
                    }
                    break;
                case CommonClass.ZoneType.EnemyHandZone:
                    {
                    bufferCard.reduction = false;
                    bufferCard.isFront = false;
                    bufferCard.clickAble = false;
                        enemyHandCards.Add(bufferCard);
                        ArrangeCardAsFan(enemyHandCards, enemyHandCardLeft, enemyHandCardRight);
                    }
                    break;
                case CommonClass.ZoneType.EnemyBlockZone:
                    {
                    bufferCard.reduction = true;
                    bufferCard.isFront = false;
                    bufferCard.clickAble = false;
                    enemyBlockCards.Add(bufferCard);
                        ArrangeCardAsList(enemyBlockCards, enemyBlockCardLeft, enemyBlockCardRight);
                    }
                    break;
                case CommonClass.ZoneType.EnemyDeckZone:
                    {
                    bufferCard.clickAble = false;
                    bufferCard.isFront = false;
                    bufferCard.selected = false;
                        bufferCard.clickAble = false;
                        DOTween.Kill(bufferCard.transform);
                        bufferCard.transform.localScale = Vector3.one;
                        bufferCard.transform.DOScale(Vector3.zero, 0.7f);
                        bufferCard.transform.DOMove(enemyDeckIcon.transform.position, 0.7f)
                            .OnComplete(() => Destroy(bufferCard.gameObject));
                    }
                    break;
                case CommonClass.ZoneType.EnemyGraveZone:
                    {
                    bufferCard.clickAble = false;
                    bufferCard.isFront = false;
                    bufferCard.selected = false;
                        bufferCard.clickAble = false;
                        DOTween.Kill(bufferCard.transform);
                        bufferCard.transform.localScale = Vector3.one;
                        bufferCard.transform.DOScale(Vector3.zero, 0.7f);
                        bufferCard.transform.DOMove(enemyGraveIcon.transform.position, 0.7f)
                            .OnComplete(() => Destroy(bufferCard.gameObject));
                    }
                break;
            case CommonClass.ZoneType.PlayerAttackZone:
                {
                    bufferCard.reduction = true;
                    bufferCard.isFront = true;
                    bufferCard.selected = false;
                    bufferCard.clickAble = false;
                    bufferCard.transform.DOScale(Vector3.one, 0.7f);
                    bufferCard.transform.DORotate(new Vector3(0, 0, 0), 0.7f);
                    bufferCard.transform.DOMove(PlayerAttackZone.transform.position, 0.7f);
                    attackCard= bufferCard;
                }
                    break;
            case CommonClass.ZoneType.EnemyAttackZone:
                {
                    bufferCard.reduction = true;
                    bufferCard.isFront = true;
                    bufferCard.selected = false;
                    bufferCard.clickAble = false;
                    bufferCard.transform.DOScale(Vector3.one, 0.7f);
                    bufferCard.transform.DORotate(new Vector3(0, 0, 0), 0.7f);
                    bufferCard.transform.DOMove(EnemyAttackZone.transform.position, 0.7f);
                    attackCard = bufferCard;
                }
                break;
        }
        
    }
    void ArrangeCardAsFan(List<CardView> cards, RectTransform left, RectTransform right)
    {
        if (cards.Count == 0) return;

        float totalAngle = 30f; // 끝 카드의 최대 기울기 (도)

        for (int i = 0; i < cards.Count; i++)
        {
            float t = cards.Count == 1 ? 0.5f : (float)i / (cards.Count - 1);

            // x는 left~right 사이에 균등 분배, y는 고정
            Vector3 newPos = Vector3.Lerp(left.position, right.position, t);

            // 가운데 기준으로 -1 ~ 1 범위
            float normalizedT = t * 2f - 1f;
            float angle = -normalizedT * totalAngle; // 왼쪽+, 오른쪽-

            cards[i].transform.DOMove(newPos, 0.5f);
            cards[i].transform.DORotate(new Vector3(0f, 0f, angle), 0.5f);
        }
    }
    void ArrangeCardAsList(List<CardView> cards, RectTransform left, RectTransform right)
    {
        if (cards.Count == 0) return;

        for (int i = 0; i < cards.Count; i++)
        {
            float t = cards.Count == 1 ? 0.5f : (float)i / (cards.Count - 1);

            // x는 left~right 사이에 균등 분배, y는 고정
            Vector3 newPos = Vector3.Lerp(left.position, right.position, t);

            // 가운데 기준으로 -1 ~ 1 범위
            float normalizedT = t * 2f - 1f;
        

            cards[i].transform.DOMove(newPos, 0.5f);
            cards[i].transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f);
        }
    }




    public Vector3 GetWorldPositionFromUI(RectTransform uiElement)//캔버스 좌표를 주면 월드 좌표상 어디로 가야 거기로 가는 것 처럼 보이는지 알려준다.
    {
        // Overlay는 스크린 좌표 = UI 좌표 그대로
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null,uiElement.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }
    void e_CardLeftClicked(EventBus.CardLeftClickedEvent e)//이벤트 버스에서 구독함.invoke() 하면 실행됨 연출 관련 해서만 적어두자.
    {
        if (e.card == null||(fsmManager.GetCurrState() is not PlayerMainPhaseState && fsmManager.GetCurrState() is not PlayerSettingBlockPhaseState)) return;

        
        foreach (var card in playerHandCards)//플레이어 손패 카드가 눌린건지 확인을 위해 반복문 돌림
        {
            if (card == e.card)//여기 걸리면 손패 카드가 눌린것이 맞음
            {
                if (currSelectedCard != e.card)//눌린 카드가 새로운 카드일시.
                {
                    if (currSelectedCard != null) currSelectedCard.selected = false;//이전 카드는 선택을 안하도록 처리
                }
                currSelectedCard = e.card;//선택카드 갱신
                e.card.selected = true;//눌린 카드는 선택했다 처리.
                return;
            }
        }

       
    }
    void e_CardRightClicked(EventBus.CardRightClickedEvent e)
    {
        if (e.card == null) return;

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
    void e_UpdateAllGraveViewer(EventBus.UpdateGrave e)//무덤 창 업데이트. 정렬은 안함
    {
        GameObject makedCard;


        if (graveContent==null)
        {
            Debug.Log("뭔가 잘못됨");
            return;
        }

        for(int i=graveContent.childCount-1  ;  i>=0  ;  i--)//기존에 있던 카드들을 제거
        {
            Destroy(graveContent.GetChild(i).gameObject);
        }

        foreach (var item in e.cards)//카드 데이터 메니저를 참고해서 새로 만든다. 이건 이벤트 버스로 데이터를 받는것으로 수정예정
        {
           makedCard = Instantiate(viwerCard, graveContent);

            makedCard.GetComponent<CardView>()?.Init( item);
          
        }



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

    void e_UpdateAllDeckViewer(EventBus.UpdateDeck e)//덱 창 업데이트. 정렬은 안함
    {
        GameObject makedCard;


        if ( deckContent == null)
        {
            Debug.Log("뭔가 잘못됨");
            return;
        }

        for (int i = deckContent.childCount - 1; i >= 0; i--)//기존에 있던 카드들을 제거
        {
            Destroy(deckContent.GetChild(i).gameObject);
        }

        foreach (var item in e.cards)//카드 데이터 메니저를 참고해서 새로 만든다. 이건 이벤트 버스로 데이터를 받는것으로 수정예정
        {
            makedCard = Instantiate(viwerCard, deckContent);

            makedCard.GetComponent<CardView>()?.Init( item);
        }

    }
  

   

}
