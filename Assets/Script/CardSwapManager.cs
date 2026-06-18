using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class CardSwapManager : MonoBehaviour //카드를 덱, 무덤, 손패 에서 여기 옮기고 저기 옮기고 하는 클래스이다.
{
    [SerializeField] GameLoopData _data;




    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMChanged);

        EventBus.Subscribe<EventBus.StartPlayerMainPhaseEvent>(e_PlayerMainPhaseStarted);
        EventBus.Subscribe<EventBus.StartEnemyMainPhaseEvent>(e_EnemyMainPhaseStarted);
        EventBus.Subscribe<EventBus.RequestRelocateCard>(e_RequestRelocateCard);


        EventBus.Subscribe<EventBus.StartInitPhaseEvent>(e_StartInit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMChanged);

        EventBus.Unsubscribe<EventBus.StartPlayerMainPhaseEvent>(e_PlayerMainPhaseStarted);
        EventBus.Unsubscribe<EventBus.StartEnemyMainPhaseEvent>(e_EnemyMainPhaseStarted);

        EventBus.Unsubscribe<EventBus.RequestRelocateCard>(e_RequestRelocateCard);

        EventBus.Unsubscribe<EventBus.StartInitPhaseEvent>(e_StartInit);
    }

    void e_StartInit(EventBus.StartInitPhaseEvent e)
    {
        PlayerDrawCards(5);
        EnemyDrawCards(5);
    }
    void e_PlayerMainPhaseStarted(EventBus.StartPlayerMainPhaseEvent e)
    {
        if(_data.currAttackCard!=null)
        _data.SetReLocationCard(_data.currAttackCard,CommonClass.ZoneType.PlayerGraveZone);//플레이어 메인 페이즈 시작시 공격존에 있는 카드가 있다면 옮김
    }
    void e_EnemyMainPhaseStarted(EventBus.StartEnemyMainPhaseEvent e)
    {
        if (_data.currAttackCard != null)
            _data.SetReLocationCard(_data.currAttackCard, CommonClass.ZoneType.EnemyGraveZone);//플레이어 메인 페이즈 시작시 공격존에 있는 카드가 있다면 옮김
    }

    void e_FSMChanged(EventBus.FSMChanged e)
    {
        if(((e.prev is PlayerMainPhaseState||e.prev is PlayerSettingBlockPhaseState)&&e.curr is EnemyMainPhaseState))
        {
            PlayerDrawCards(5);
        }
        else if(( e.prev is EnemyMainPhaseState || e.prev is EnemySettingBlockPhaseState) && e.curr is PlayerMainPhaseState )
        {
            EnemyDrawCards(5);
        }
    }


    
    void MoveAllHandCardToGrave()
    {
  

        while(_data.player.HandCards.Count > 0)
        {
            _data.SetReLocationCard(_data.player.HandCards[0], CommonClass.ZoneType.PlayerHandZone, CommonClass.ZoneType.PlayerGraveZone);
        }

       
    }





    void EnemyDrawCards(int number)
    {


        int leftCard = number - _data.enemy.HandCards.Count;

        if (_data.enemy.DeckCards.Count < number)
        {
            foreach (var item in _data.enemy.DeckCards.ToList())//복사본을 순회하며, 일단 뽑아야할 카드보다 덱 카드가 적으니 전부 뽑아냅니다
            {
                _data.SetReLocationCard(item, CommonClass.ZoneType.EnemyDeckZone, CommonClass.ZoneType.EnemyHandZone);
                leftCard--;
            }

            foreach (var item in _data.enemy.GraveCards.ToList())//더 뽑을게 없으니 무덤카드를 전부 덱 카드로 
            {
                _data.SetReLocationCard(item, CommonClass.ZoneType.EnemyGraveZone, CommonClass.ZoneType.EnemyDeckZone);
            }
            if (_data.enemy.DeckCards.Count < number)
            {
                Debug.LogWarning("덱 카드가 너무 적어서 뽑을수가 없는거같음");
                return;
            }

            for (int i = 0; i < leftCard; i++)
            {
                int randomInt = UnityEngine.Random.Range(0, _data.enemy.DeckCards.Count);

                _data.SetReLocationCard(_data.enemy.DeckCards[randomInt], CommonClass.ZoneType.EnemyGraveZone, CommonClass.ZoneType.EnemyHandZone);
            }
        }
        else
        {

            for (int i = 0; i < leftCard; i++)
            {
                int randomInt = UnityEngine.Random.Range(0, _data.enemy.DeckCards.Count);
                _data.SetReLocationCard(_data.enemy.DeckCards[randomInt], CommonClass.ZoneType.EnemyDeckZone, CommonClass.ZoneType.EnemyHandZone);

            }
        }
    }


    void PlayerDrawCards(int number)
    {

        int leftCard = number - _data.player.HandCards.Count;

        if (_data.player.DeckCards.Count < number)
        {
            foreach (var item in _data.player.DeckCards.ToList())//복사본을 순회하며, 일단 뽑아야할 카드보다 덱 카드가 적으니 전부 뽑아냅니다
            {
                _data.SetReLocationCard(item, CommonClass.ZoneType.PlayerDeckZone, CommonClass.ZoneType.PlayerHandZone);
                leftCard--;
            }

            foreach (var item in _data.player.GraveCards.ToList())//더 뽑을게 없으니 무덤카드를 전부 덱 카드로 
            {
                _data.SetReLocationCard(item, CommonClass.ZoneType.PlayerGraveZone, CommonClass.ZoneType.PlayerDeckZone);
            }
            if (_data.player.DeckCards.Count < number)
            {
                Debug.LogWarning("덱 카드가 너무 적어서 뽑을수가 없는거같음");
                return;
            }

            for (int i = 0; i < leftCard; i++)
            {
                int randomInt = UnityEngine.Random.Range(0, _data.player.DeckCards.Count);

                _data.SetReLocationCard(_data.player.DeckCards[randomInt], CommonClass.ZoneType.PlayerDeckZone, CommonClass.ZoneType.PlayerHandZone);
            }
        }
        else
        {

            for (int i = 0; i < leftCard; i++)
            {
                int randomInt = UnityEngine.Random.Range(0, _data.player.DeckCards.Count);
                _data.SetReLocationCard(_data.player.DeckCards[randomInt], CommonClass.ZoneType.PlayerDeckZone, CommonClass.ZoneType.PlayerHandZone);

            }
        }
    }


    void e_RequestRelocateCard(EventBus.RequestRelocateCard e)
    {
        _data.SetReLocationCard(e.card, e.to);
    }

 

}
