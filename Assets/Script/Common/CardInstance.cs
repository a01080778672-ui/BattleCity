using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardDataSO;

public class CardInstance : IModifierOwner  //카드의 정보 카드의 오염 여부나 카드 인스턴스 id가 추가될 수 있음
{
    static int currPointInstanceId=0;

    private List<Modifier> _currBuff = new List<Modifier>();

    public IReadOnlyList<Modifier> currBuff => _currBuff;

    public void AddBuff(Modifier newBuff)
    {
        _currBuff.Add(newBuff);
        EventBus.Publish(new EventBus.CardBuffChanged { card = this });
    }

    public void RemoveBuff(Modifier buff)
    {
        _currBuff.Remove(buff);
        EventBus.Publish(new EventBus.CardBuffChanged { card = this });
    }

    public void RemoveBuff(int i)
    {
        _currBuff.RemoveAt(i);
        EventBus.Publish(new EventBus.CardBuffChanged { card = this });
    }

    private IModifierOwner.UserType _type;
    private EntityInstance _owner;
    public EntityInstance owner { get { return _owner; } set { _owner = value; _type = value.type; } }
    public IModifierOwner.UserType type { get { return _type; } }

    public CardInstance(CardDataSO cardDataSO,ModifierSystem modifierSystem,IModifierOwner.UserType ownerType,EntityInstance owner)
    {
        instanceId = currPointInstanceId;
        this.CardDataSO = cardDataSO;
        currPointInstanceId++;
        modifierSystem.CardRegister(this);
        this._type = ownerType;
        this._owner = owner;
    }

    public int instanceId;
    CardDataSO CardDataSO;


    public bool isCardDataSOVaild()
    {
        if(CardDataSO == null) return false;

        return true;
    }

    public int GetPower()//주인의 버프+ 내 버프를 모두 포함해서 수치를 반환해야한다.
    {
        int finalPower = CardDataSO.power;
        foreach (var item in owner.currBuff)
        {
            if(item.stat==Modifier.StatType.Power)
            {
                finalPower += item.value;
            }
        }
        foreach (var item in this.currBuff)
        {
            if (item.stat == Modifier.StatType.Power)
            {
                finalPower += item.value;
            }
        }
       return finalPower;
    }

    public int GetAttack()//주인의 버프+ 내 버프를 모두 포함해서 수치를 반환해야한다.
    {
        int finalAttack = CardDataSO.attack;
        foreach (var item in owner.currBuff)
        {
            if (item.stat == Modifier.StatType.Attack)
            {
                finalAttack += item.value;
            }
        }
        foreach (var item in this.currBuff)
        {
            if (item.stat == Modifier.StatType.Attack)
            {
                finalAttack += item.value;
            }
        }
        return finalAttack;
    }

    public int GetBlockPower()//주인의 버프+ 내 버프를 모두 포함해서 수치를 반환해야한다.
    {
        return CardDataSO.power;
    }

    public string GetCardName()
    {
        return CardDataSO.cardName;
    }
    public Cost[] GetCardCost()
    {
        return CardDataSO.cardCost;
    }

    public Sprite GetIllustration()
    {
        return CardDataSO.illustration;
    }

    public ColorType GetColorType()
    {
        return CardDataSO.colorType;
    }


    public CardType GetCardType()
    {
        return CardDataSO.type;
    }
    public List<EffectDefinition> GetHitEffects()
    {
        return CardDataSO.hitEffects;
    }

    public int GetCardId()
    {
        return CardDataSO.cardId;
    }
  



}
