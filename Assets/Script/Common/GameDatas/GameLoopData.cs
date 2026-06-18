using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameLoopData: MonoBehaviour 
    //게임 루프 중에 저장해야할 데이터들을 여기에 저장한다. 
    //player나 enemy의 카드 리스트를 직접 수정하는 함수를 여기가 아닌 밖에서 쓰지 않도록 해야할 듯 합니다.
    //그리고 카드를 추가하는 방법이 지금은 플레이어 덱에 추가하기, 적 덱에 추가하기 밖에 없도록 해야합니다
{

    [SerializeField] Slider playerHpBar;
    [SerializeField] TextMeshProUGUI playerHpText;
    [SerializeField] TextMeshProUGUI playerEnergyText;


    [SerializeField] Slider enemyHpBar;
    [SerializeField] TextMeshProUGUI enemyHpText;
    [SerializeField] TextMeshProUGUI enemyEnergyText;


    public int currTurn;
    public EntityInstance player { get; private set; }
    public EntityInstance enemy { get; private set; }
    public CardInstance currAttackCard;

    private void Awake()
    {
        player = new EntityInstance(playerHpBar,playerHpText,playerEnergyText);
        enemy = new EntityInstance(enemyHpBar,enemyHpText,enemyEnergyText);
        currTurn = 1;




    }

    public void AddPlayerDeckCard(CardInstance newCard)//플레이어에게 줍니다. 덱에 없었던 카드도 새롭게 추가합니다.
    {
        player.AddToDeck(newCard);
    }

    public void AddEnemyDeckCard(CardInstance newCard)//적에게 줍니다. 덱에 없었던 카드도 새롭게 추가합니다.
    {
        enemy.AddToDeck(newCard);
    }

    public void SetReLocationCard(CardInstance card, CommonClass.ZoneType from, CommonClass.ZoneType to)//카드의 위치를 옮깁니다. UI관련 이동 이벤트도 함께 터트립니다.
    {
        if (card == null) return;
        if(RemoveFromZone(card, from)==false)
        {
            Debug.LogWarning("카드 삭제 실패");
            Debug.LogWarning("카드 삭제 실패");

        }
        AddToZone(card, to);
        EventBus.Publish(new EventBus.RelocateCardUI { card = card, from = from, to = to });
        EventBus.Publish(new EventBus.CheckSelectedCardLive());
    }
    public void SetReLocationCard(CardInstance card, CommonClass.ZoneType to)//어디에 있는지 모르는 카드를 옮깁니다. UI관련 이동 이벤트도 함께 터트립니다.
    {
        if (card == null) return;

        CommonClass.ZoneType from = FindZone(card);//어디에 있는지 모르니 직접 어디있는지 찾아내야합니다.
        if (from == CommonClass.ZoneType.None)
        {
            Debug.LogWarning("카드를 어느 존에서도 찾지 못했음");
            return;
        }

        SetReLocationCard(card, from, to);
    }
    private bool RemoveFromZone(CardInstance card, CommonClass.ZoneType zone)
    {
        switch (zone)
        {
            case CommonClass.ZoneType.PlayerHandZone: return player.RemoveFromHand(card); 
            case CommonClass.ZoneType.PlayerBlockZone: return player.RemoveFromBlock(card); 
            case CommonClass.ZoneType.PlayerDeckZone: return player.RemoveFromDeck(card); 
            case CommonClass.ZoneType.PlayerGraveZone: return player.RemoveFromGrave(card); 
            case CommonClass.ZoneType.EnemyHandZone: return enemy.RemoveFromHand(card); 
            case CommonClass.ZoneType.EnemyBlockZone: return enemy.RemoveFromBlock(card); 
            case CommonClass.ZoneType.EnemyDeckZone: return enemy.RemoveFromDeck(card); 
            case CommonClass.ZoneType.EnemyGraveZone: return enemy.RemoveFromGrave(card);
            case CommonClass.ZoneType.PlayerAttackZone: currAttackCard = null; return true;
            case CommonClass.ZoneType.EnemyAttackZone: currAttackCard = null; return true;
        }
        return false;
    }

    private void AddToZone(CardInstance card, CommonClass.ZoneType zone)
    {
        switch (zone)
        {
            case CommonClass.ZoneType.PlayerHandZone: player.AddToHand(card); break;
            case CommonClass.ZoneType.PlayerBlockZone: player.AddToBlock(card); break;
            case CommonClass.ZoneType.PlayerDeckZone: player.AddToDeck(card); break;
            case CommonClass.ZoneType.PlayerGraveZone: player.AddToGrave(card); break;
            case CommonClass.ZoneType.EnemyHandZone: enemy.AddToHand(card); break;
            case CommonClass.ZoneType.EnemyBlockZone: enemy.AddToBlock(card); break;
            case CommonClass.ZoneType.EnemyDeckZone: enemy.AddToDeck(card); break;
            case CommonClass.ZoneType.EnemyGraveZone: enemy.AddToGrave(card); break;
            case CommonClass.ZoneType.PlayerAttackZone: currAttackCard = card;break;
            case CommonClass.ZoneType.EnemyAttackZone: currAttackCard = card; break;
        }
    }

    private CommonClass.ZoneType FindZone(CardInstance card)
    {
        if (player.HandCards.Contains(card)) return CommonClass.ZoneType.PlayerHandZone;
        if (player.BlockCards.Contains(card)) return CommonClass.ZoneType.PlayerBlockZone;
        if (player.DeckCards.Contains(card)) return CommonClass.ZoneType.PlayerDeckZone;
        if (player.GraveCards.Contains(card)) return CommonClass.ZoneType.PlayerGraveZone;
        if (enemy.HandCards.Contains(card)) return CommonClass.ZoneType.EnemyHandZone;
        if (enemy.BlockCards.Contains(card)) return CommonClass.ZoneType.EnemyBlockZone;
        if (enemy.DeckCards.Contains(card)) return CommonClass.ZoneType.EnemyDeckZone;
        if (enemy.GraveCards.Contains(card)) return CommonClass.ZoneType.EnemyGraveZone;
        if (currAttackCard == card) return CommonClass.ZoneType.PlayerAttackZone;
        if (currAttackCard == card) return CommonClass.ZoneType.EnemyAttackZone;
        return CommonClass.ZoneType.None;
    }

}