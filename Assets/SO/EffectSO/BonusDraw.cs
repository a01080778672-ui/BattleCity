using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Effects/BonusDraw")]
public class BonusDraw : EffectSO  //추가 드로우
{

    public int drawNumber;


    public override void Execute(CardContext card)
    {

        EventBus.Publish<EventBus.RequestDrawCards>(new EventBus.RequestDrawCards { who = card.usedEntity,number=drawNumber });



    }


    public override string GetCardScript(CardContext ctx)
    {
        return string.Format("{0}장 추가드로우", drawNumber);
    }

}
