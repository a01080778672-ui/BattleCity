using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Effects/HealEnergy")]
public class HealEnergy : EffectSO  //에너지회복
{

   

    public override void Execute(CardContext card, int effectPower)
    {
        card.usedEntity.currEnergy += effectPower;





    }


    public override string GetCardScript(int effectPower,CardContext ctx)
    {
        return string.Format("{0} 에너지추가", effectPower);
    }

}
