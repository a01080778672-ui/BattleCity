using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Effects/InstantDamage")]
public class AttackEffectSO : EffectSO //공격을 즉시 발동한다 데미지를 줄수 있음
{
    public int attackDamage;

    public override void Execute(CardContext card) //즉시 공격 발동시 코스트를 소모하고 상대에게 대미지를 준다. 
    {
      
        EventBus.Publish(new EventBus.RequestOtherDamage
        {
            damage = attackDamage
        });
    }

    public override string GetCardScript(CardContext ctx)
    {
        return string.Format("{0}데미지를 준다.",attackDamage);
    }
}
