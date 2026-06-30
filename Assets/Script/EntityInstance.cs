using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EntityInstance//캡슐화를 위해 코드의 길이를 많이 늘려버렸습니다.
{
   public enum EntityType
    {
        player,
        enemy


    }
    public EntityType type { get; private set; }


    Slider hpBar;
   TextMeshProUGUI hpText;
     TextMeshProUGUI energyText;

    private int _currHp;
    public int currHp 
        {
           get { return _currHp; }
        set { _currHp = value;
        if(hpBar != null&&hpText!=null)
            {
                hpBar.value = (float)_currHp / (float)maxHp;
                hpText.text=string.Format("{0}/{1}",_currHp,maxHp);
            }
        }
        } 
    public int maxHp;

    private int _currEnergy;
    public int currEnergy
    {
        get { return _currEnergy; }
        set
        {
            _currEnergy = value;
            if(energyText!=null)
            {
                energyText.text=string.Format("{0}/{1}",_currEnergy,maxEnergy);
            }

        }
        }
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

    public EntityInstance(Slider hpBar,
    TextMeshProUGUI hpText,
    TextMeshProUGUI energyText,
    EntityType type)
    {
        this.type = type;
        this.hpBar = hpBar;
        this.hpText = hpText;
        this.energyText = energyText;


        _currHp = maxHp = 40;
        currEnergy = 0;
        maxEnergy = 5;

        if (hpBar != null && hpText != null)
        {
            hpBar.value = (float)_currHp / (float)maxHp;
            hpText.text = string.Format("{0}/{1}", _currHp, maxHp);
        }
        if (energyText != null)
        {
            energyText.text = string.Format("{0}/{1}", _currEnergy, maxEnergy);
        }


    }




}
