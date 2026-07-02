using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Conditions/HandCardNumber")]
public class HandCardNumberConditionSO : ConditionSO
{
    [SerializeField] bool isUpper;
    [SerializeField] bool isUsersCard;
    [SerializeField] int number;


    public override bool Evaluate(CardContext ctx)
    {
        if (isUsersCard)
        {
            if (isUpper)
            {
                return (ctx.usedEntity.HandCards.Count >= number);
            }
            else
            {
                return (ctx.usedEntity.HandCards.Count <= number);
            }





        }
        else
        {
            if (isUpper)
            {
                return (ctx.targetEntity.HandCards.Count >= number);
            }
            else
            {
                return (ctx.targetEntity.HandCards.Count <= number);
            }



        }



    }

    public override string GetEvaluateScript(CardContext ctx = null)
    {
        string buffer = "";
        if (isUsersCard)
        {
            buffer += "사용자의손패수가";
        }
        else
        {
            buffer += "피격자의손패수가";
        }
        buffer += number.ToString();
        if (isUpper)
        {
            buffer += "이상이면,";
        }
        else
        {
            buffer += "이하이면,";
        }

        return buffer;
    }
}
