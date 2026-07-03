using System;
using System.Collections.Generic;
using System.Linq;

public class DakgongAlgorithm
{
    private const string Guard = "\uC218\uBE44";
    private const string Desperate = "\uC8FD\uAE30\uC0B4\uAE30\uB85C";
    private const string Flicker = "\uD50C\uB9AC\uCEE4";
    private const string Slash = "\uBCA0\uAE30";
    private const string Sharpen = "\uC22B\uB3CC\uC9C8";
    private const string DropKick = "\uB4DC\uB86D\uD0A5";
    private const string Bind = "\uD718\uAC10\uAE30";
    private const string Ignite = "\uC810\uD654";
    private const string Club = "\uBABD\uB465\uC774\uC9C8";

    private static readonly string[] DefenseSetPriority =
    {
        Guard,
        Desperate,
        Flicker,
        Slash,
        Sharpen,
        DropKick,
        Bind,
        Ignite,
        Club
    };

    private static readonly string[] DefaultAttackPriority =
    {
        Club,
        Bind,
        DropKick,
        Ignite,
        Flicker,
        Slash,
        Sharpen
    };

    private static readonly string[] IgniteFinisherAttackPriority =
    {
        Club,
        Ignite,
        Bind,
        DropKick,
        Flicker,
        Slash,
        Sharpen
    };

    private static readonly string[] PitchPriority =
    {
        Guard,
        Desperate,
        Sharpen,
        Slash,
        Flicker,
        DropKick,
        Ignite,
        Bind,
        Club
    };

    public int GetTargetDefenseCount(EntityInstance enemy)
    {
        if (enemy == null) return 0;
        if (enemy.currHp <= 2) return 3;
        if (enemy.currHp <= 5) return 2;
        return 1;
    }

    public List<CardInstance> ChooseDefenseSetCards(EntityInstance enemy, int maxDefenseCards)
    {
        var result = new List<CardInstance>();
        if (enemy == null) return result;

        int targetCount = Math.Min(maxDefenseCards, GetTargetDefenseCount(enemy));
        int needCount = targetCount - enemy.BlockCards.Count;
        if (needCount <= 0) return result;

        return SortByPriority(enemy.HandCards, DefenseSetPriority)
            .Take(needCount)
            .ToList();
    }

    public CardInstance ChooseAttackCard(EntityInstance enemy)
    {
        if (enemy == null) return null;

        string[] priority = ShouldUseIgniteFinisherPriority(enemy)
            ? IgniteFinisherAttackPriority
            : DefaultAttackPriority;

        return SortByPriority(enemy.HandCards, priority)
            .Where(IsAttackCard)
            .Where(card => CanPayAfterPitch(enemy, card))
            .FirstOrDefault();
    }

    public List<CardInstance> ChoosePitchCards(EntityInstance enemy, CardInstance targetAttackCard)
    {
        var result = new List<CardInstance>();
        if (enemy == null || targetAttackCard == null) return result;

        int needEnergy = GetCardCost(targetAttackCard) - enemy.currEnergy;
        if (needEnergy <= 0) return result;

        return SortForPitch(enemy.HandCards)
            .Where(card => card != targetAttackCard)
            .Take(needEnergy)
            .ToList();
    }

    public bool ShouldDefend(EntityInstance enemy, CardInstance incomingAttackCard)
    {
        if (enemy == null || incomingAttackCard == null) return false;
        if (enemy.BlockCards.Count == 0) return false;
        if (!CanFullyBlock(enemy, incomingAttackCard)) return false;

        int incomingDamage = GetAttackDamage(incomingAttackCard);
        if (enemy.currHp <= incomingDamage) return true;
        if (incomingDamage >= 3) return true;
        if (HasCardName(incomingAttackCard, Club)) return true;

        return false;
    }

    public List<CardInstance> ChooseBlockCards(EntityInstance enemy, CardInstance incomingAttackCard)
    {
        var selectedCards = new List<CardInstance>();
        if (enemy == null || incomingAttackCard == null) return selectedCards;

        int requiredPower = GetAttackPower(incomingAttackCard);
        int blockScore = 0;

        foreach (CardInstance card in SortByPriority(enemy.BlockCards, DefenseSetPriority))
        {
            selectedCards.Add(card);
            blockScore += GetBlockPower(card);

            if (blockScore >= requiredPower)
            {
                return selectedCards;
            }
        }

        selectedCards.Clear();
        return selectedCards;
    }

