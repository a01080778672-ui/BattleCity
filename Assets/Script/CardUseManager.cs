using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static UnityEditor.Progress;

public class CardUseManager : MonoBehaviour //카드의 사용 또는 피치를 당담하는 매니저이다.
{
    [SerializeField] CardDB cardDB;// start()에서 카드를 시작할때 아무거나 만들어주기 위해서 넣었음 지울수도 있음
    [SerializeField]FSMManager fsmManager;
    [SerializeField]GameLoopData _data;


    List<CardInstance> currSelectedBlockCards;


    CardInstance currSelectedCard;
    private void Awake()
    {
        currSelectedBlockCards=new List<CardInstance>();
    }

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            _data.AddPlayerDeckCard(new CardInstance(cardDB.GetCardSO(i)));
        }
        for (int i = 0; i < 20; i++)
        {
            _data.AddEnemyDeckCard(new CardInstance(cardDB.GetCardSO(i)));
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
    }

    private void OnDisable()
    {
    
        EventBus.Unsubscribe<EventBus.CardLeftClickedEvent>(e_CardLeftClicked);
        EventBus.Unsubscribe<EventBus.CardRightClickedEvent>(e_CardRightClicked);
        EventBus.Unsubscribe<EventBus.CheckSelectedCardLive>(e_CheckSelectCard);
        EventBus.Unsubscribe<EventBus.BlockButtonClicked>(e_TryPlayerUseBlockCard);
        EventBus.Unsubscribe<EventBus.PlayerAttackSuccess>(e_PlayerSuccessAttack);
        EventBus.Unsubscribe<EventBus.EnemyAttackSuccess>(e_EnemySuccessAttack);


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

        if (e.card.cardInstance != currSelectedCard)//눌린 카드가 선택 카드가 아니라면
        {
            currSelectedCard = e.card.cardInstance;// 선택됐던 카드가 아니면 이 카드를 선택하는것으로 마무리함
            return;
        }
        
            foreach (var item in _data.player.HandCards)//카드를 쓰게 될텐데 손패에 있던 카드라면
            {
                if (item == currSelectedCard)
                {
                    if (item.CardDataSO.type != CardDataSO.CardType.Block)
                    {
                        TryPlayerUseAttackCard( e.card.cardInstance);//카드를 쓰려고 하겠다 공격으로
                    return;
                    }
                    else if (_data.player.BlockCards.Count < 5)
                    {
                    EventBus.Publish(new EventBus.RequestRelocateCard { card = item,to=CommonClass.ZoneType.PlayerBlockZone });//카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기
                    return;
                }
                }
            }

            foreach (var item in _data.player.BlockCards)//방어존에 있던 카드라면
            {
                //방어존에서 선택이 아니였던 카드라면 방어존 현재 선택 카드 리스트에 추가합니다
                //방어존에서 선택됐던 카드라면 방어존 현재 선택 카드 리스트에서 없엡니다
                if (item == currSelectedCard )
                {
                   
                   
                    return;
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
                // 우클릭 = 카드 버리고 에너지 +1
                EventBus.Publish(new EventBus.RequestUsePlayerEnergy
                {
                    energy = -1  // 음수면 에너지 증가
                });
                EventBus.Publish(new EventBus.RequestRelocateCard { card = e.card.cardInstance, to = CommonClass.ZoneType.PlayerGraveZone });//카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기
                return;
            }
        }
        
    }
    void e_TryPlayerUseBlockCard(EventBus.BlockButtonClicked e)//방어카드를 사용하는것을 판단하고 적중 처리도 있을수도 그 카드를 무덤으로, 
    {
        if (_data == null || _data.currAttackCard == null) return;


        int blockScore = 0;//방어력 총합
        /*
       int cost = 0;
       foreach (var cardContext in currSelectedBlockCards)
       {
           cost += card.usedCard.CardDataSO.cardCost[0].cost;//쓸 카드들을 전부 합해 코스트를 구한다.
           방어 카드는 현재 코스트를 사용하지 않음
       }


       /*
       if (_data.currPlayerEnergy < cost)
       {
           EventBus.Publish(new EventBus.AlarmText
           {
               alarmText = "코스트가 부족하여 카드를 쓸수 없다!"
           });
           return;
       }

        //여기 밑으로 가면 코스트는 충분함
        EventBus.Publish(new EventBus.RequsetUsePlayerEnergy
        {
            energy = cost;//코스트 소모
            //방어카드는 현재 코스트를 소모하지 않음
        });*/


        foreach (var cardInstance in currSelectedBlockCards)
        {
            foreach (var effect in cardInstance.CardDataSO.hitEffects)
            {
                //effect.Execute(card);//방어로 쓰는거면 효과를 따로 안 발동합니다.
                //추후 타입: "방어카드"에 한정해서 효과를 발동시켜야합니다
            }


        }


        foreach (var cardInstance in currSelectedBlockCards)//현재 선택된 방어 카드들로 반복문을 돌립니다.
        {
            CardContext cardContext=new CardContext { usedCard=cardInstance};

            //반복문을 돌려 단일 카드 방어 요청을 여러번 터트립니다.

            if (cardInstance.CardDataSO.type == CardDataSO.CardType.Block)
            {
                blockScore += cardInstance.CardDataSO.blockPower;//그 카드의 방어력만큼 방어력 업
            }
            else
            {
                blockScore += 1;//방어 카드 아니면 무조껀 1만 증가
            }
            EventBus.Publish(new EventBus.RequestRelocateCard { card = cardInstance, to = CommonClass.ZoneType.PlayerBlockZone });//카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기
         
        }

        currSelectedBlockCards.Clear();//방어했으니 선택한 방어카드는 싹 비워야함

        if (_data.currAttackCard.CardDataSO.power <= blockScore)
        {
            //방어 성공
        }
        else
        {
            //방어 실패
        }


       

    }

    void e_EnemySuccessAttack(EventBus.EnemyAttackSuccess e)
    {
        CardContext context=new CardContext { usedCard=_data.currAttackCard,usedEntity=_data.enemy,targetEntity=_data.player};

        if (context.usedCard == null) return;

        foreach (var effect in context.usedCard.CardDataSO.hitEffects)
        {
            effect.Execute(context);//그 카드가 가진 효과를 전부 수행합니다.
        }

    }
    void e_PlayerSuccessAttack(EventBus.PlayerAttackSuccess e)
    {
        CardContext context = new CardContext { usedCard = _data.currAttackCard, usedEntity = _data.player, targetEntity = _data.enemy };

        if (context.usedCard == null) return;

        foreach (var effect in context.usedCard.CardDataSO.hitEffects)
      {
          effect.Execute(context);//그 카드가 가진 효과를 전부 수행합니다.
      }
     
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
        


        //여기 밑으로 가면 코스트는 충분함
        EventBus.Publish(new EventBus.RequestUsePlayerEnergy
        {
            energy = card.CardDataSO.cardCost[0].cost//코스트 소모
        });

      


        EventBus.Publish(new EventBus.RequestRelocateCard { card = card, to = CommonClass.ZoneType.AttackZone });
        //카드 스왑 매니저에 있는 카드 이동 시키기 이벤트 터트리기
        //카드를 써서 공격을 시도하는데 적이 방어여부를 고려하고 다시 플레이어 메인턴으로 가게될듯함.

        EventBus.Publish(new EventBus.StartEnemyTryBlockPhaseEvent { });//그러니까 이 이벤트가 필요하다.



        currSelectedCard = null;
    }






}
