using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInstance//캡슐화를 위해 코드의 길이를 많이 늘려버렸습니다.
{
    public int currHp;
    public int maxHp;
    public int currEnergy;
    public int maxEnergy;

    private List<CardInstance> _graveCards = new List<CardInstance>();
    private List<CardInstance> _handCards = new List<CardInstance>();
    private List<CardInstance> _deckCards = new List<CardInstance>();
    private List<CardInstance> _blockCards = new List<CardInstance>();

    public IReadOnlyList<CardInstance> GraveCards => _graveCards;
    public IReadOnlyList<CardInstance> HandCards => _handCards;
    public IReadOnlyList<CardInstance> DeckCards => _deckCards;
    public IReadOnlyList<CardInstance> BlockCards => _blockCards;

    
    public bool RemoveFromGrave(CardInstance card) => _graveCards.Remove(card);
    public bool RemoveFromHand(CardInstance card) => _handCards.Remove(card);
    public bool RemoveFromDeck(CardInstance card) => _deckCards.Remove(card);
    public bool RemoveFromBlock(CardInstance card) => _blockCards.Remove(card);

    public void AddToGrave(CardInstance card) => _graveCards.Add(card);
    public void AddToHand(CardInstance card) => _handCards.Add(card);
    public void AddToDeck(CardInstance card) => _deckCards.Add(card);
    public void AddToBlock(CardInstance card) => _blockCards.Add(card);
    
    public EntityInstance()
    {
        currHp = maxHp = 40;
        currEnergy = 0;
        maxEnergy = 5;
    }
}
