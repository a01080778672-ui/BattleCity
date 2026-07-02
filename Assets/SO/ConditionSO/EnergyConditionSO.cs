using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Conditions/Energy")]
public class EnergyConditionSO : ConditionSO
{
    [SerializeField] bool isUpper;
    [SerializeField] bool isUsersHp;
    [SerializeField] int number;


    public override bool Evaluate(CardContext ctx)
    {
        if (isUsersHp)
        {
            if (isUpper)
            {
                return (ctx.usedEntity.currEnergy >= number);
            }
            else
            {
                return (ctx.usedEntity.currEnergy <= number);
            }





        }
        else
        {
            if (isUpper)
            {
                return (ctx.targetEntity.currEnergy >= number);
            }
            else
            {
                return (ctx.targetEntity.currEnergy <= number);
            }



        }



    }

    public override string GetEvaluateScript(CardContext ctx = null)
    {
        string buffer = "";
        if (isUsersHp)
        {
            buffer += "사용자의에너지가";
        }
        else
        {
            buffer += "피격자의에너지가";
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
