using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Effects/InstantHeal")]
public class HealEffectSO : EffectSO  //방어 효과를 발동한다. 다만 지금은 회복으로 구현되었으므로 수정이 필요할것이다.
{

    public int healPower;


    public override void Execute(CardContext card)
    {
        card.usedEntity.currHp += healPower;


     
  

    }


    public override string GetCardScript(CardContext ctx)
    {
        return string.Format("{0} 힐파워", healPower);
    }

}
