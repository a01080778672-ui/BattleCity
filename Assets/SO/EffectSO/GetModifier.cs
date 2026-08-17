using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Effects/GetModifier")]
public class GetModifier : EffectSO//발동시 스탯(버프같은거)을 부여한다.
{
    [SerializeField]Modifier.StatType statType;
    [SerializeField]int value;
    [SerializeField]bool isEntity;//true면 주인인 엔티티가 획득 false면 카드 본인이 획득
    [SerializeField]Modifier.ModifierTrigger trigger;
    [SerializeField] int stack;


    public override void Execute(CardContext card)
    {
        Modifier bufferMod = new Modifier(statType, value, trigger, stack);


        if(isEntity)
        {
            card.usedEntity.AddBuff(bufferMod);
        }
        else
        {
            card.usedCard.AddBuff(bufferMod);
        }

    }


    public override string GetCardScript(CardContext ctx)
    {
        return "";
    }
}
