using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameLoopData;


public class GameLoopData : MonoBehaviour
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
    [SerializeField] ModifierSystem modifierSystem;

    public int currTurn;
    public EntityInstance player { get; private set; }
    public EntityInstance enemy { get; private set; }
    public List<BattleLog> battleLogs { get; private set; }
    public CardInstance currAttackCard;

    private void Awake()
    {
        battleLogs=new List<BattleLog>();
        player = new EntityInstance(playerHpBar, playerHpText, playerEnergyText, IModifierOwner.UserType.player, modifierSystem);
        enemy = new EntityInstance(enemyHpBar, enemyHpText, enemyEnergyText, IModifierOwner.UserType.enemy, modifierSystem);
        currTurn = 1;




    }
    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMchanged);
        EventBus.Subscribe<EventBus.RequestAddLog>(e_AddBattleLog);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMchanged);
        EventBus.Unsubscribe<EventBus.RequestAddLog>(e_AddBattleLog);
    }

    public string GetLogString()
    {
        string result = "";
        foreach (var log in battleLogs)
        {
            string typeString = "";






            result += String.Format("=={0}턴, ", log.turn);

            if (log.actor == player) result += "플레이어가주체로, ";
            else if (log.actor == enemy) result += "상대가주체로, ";

            if (log.card != null)
            {
                result += String.Format("{0}아이디의 카드가",log.cardId);
            }

            switch (log.type)
            {
                case LogType.TryAttack: typeString = "공격시도"; break;
                case LogType.TryBlock: typeString = "방어로소모";  break;
                case LogType.SetBlock: typeString = "방어존으로이동"; break;
                case LogType.peach: typeString = "피치"; break;
                case LogType.AttackFail: typeString = "공격실패"; break;
                case LogType.AttackSuccess: typeString = "공격명중"; break;
                case LogType.GetDamaged: typeString = string.Format("{0}데미지입음",log.value);break;
            }

            result += String.Format("{0} 하였다. \n", typeString);

        }




        return result;
    }

    public void e_FSMchanged(EventBus.FSMChanged e)
    {
        if(e.prev is EnemyMainPhaseState&&e.curr is PlayerMainPhaseState)
        {
            currTurn++;//fsm의 변이에 따라 현재 턴수 1을 추가함
        }
    }
    public void e_AddBattleLog(EventBus.RequestAddLog e)
    {
        BattleLog bufferBattleLog=new BattleLog();
        bufferBattleLog = e.newBattleLog;
        bufferBattleLog.turn = currTurn;

        EventBus.Publish<EventBus.LogUpdatedComplete>(new EventBus.LogUpdatedComplete { newBattleLog =e.newBattleLog });
        battleLogs.Add(bufferBattleLog);
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
        if (RemoveFromZone(card, from) == false)
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
            case CommonClass.ZoneType.PlayerAttackZone: currAttackCard = card; break;
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


    public enum LogType
    {
        TryAttack,//공격시도
        AttackSuccess,//공격성공
        AttackFail,//공격실패
        TryBlock,//방어카드 사용
        SetBlock,//방어카드 배치
        peach,//피치
        GetDamaged//데미지를 받음

    }
    public struct BattleLog
    {
        public int turn;

        public LogType type;

        public EntityInstance actor;   // 행동한 주체
        public EntityInstance target;  // 대상

        public CardInstance card;
        public int cardId;

        public int value;

    }
}