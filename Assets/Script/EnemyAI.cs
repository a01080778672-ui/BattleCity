using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameLoopData _data;
    [SerializeField] FSMManager fsmManager;
    [SerializeField] int maxEnemyCapacityBlockCardNumber = 4;
    [SerializeField] float initPhaseDelay = 2.0f;
    [SerializeField] float mainPhaseDelay = 5.0f;
    [SerializeField] float tryBlockDelay = 5.0f;

    readonly DakgongAlgorithm dakgongAlgorithm = new DakgongAlgorithm();

    Coroutine TryBlockCor;
    Coroutine MainPhaseCor;
    Coroutine InitPhaseCor;

    private void Awake()
    {
        TryBlockCor = null;
        MainPhaseCor = null;
        InitPhaseCor = null;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.StartInitPhaseEvent>(e_StartAI);
        EventBus.Subscribe<EventBus.StartEnemyTryBlockPhaseEvent>(e_TryBlock);
        EventBus.Subscribe<EventBus.StartEnemyMainPhaseEvent>(e_MainEnemyPhase);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.StartInitPhaseEvent>(e_StartAI);
        EventBus.Unsubscribe<EventBus.StartEnemyTryBlockPhaseEvent>(e_TryBlock);
        EventBus.Unsubscribe<EventBus.StartEnemyMainPhaseEvent>(e_MainEnemyPhase);
    }

    void e_StartAI(EventBus.StartInitPhaseEvent e)
    {
        if (InitPhaseCor != null) return;
        InitPhaseCor = StartCoroutine(InitPhase());
    }

    IEnumerator InitPhase()
    {
        yield return new WaitForSeconds(initPhaseDelay);

        EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
        InitPhaseCor = null;
    }

    void e_TryBlock(EventBus.StartEnemyTryBlockPhaseEvent e)
    {
        if (TryBlockCor != null) return;
        TryBlockCor = StartCoroutine(TryBlock());
    }

    IEnumerator TryBlock()
    {
        CardInstance incomingAttackCard = _data != null ? _data.currAttackCard : null;
        yield return new WaitForSeconds(tryBlockDelay);

        if (incomingAttackCard == null)//방어할 카드가 없어서 그냥 넘김
        {
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            TryBlockCor = null;
            yield break;
        }

        if (!dakgongAlgorithm.ShouldDefend(_data.enemy, incomingAttackCard))//방어 포기
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "Enemy takes the attack." });
            EventBus.Publish(new EventBus.PlayerAttackSuccess {player=_data.player,enemy=_data.enemy });
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            TryBlockCor = null;
            yield break;
        }

        List<CardInstance> blockCards = dakgongAlgorithm.ChooseBlockCards(_data.enemy, incomingAttackCard);
        if (blockCards.Count == 0)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "Enemy cannot fully block." });
            EventBus.Publish(new EventBus.PlayerAttackSuccess { player = _data.player, enemy = _data.enemy });
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            TryBlockCor = null;
            yield break;
        }

        foreach (CardInstance blockCard in blockCards)
        {
            GameLoopData.BattleLog bufferLog=new GameLoopData.BattleLog();
            bufferLog.actor=_data.enemy;
            bufferLog.target=_data.player;
            bufferLog.card=blockCard;
            bufferLog.cardId=blockCard.GetCardId();
            bufferLog.type=GameLoopData.LogType.TryBlock;
            EventBus.Publish<EventBus.RequestAddLog>(new EventBus.RequestAddLog {newBattleLog=bufferLog });//방어시도 쓴카드 로그 추가 요청



            UseDefenseCardEffect(blockCard);
            EventBus.Publish(new EventBus.RequestRelocateCard
            {
                card = blockCard,
                to = CommonClass.ZoneType.EnemyGraveZone
            });
        }

        EventBus.Publish(new EventBus.AlarmText
        {
            alarmText = $"Enemy blocks with {blockCards.Count} card(s)."
        });
        EventBus.Publish(new EventBus.RequestRelocateCard
        {
            card = incomingAttackCard,
            to = CommonClass.ZoneType.PlayerGraveZone
        });
        EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
        TryBlockCor = null;
    }

    void e_MainEnemyPhase(EventBus.StartEnemyMainPhaseEvent e)
    {
        if (MainPhaseCor != null) return;
        MainPhaseCor = StartCoroutine(EnemyMainPhase());
    }

    IEnumerator EnemyMainPhase()
    {
        yield return new WaitForSeconds(mainPhaseDelay);

        SetDefenseCardsByAlgorithm();

        CardInstance attackCard = dakgongAlgorithm.ChooseAttackCard(_data.enemy);
        if (attackCard == null)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "Enemy has no playable attack." });
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            MainPhaseCor = null;
            yield break;
        }

        PitchCardsForAttack(attackCard);

        int cost = dakgongAlgorithm.GetCardCost(attackCard);
        if (_data.enemy.currEnergy < cost)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "Enemy does not have enough energy." });
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            MainPhaseCor = null;
            yield break;
        }

        _data.enemy.currEnergy -= cost;

        GameLoopData.BattleLog bufferLog = new GameLoopData.BattleLog();
        bufferLog.actor = _data.enemy;
        bufferLog.target = _data.player;
        bufferLog.card = attackCard;
        bufferLog.cardId = attackCard.GetCardId();
        bufferLog.type = GameLoopData.LogType.TryAttack;
        EventBus.Publish<EventBus.RequestAddLog>(new EventBus.RequestAddLog { newBattleLog = bufferLog });//공격시도 로그 추가

        EventBus.Publish(new EventBus.RequestRelocateCard
        {
            card = attackCard,
            to = CommonClass.ZoneType.EnemyAttackZone
        });//적의 공격을 위해 카드를 옮긴듯.

        EventBus.Publish(new EventBus.AlarmText
        {
            alarmText = $"Enemy attacks with {attackCard.GetCardName()}."
        });
        EventBus.Publish(new EventBus.StartPlayerTryBlockPhaseEvent { });
        MainPhaseCor = null;
    }

    void SetDefenseCardsByAlgorithm()
    {
        if (_data == null || _data.enemy == null) return;

        List<CardInstance> defenseCards = dakgongAlgorithm.ChooseDefenseSetCards(_data.enemy, maxEnemyCapacityBlockCardNumber);
        foreach (CardInstance card in defenseCards)
        {
            GameLoopData.BattleLog bufferLog = new GameLoopData.BattleLog();
            bufferLog.actor = _data.enemy;
            bufferLog.card = card;
            bufferLog.cardId = card.GetCardId();
            bufferLog.type = GameLoopData.LogType.SetBlock;
            EventBus.Publish<EventBus.RequestAddLog>(new EventBus.RequestAddLog { newBattleLog = bufferLog });//방어 배치 로그 추가




            EventBus.Publish(new EventBus.RequestRelocateCard
            {
                card = card,
                to = CommonClass.ZoneType.EnemyBlockZone
            });
        }
    }

    void PitchCardsForAttack(CardInstance attackCard)//공격을 하기위해 피치한다. 
    {
        List<CardInstance> pitchCards = dakgongAlgorithm.ChoosePitchCards(_data.enemy, attackCard);
        foreach (CardInstance card in pitchCards)
        {
            GameLoopData.BattleLog bufferLog = new GameLoopData.BattleLog();
            bufferLog.actor = _data.enemy;
            bufferLog.card = card;
            bufferLog.cardId=card.GetCardId();
            bufferLog.type = GameLoopData.LogType.peach;
            EventBus.Publish<EventBus.RequestAddLog>(new EventBus.RequestAddLog { newBattleLog = bufferLog });//피치 로그 추가 요청



            EventBus.Publish(new EventBus.RequestRelocateCard
            {
                card = card,
                to = CommonClass.ZoneType.EnemyGraveZone
            });
            _data.enemy.currEnergy += 1;//피치해서 얻는게 늘어나면 여기를 바꿔야할듯.
        }
    }

    void UseDefenseCardEffect(CardInstance card)
    {
        if (card == null || card.isCardDataSOVaild()==false) return;
        if (card.GetCardType() != CardDataSO.CardType.Block) return;
       

        CardContext context = new CardContext(card, fsmManager.GetCurrState(), _data, _data.enemy, _data.player);

     

        foreach (var effectData in card.GetHitEffects())
        {
            bool canUseEffect = true;
            if (effectData.conditions != null)
            {
                foreach (var condition in effectData.conditions)
                {
                    if (condition != null && !condition.Evaluate(context))
                    {
                        canUseEffect = false;
                        break;
                    }
                }
            }

            if (canUseEffect && effectData.effects != null)
            {
                effectData.effects.Execute(context);
            }
        }
    }
}