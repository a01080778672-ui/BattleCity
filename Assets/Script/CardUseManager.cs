using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CardUseManager : MonoBehaviour //카드의 사용 또는 피치를 당담하는 매니저이다.
{
    [SerializeField] CardDB cardDB;// start()에서 카드를 시작할때 아무거나 만들어주기 위해서 넣었음 지울수도 있음
    [SerializeField]FSMManager fsmManager;
    [SerializeField]GameLoopData _data;
    [SerializeField] int MaxPlayerCapacityBlockCardNumber = 4;

    List<CardInstance> currSelectedBlockCards;


    CardInstance currSelectedCard;
    private void Awake()
    {
        currSelectedBlockCards=new List<CardInstance>();
    }

    private void Start()
    {
        int j = 1;
        for (int i = 0; i < 4; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;
        for (int i = 0; i < 2; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;
        for (int i = 0; i < 2; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;
        for (int i = 0; i < 2; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;
        for (int i = 0; i < 2; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;
        for (int i = 0; i < 1; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;
        for (int i = 0; i < 1; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;

        for (int i = 0; i < 4; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));
        }
        j++;

        for (int i = 0; i < 2; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(j)));
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(j)));//8
        }


    }

    private void OnEnable()
    {
       
        EventBus.Subscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);
        EventBus.Subscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);
        EventBus.Subscribe<EventBus.CheckSelectedCardLive>(e_CheckSelectCard);
        EventBus.Subscribe<EventBus.BlockButtonClicked>(e_TryPlayerUseBlockCard);
        EventBus.Subscribe<EventBus.PlayerAttackSuccess>(e_PlayerSuccessAttack);
        EventBus.Subscribe<EventBus.EnemyAttackSuccess>(e_EnemySuccessAttack);

        EventBus.Subscribe<EventBus.BlockButtonClicked>(e_TryPlayerUseBlockCard); // 임시 추가 0609김종호

        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMchanged);
    }

    private void OnDisable()
    {
    
        EventBus.Unsubscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);
        EventBus.Unsubscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);
        EventBus.Unsubscribe<EventBus.CheckSelectedCardLive>(e_CheckSelectCard);
        EventBus.Unsubscribe<EventBus.BlockButtonClicked>(e_TryPlayerUseBlockCard);
        EventBus.Unsubscribe<EventBus.PlayerAttackSuccess>(e_PlayerSuccessAttack);
        EventBus.Unsubscribe<EventBus.EnemyAttackSuccess>(e_EnemySuccessAttack);

        EventBus.Unsubscribe<EventBus.BlockButtonClicked>(e_TryPlayerUseBlockCard); // 임시 추가 0609김종호

        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMchanged);
    }

    private void Update() // 임시로 추가함 0609김종호
    {
        if (Input.GetKeyDown(KeyCode.Space)) { 
            if(fsmManager.GetCurrState() is PlayerTryBlockPhaseState)
            {
                EventBus.Publish(new EventBus.AlarmText { alarmText = "방어 카드 사용 확정 버튼을 눌렀습니다." });
                EventBus.Publish(new EventBus.BlockButtonClicked { });
            }
        }
    }

    void e_FSMchanged(EventBus.FSMChanged e)
    {
        currSelectedBlockCards.Clear();
        currSelectedCard = null;
    }

    void e_CheckSelectCard(EventBus.CheckSelectedCardLive e)//카드 버렸는데 그게 선택중이었던 카드면 없에야함
    {
        foreach (CardInstance item in _data.player.HandCards)
        {
            if(item==currSelectedCard)
            {
                return;//손패에 선택한 카드가 있으면 그냥 패스
            }
        }
        
        currSelectedCard = null;//손패에 선택한 카드가 없으므로 null로 변경
    }
    
    void e_CardLeftClicked(EventBus.CardLeftClickedEvent e)
    {

        if(fsmManager.GetCurrState()is not PlayerMainPhaseState&& fsmManager.GetCurrState() is not PlayerTryBlockPhaseState && fsmManager.GetCurrState() is not PlayerSettingBlockPhaseState)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "카드를 쓸수 있는 페이즈 아님" });
            return;//플레이어가 카드를 만질 이유가 없는 상태이면 return하는게 맞을듯함.
        }


        if (fsmManager.GetCurrState() is PlayerTryBlockPhaseState) // 임시 추가 0609김종호
        {
            foreach (var item1 in _data.player.BlockCards)//방어존에 있던 카드라면
            {

                if (item1 == e.card.cardInstance)
                {
                    if (!currSelectedBlockCards.Contains(item1))
                    {
                        currSelectedBlockCards.Add(item1);
                        e.card.selected = true;
                        EventBus.Publish(new EventBus.AlarmText { alarmText = "방어카드 선택함" });

                    }
                    else
                    {
                        currSelectedBlockCards.Remove(item1);
                        e.card.selected = false;
                        EventBus.Publish(new EventBus.AlarmText { alarmText = "방어카드 선택취소함" });
                    }
                }
         
            }

        }
        else
        {
            foreach (var item in _data.player.HandCards)//카드를 쓰게 될텐데 손패에 있던 카드라면
            {
                if (e.card.cardInstance != currSelectedCard)//눌린 카드가 선택 카드가 아니라면
                {
                
                    currSelectedCard = e.card.cardInstance;// 선택됐던 카드가 아니면 이 카드를 선택하는것으로 마무리함
                
                    return;
                }


                else if (item == currSelectedCard)
                {
                    if (item.CardDataSO.type != CardDataSO.CardType.Block)
                    {
                        if (fsmManager.GetCurrState() is PlayerMainPhaseState)
                            TryPlayerUseAttackCard(e.card.cardInstance);//카드를 쓰려고 하겠다 공격으로

                        if (fsmManager.GetCurrState() is PlayerSettingBlockPhaseState&& _data.player.BlockCards.Count < MaxPlayerCapacityBlockCardNumber)//방어 세팅 페이즈에선 공격카드역시 방어존으로 갑니다
                            EventBus.Publish(new EventBus.RequestRelocateCard { card = item, to = CommonClass.ZoneType.PlayerBlockZone });//카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기


                        return;
                    }
                    else if (_data.player.BlockCards.Count < MaxPlayerCapacityBlockCardNumber)
                    {
                        if (fsmManager.GetCurrState() is PlayerMainPhaseState || fsmManager.GetCurrState() is PlayerSettingBlockPhaseState)
                            EventBus.Publish(new EventBus.RequestRelocateCard { card = item, to = CommonClass.ZoneType.PlayerBlockZone });//카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기






                        return;
                    }
                }
            }


        }


          
        
      
    }
    void e_CardRightClicked(EventBus.CardRightClickedEvent e)
    {
       if(fsmManager.GetCurrState() is not PlayerMainPhaseState)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "피치할수 있는 페이즈아님" });
            return;
        }
       foreach (var item in _data.player.HandCards)//피치는 손패에서만 가능하므로 이렇게 검사
       {
            if (e.card.cardInstance == currSelectedCard && item == e.card.cardInstance)
            {
                _data.player.currEnergy += 1;
                // 우클릭 = 카드 버리고 에너지 +1


                EventBus.Publish(new EventBus.RequestRelocateCard { card = e.card.cardInstance, to = CommonClass.ZoneType.PlayerGraveZone });//카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기
                return;
            }
        }
        
    }



    //방어 버튼이 눌리면 이 함수가 발동 되도록 하는것 역시 해야 함.
    void e_TryPlayerUseBlockCard(EventBus.BlockButtonClicked e)//방어카드를 사용하는것을 판단하고 적중 처리도 있을수도 그 카드를 무덤으로, 
    {
        //방어 버튼이 눌리면 이벤트 버스로부터 실행될 함수이다. 이 함수가 실행되면 (연출?)이후 이너미 메인 페이즈로 넘어가게될 가능성 매우 높음!!!
        //List<CardInstance> currSelectedBlockCards; 라는 아직은 아무것도 안하는 멤버 함수가 있는데 이걸 이용할수도 있고 아닐수도 있을것 같음. 
        //List<CardInstance> currSelectedBlockCards; 를 만든 의도는 플레이어 방어 시도 페이즈가 시작되면 우선 싹다 비우고,
        //플레이어가 방어존에서 카드를 선택하면 거기에 넣고 취소하면 그 카드만 빼는 식으로 플레이어가 선택한 방어 카드들을 넣습니다.
        //그리고 그렇게 저장된 방어카드들을 쓰는 것을 확정 시키면 될듯함.
        //혹시 모르니 이 함수가 끝났을때 List<CardInstance> currSelectedBlockCards 를 다시 지우기.
        //사용한 카드는 이벤트 버스의 카드 이동을 써서 무덤으로 보내기

       



        if (_data == null || _data.currAttackCard == null) return;
        if (fsmManager.GetCurrState() is not PlayerTryBlockPhaseState) return;

        int blockScore = 0;//방어력 총합
        int cost = 0;//코스트 총합

        foreach (var cardInstance in currSelectedBlockCards) // 임시 추가 0609 김종호
        {
            blockScore += cardInstance.CardDataSO.blockPower;//반복문 돌기.

            if(cardInstance.CardDataSO.type is CardDataSO.CardType.Block)
            cost += cardInstance.CardDataSO.cardCost[0].cost;//방어카드의 경우에만 코스트를 합산해 구한다.
        }
   
    
     

 
       if (_data.player.currEnergy < cost)
       {
           EventBus.Publish(new EventBus.AlarmText
           {
               alarmText = "코스트가 부족하여 카드를 쓸수 없다!"
           });
           return;
       }

        //여기 밑으로 가면 코스트는 충분함
       _data.player.currEnergy -= cost;//감소처리

       

        


        foreach (var cardInstance in currSelectedBlockCards)//현재 선택된 방어 카드들로 반복문을 돌립니다.
        {
            CardContext cardContext=new CardContext { usedCard=cardInstance,fsmState=fsmManager.GetCurrState(),usedEntity=_data.player,targetEntity=_data.enemy};

            //반복문을 돌려 단일 카드 방어 요청을 여러번 터트립니다.

            if (cardInstance.CardDataSO.type == CardDataSO.CardType.Block)
            {
                blockScore += cardInstance.CardDataSO.blockPower;//그 카드의 방어력만큼 방어력 업
            }
            else
            {
                blockScore += 1;//방어 카드 아니면 무조껀 1만 증가
            }
           // EventBus.Publish(new EventBus.RequestRelocateCard { card = cardInstance, to = CommonClass.ZoneType.PlayerBlockZone });//카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기

            if(cardInstance.CardDataSO.type is CardDataSO.CardType.Block)
            UseCard(cardContext);//방어카드 타입으로 온전히 방어라면 그 카드 효과도 터트려야함
        }



        if (_data.currAttackCard.CardDataSO.power <= blockScore)
        {
            //방어 성공
            EventBus.Publish(new EventBus.AlarmText { alarmText = $"방어 성공" }); //임시 추가 0609김종호
        }
        else
        {
            //방어 실패
            EventBus.Publish(new EventBus.AlarmText { alarmText = $"방어 실패" }); // 임시 추가 0609김종호
   
            EventBus.Publish(new EventBus.EnemyAttackSuccess { });
            //EventBus.Publish(new EventBus.RequestPlayerDamage { damage = _data.currAttackCard.CardDataSO.power });
        }

        foreach(var cardInstance in currSelectedBlockCards) // 임시추가 0609김종호
        {
            EventBus.Publish(new EventBus.RequestRelocateCard { card = cardInstance, to = CommonClass.ZoneType.PlayerGraveZone });//방어 선택 카드들을 싹 플레이어 무덤으로 
        }
        currSelectedBlockCards.Clear();//방어 선택 카드들을 다시 싹다 비웁니다
        EventBus.Publish(new EventBus.RequestRelocateCard { card = _data.currAttackCard, to = CommonClass.ZoneType.EnemyGraveZone });//공격 존의 카드도 적 무덤존으로.


        Debug.Log(currSelectedBlockCards.Count);
        Debug.Log($"방어존{_data.player.BlockCards.Count}");

        EventBus.Publish(new EventBus.StartEnemyMainPhaseEvent{ }); // 방어 끝나면 다시 이너미 턴 상태로 갑니다.
    }

    void e_EnemySuccessAttack(EventBus.EnemyAttackSuccess e)
    {
        CardContext context = new CardContext { usedCard = _data.currAttackCard, usedEntity = _data.enemy, targetEntity = _data.player };

        UseCard(context);

    }
    void e_PlayerSuccessAttack(EventBus.PlayerAttackSuccess e)
    {
        CardContext context = new CardContext { usedCard = _data.currAttackCard, usedEntity = _data.player, targetEntity = _data.enemy };

        UseCard(context);

    }

    void TryPlayerUseAttackCard(CardInstance card)//공격 카드를 사용하는것을 판단
    {
        if (fsmManager?.GetCurrState() is not PlayerMainPhaseState)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "지금은 공격카드 쓸 턴이 아니다." });
            return;
        }


        int cost = card.CardDataSO.cardCost[0].cost;
  
        if (_data.player.currEnergy < cost)
        {
            EventBus.Publish(new EventBus.AlarmText
            {
                alarmText = "코스트가 부족하여 카드를 쓸수 없다!"
            });
            return;
        }
        
        _data.player.currEnergy -= card.CardDataSO.cardCost[0].cost;

        //여기 밑으로 가면 에너지는 충분

      


        EventBus.Publish(new EventBus.RequestRelocateCard { card = card, to = CommonClass.ZoneType.PlayerAttackZone });
        //카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기
        //카드를 써서 공격을 시도하는데 적이 방어여부를 고려하고 다시 플레이어 메인턴으로 가게될듯함.

        EventBus.Publish(new EventBus.StartEnemyTryBlockPhaseEvent { });//그러니까 이 이벤트가 필요하다.



        currSelectedCard = null;
    }


    void UseCard(CardContext useontext)
    {
        CardContext context = useontext;

        if (context.usedCard == null) return;

        if (context.usedCard.CardDataSO.type is CardDataSO.CardType.Attack)
        {
            context.targetEntity.currHp -= context.usedCard.CardDataSO.attack;
        }


        foreach (var effect in context.usedCard.CardDataSO.hitEffects)
        {
            bool pass = true;
            foreach (var item in effect.conditions)
            {
                if (item.Evaluate(context) == false)
                { pass = false; break; }

            }
            if (pass)
            {

                effect.effects.Execute(context);//조건이 맞다면 그  효과를  수행합니다.
            }

        }



     

    }


    public int CheckCurrSelectedBlockCard()
    {
        return currSelectedBlockCards.Count;
    }
    public bool CheckBlockSuccess()
    {
    if (_data == null || _data.currAttackCard == null) return false;
    if (fsmManager.GetCurrState() is not PlayerTryBlockPhaseState) return false;
    
        int blockScore = 0;//방어력 총합

        foreach (var cardInstance in currSelectedBlockCards)
        {
         blockScore += cardInstance.CardDataSO.blockPower;
        }
        return _data.currAttackCard.CardDataSO.power <= blockScore;
    }

}
