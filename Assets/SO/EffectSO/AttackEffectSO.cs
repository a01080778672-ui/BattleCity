using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Effects/InstantDamage")]
public class AttackEffectSO : EffectSO //공격을 즉시 발동한다 데미지를 줄수 있음
{

    [SerializeField] bool isvictimDamage;//피해자가 데미지를 입는건가요?

    public override void Execute(CardContext card,int effectPower) //즉시 공격 발동시 상대에게 대미지를 준다. 
    {
        if (isvictimDamage)
        {
            card.targetEntity.currHp -= effectPower;
        }
        else
        {
            card.usedEntity.currHp -= effectPower;
        }
    }

    public override string GetCardScript(int effectPower,CardContext ctx)
    {
        if (isvictimDamage)
        {
            return string.Format("{0}데미지를 준다.", effectPower);
        }
        else
        {
            return string.Format("{0}데미지를 입는다.", effectPower);
        }
    }
}
