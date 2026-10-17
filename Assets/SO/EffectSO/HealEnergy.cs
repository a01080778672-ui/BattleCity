using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Effects/HealEnergy")]
public class HealEnergy : EffectSO  //에너지회복
{

    public int healPower;


    public override void Execute(CardContext card)
    {
        card.usedEntity.currEnergy += healPower;





    }


    public override string GetCardScript(CardContext ctx)
    {
        return string.Format("{0} 에너지추가", healPower);
    }

}
