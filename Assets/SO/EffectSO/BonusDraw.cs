using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Effects/BonusDraw")]
public class BonusDraw : EffectSO  //추가 드로우
{




    public override void Execute(CardContext card, int effectPower)
    {

        EventBus.Publish<EventBus.RequestDrawCards>(new EventBus.RequestDrawCards { who = card.usedEntity,number=effectPower });



    }


    public override string GetCardScript(int effectPower, CardContext ctx)
    {
        return string.Format("{0}장 추가드로우", effectPower);
    }

}
