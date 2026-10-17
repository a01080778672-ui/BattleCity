using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CardDataManager : MonoBehaviour, IManager
{
    public class CardData
    {
        public List<CardInstance> graveCards = new List<CardInstance>();//무덤에 있는 카드 데이터 
        public List<CardInstance> handCards = new List<CardInstance>();//내가 현재 쓸수 있는 카드 데이터 
        public List<CardInstance> DeckCards = new List<CardInstance>();//덱에 있는 카드 데이터
        public List<CardInstance> BlockCards = new List<CardInstance>();//방어 대기 중인 카드 데이터
    }

    private int _currInstanceCardId=0;//사용할진 미지수 카드를 만들떄마다 1씩 증가하며, 오브젝트로써의 카드 아이디를 체크한다.



    public Action<List<CardInstance>> drawCards;   //카드 데이터 매니저가 카드를 드로우 했으니 보여주라고 지시하기 위한 이벤트 cardSetter쪽에서 구독을 한다.
    public Action<CardInstance> trashCard;//카드 하나를 무덤  보내라는 이벤트.


    public CardData currCardData { get; private set; } = new CardData();//현재 어떤 상태인지 여기서 제어
    public CardInstance currSelectedCard { get; private set; } = null;//아무것도 선택하지 않을시 null


    public void Register()
    {
        if (MasterManager.Instance != null)
        {
            MasterManager.Instance.RegisterManager<CardDataManager>(this);
        }
        else
        {
            StartCoroutine(Registering());
        }
    }
    IEnumerator Registering()
    {
        yield return null;
        if (MasterManager.Instance != null)
        {
            MasterManager.Instance.RegisterManager<CardDataManager>(this);
        }
    }
    private void Awake()
    {
        Register();
    }

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            currCardData.DeckCards.Add(new CardInstance(MasterManager.Instance.cardDB.GetCardSO(i), _currInstanceCardId++));
        }
    }

    private void OnEnable()//이벤트를 구독한다
    {
        EventBus.Subscribe<EventBus.EndPlayerTurnEvent>(e_EndPlayerTurn);

        EventBus.Subscribe<EventBus.StartPlayerTurnEvent>(e_PlayerTurnStarted);

        EventBus.Subscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);

        EventBus.Subscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);

    }
    private void OnDisable()//이벤트를 구독 해제한다
    {
        EventBus.Unsubscribe<EventBus.EndPlayerTurnEvent>(e_EndPlayerTurn);

        EventBus.Unsubscribe<EventBus.StartPlayerTurnEvent>(e_PlayerTurnStarted);

        EventBus.Unsubscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);

        EventBus.Unsubscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);
    }
    void e_PlayerTurnStarted(EventBus.StartPlayerTurnEvent e)
    {
      drawCards?.Invoke( DrawCards(5));
    }
    void e_EndPlayerTurn(EventBus.EndPlayerTurnEvent e)
    {

       MoveAllHandCardToGrave();




    }

    List<CardInstance> DrawCards(int number)
    {

        MoveAllHandCardToGrave();

        List<CardInstance> gettedCardID = new List<CardInstance>();
        int leftCard = number;

        if (currCardData.DeckCards.Count < number)//덱에 남은 카드가 뽑아야할 카드보다 더 적으면
        {
            foreach (var item in currCardData.DeckCards)
            {
                gettedCardID.Add(item);//일단 있는것들 다 뽑음
                leftCard--;
            }
            currCardData.DeckCards.Clear();//그리고 덱 에 든 카드들은 싹 지워도 됨.



            //그다음 무덤에 있는걸 싹 덱으로 옮겨야함
            foreach (var item in currCardData.graveCards)
            {
        
                currCardData.DeckCards.Add(item);
            }
            currCardData.graveCards.Clear();



            //그다음 거기서 랜덤으로 뽑아야함
            for(int i=0;i<leftCard;i++)
            {
                int randomInt= UnityEngine.Random.Range(0,currCardData.DeckCards.Count);
        
                gettedCardID.Add(currCardData.DeckCards[randomInt]);
                currCardData.DeckCards.RemoveAt(randomInt);
              
            }
        }
        else//덱에 남은 카드가 충분하다면
        {
            for (int i = 0; i < leftCard; i++)
            {
                int randomInt = UnityEngine.Random.Range(0, currCardData.DeckCards.Count);
                gettedCardID.Add(currCardData.DeckCards[randomInt]);
                currCardData.DeckCards.RemoveAt(randomInt);

            }
        }




        foreach (var item in gettedCardID)
        {
            currCardData.handCards.Add(item);
        }
            return gettedCardID;

    }

    void MoveAllHandCardToGrave()
    {
        Debug.Log("손카드무덤으로");
        foreach (var item in currCardData.handCards)
        {
            
            currCardData.graveCards.Add(item);
        }
        currCardData.handCards.Clear();
    }


   

    void e_CardLeftClicked(EventBus.CardLeftClickedEvent e)
    {
        if(e.card.cardInstance==currSelectedCard)//선택된 카드가 좌클릭이 됐다면? ->사용이 될듯?
        {
            CardDataSO currUsingCardSO = e.card.cardInstance.CardDataSO;
            GameDataManager gameDataManager = MasterManager.Instance.GetManager<GameDataManager>();//이거 일단 이렇게 만들면 안됨 수정해야함 여기서 조건 체크밑 효과 내면 안됨!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


            switch (currUsingCardSO.type)
            {
                case CardDataSO.CardType.Attack:
                    if (gameDataManager.currPlayerEnergy >= currUsingCardSO.cardCost[0].cost)//일단 제일 앞의 코스트 조건만 본다 수정해야함!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    {
                        gameDataManager.SetPlayerEnergy(gameDataManager.currPlayerEnergy- currUsingCardSO.cardCost[0].cost);//제일 앞 코스트 정보대로 일단 뻇어간다
                        gameDataManager.SetOtherHp(gameDataManager.currOtherHp - currUsingCardSO.attackPower);//이거 일단 이렇게 만들면 안됨 수정해야함. 여기서 조건 체크밑 효과 내면 안됨!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                       UseOneCard(e.card.cardInstance);//사용하면 무덤으로 가야함
                    }
                    else
                    {
                        EventBus.Publish<EventBus.AlarmText>(new EventBus.AlarmText {alarmText="코스트가 부족하여 카드를 쓸수 없다!" });
                    }

                        break;

                case CardDataSO.CardType.Block:

                    EventBus.Publish<EventBus.AlarmText>(new EventBus.AlarmText { alarmText = "방어카드는 아직미구현.." });
                    break;
            }



            
        }


        currSelectedCard = e.card.cardInstance;
    }

    void e_CardRightClicked(EventBus.CardRightClickedEvent e)
    {
        if(e.card.cardInstance==currSelectedCard)//선택된 카드가 우클릭이 됐다면?
        {
            GameDataManager dataManager = MasterManager.Instance.GetManager<GameDataManager>();
            dataManager.SetPlayerEnergy(dataManager.currPlayerEnergy + 1);
            UseOneCard(e.card.cardInstance);
        }
    }
    void UseOneCard(CardInstance useCard)//하나의 카드를 사용하기 위한 처리들을 담음. 무덤 옮기기 밑 cardSetter이벤트 invoke
    {
        trashCard?.Invoke(useCard);//cardSetter에게 이벤트 처리해서 그 카드만 무덤으로 보냄.
        this.currCardData.graveCards.Add(useCard);//무덤으로 이동
        this.currCardData.handCards.Remove(useCard);//선택된 카드는 손패에선
        if (currSelectedCard == useCard)
        {
            currSelectedCard = null;//선택된 카드가 만약에 이번에 삭제된 카드일시 null처리
        }
    }

}
