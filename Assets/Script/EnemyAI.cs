using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameLoopData _data;
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

        if (incomingAttackCard == null)
        {
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            TryBlockCor = null;
            yield break;
        }

        if (!dakgongAlgorithm.ShouldDefend(_data.enemy, incomingAttackCard))
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "Enemy takes the attack." });
            EventBus.Publish(new EventBus.PlayerAttackSuccess { });
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            TryBlockCor = null;
            yield break;
        }

        List<CardInstance> blockCards = dakgongAlgorithm.ChooseBlockCards(_data.enemy, incomingAttackCard);
        if (blockCards.Count == 0)
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "Enemy cannot fully block." });
            EventBus.Publish(new EventBus.PlayerAttackSuccess { });
            EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
            TryBlockCor = null;
            yield break;
        }

        foreach (CardInstance blockCard in blockCards)
        {
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
        EventBus.Publish(new EventBus.RequestRelocateCard
        {
            card = attackCard,
            to = CommonClass.ZoneType.EnemyAttackZone
        });

        EventBus.Publish(new EventBus.AlarmText
        {
            alarmText = $"Enemy attacks with {attackCard.CardDataSO.cardName}."
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
            EventBus.Publish(new EventBus.RequestRelocateCard
            {
                card = card,
                to = CommonClass.ZoneType.EnemyBlockZone
            });
        }
    }

    void PitchCardsForAttack(CardInstance attackCard)
    {
        List<CardInstance> pitchCards = dakgongAlgorithm.ChoosePitchCards(_data.enemy, attackCard);
        foreach (CardInstance card in pitchCards)
        {
            EventBus.Publish(new EventBus.RequestRelocateCard
            {
                card = card,
                to = CommonClass.ZoneType.EnemyGraveZone
            });
            _data.enemy.currEnergy += 1;
        }
    }

    void UseDefenseCardEffect(CardInstance card)
    {
        if (card == null || card.CardDataSO == null) return;
        if (card.CardDataSO.type != CardDataSO.CardType.Block) return;
        if (card.CardDataSO.hitEffects == null) return;

        CardContext context = new CardContext
        {
            usedCard = card,
            usedEntity = _data.enemy,
            targetEntity = _data.player
        };

        foreach (var effectData in card.CardDataSO.hitEffects)
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