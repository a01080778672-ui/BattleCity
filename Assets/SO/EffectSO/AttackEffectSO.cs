using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Effects/InstantDamage")]
public class AttackEffectSO : EffectSO //공격을 즉시 발동한다 데미지를 줄수 있음
{
    public int attackDamage;

    [SerializeField] bool isvictimDamage;//피해자가 데미지를 입는건가요?

    public override void Execute(CardContext card) //즉시 공격 발동시 상대에게 대미지를 준다. 
    {
        if (isvictimDamage)
        {
            card.targetEntity.currHp -= attackDamage;
        }
        else
        {
            card.usedEntity.currHp -= attackDamage;
        }
    }

    public override string GetCardScript(CardContext ctx)
    {
        if (isvictimDamage)
        {
            return string.Format("{0}데미지를 준다.", attackDamage);
        }
        else
        {
            return string.Format("{0}데미지를 입는다.", attackDamage);
        }
    }
}