    public bool CanFullyBlock(EntityInstance enemy, CardInstance incomingAttackCard)
    {
        return ChooseBlockCards(enemy, incomingAttackCard).Count > 0;
    }

    public int GetCardCost(CardInstance card)
    {
        if (card == null || card.CardDataSO == null || card.CardDataSO.cardCost == null || card.CardDataSO.cardCost.Length == 0)
        {
            return 0;
        }

        return Math.Max(0, card.CardDataSO.cardCost[0].cost);
    }

    public int GetAttackDamage(CardInstance card)
    {
        return card != null && card.CardDataSO != null ? card.CardDataSO.attack : 0;
    }

    public int GetAttackPower(CardInstance card)
    {
        return card != null && card.CardDataSO != null ? card.CardDataSO.power : 0;
    }

    public int GetBlockPower(CardInstance card)
    {
        if (card == null || card.CardDataSO == null) return 0;

        int blockPower = card.CardDataSO.blockPower;
        if (blockPower <= 0 && card.CardDataSO.type != CardDataSO.CardType.Block)
        {
            return 1;
        }

        return Math.Max(0, blockPower);
    }

    private bool CanPayAfterPitch(EntityInstance enemy, CardInstance targetAttackCard)
    {
        if (enemy == null || targetAttackCard == null) return false;

        int needEnergy = GetCardCost(targetAttackCard) - enemy.currEnergy;
        if (needEnergy <= 0) return true;

        int pitchableCount = SortForPitch(enemy.HandCards)
            .Count(card => card != targetAttackCard);

        return pitchableCount >= needEnergy;
    }

    private bool ShouldUseIgniteFinisherPriority(EntityInstance enemy)
    {
        CardInstance igniteCard = enemy.HandCards.FirstOrDefault(card => HasCardName(card, Ignite));
        if (igniteCard == null) return false;

        int needEnergy = GetCardCost(igniteCard) - enemy.currEnergy;
        int pitchCount = Math.Max(0, needEnergy);
        if (ChoosePitchCards(enemy, igniteCard).Count < pitchCount) return false;

        return enemy.HandCards.Count - 1 - pitchCount == 0;
    }

    private IEnumerable<CardInstance> SortByPriority(IEnumerable<CardInstance> cards, string[] priority)
    {
        return cards
            .Where(card => card != null && card.CardDataSO != null)
            .OrderBy(card => GetPriorityIndex(card, priority))
            .ThenByDescending(GetAttackDamage)
            .ThenByDescending(GetAttackPower)
            .ThenByDescending(GetBlockPower)
            .ThenBy(GetCardCost)
            .ThenBy(card => card.instanceId);
    }

    private IEnumerable<CardInstance> SortForPitch(IEnumerable<CardInstance> cards)
    {
        return cards
            .Where(card => card != null && card.CardDataSO != null)
            .OrderBy(card => GetPriorityIndex(card, PitchPriority))
            .ThenBy(GetAttackDamage)
            .ThenBy(GetAttackPower)
            .ThenBy(GetBlockPower)
            .ThenBy(GetCardCost)
            .ThenBy(card => card.instanceId);
    }

    private int GetPriorityIndex(CardInstance card, string[] priority)
    {
        string cardName = GetCardName(card);
        for (int i = 0; i < priority.Length; i++)
        {
            if (string.Equals(cardName, priority[i], StringComparison.Ordinal))
            {
                return i;
            }
        }

        return priority.Length;
    }

    private bool IsAttackCard(CardInstance card)
    {
        return card != null && card.CardDataSO != null && card.CardDataSO.type == CardDataSO.CardType.Attack;
    }

    private bool HasCardName(CardInstance card, string expectedName)
    {
        return string.Equals(GetCardName(card), expectedName, StringComparison.Ordinal);
    }

    private string GetCardName(CardInstance card)
    {
        return card != null && card.CardDataSO != null && card.CardDataSO.cardName != null
            ? card.CardDataSO.cardName.Trim()
            : string.Empty;
    }
}