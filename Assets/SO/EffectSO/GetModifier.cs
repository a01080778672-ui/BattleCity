using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Effects/GetModifier")]
public class GetModifier : EffectSO//발동시 스탯(버프같은거)을 부여한다.
{
    [SerializeField]Modifier.StatType statType;
    [SerializeField]int value;
    [SerializeField]bool isEntity;//true면 주인인 엔티티가 획득 false면 카드 본인이 획득
    [SerializeField]List<ConditionSO> consumptionTrigger;
    [SerializeField] int stack;//부여할 버프의 스택

    [SerializeField] List<ConditionSO> deadTrigger;//부여할 버프의, 여기에 해당하는 조건에 맞으면 즉시 버프 삭제
    [SerializeField] List<ConditionSO> playTrigger;//부여할 버프의, 여기에 해당하는 조건에 맞으면 해당 버프의 효과발동
    [SerializeField]List<EffectSO> buffEffects;//부여할 버프의, 버프의 추가 효과가 있다면 여기에

    [SerializeField] int buffIconID;
    [SerializeField] string buffScript;


    public override void Execute(CardContext card)
    {
        Modifier bufferMod = new Modifier(statType, value, consumptionTrigger, stack,buffIconID,buffScript, deadTrigger,playTrigger,buffEffects);


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
        return "버프를획득";
    }
}
