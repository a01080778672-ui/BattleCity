using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Conditions/Hp")]
public class HpConditionSO : ConditionSO
{
    [SerializeField] bool isUpper;
    [SerializeField] bool isUsersHp;
    [SerializeField] int number;


    public override bool Evaluate(CardContext ctx)
    {
        if (isUsersHp)
        {
            if(isUpper)
            {
                return (ctx.usedEntity.currHp >= number);
            }
            else
            {
                return (ctx.usedEntity.currHp <= number);
            }

              
                


        }
        else
        {
            if (isUpper)
            {
                return (ctx.targetEntity.currHp >= number);
            }
            else
            {
                return (ctx.targetEntity.currHp <= number);
            }


           
        }



    }

    public override string GetEvaluateScript(CardContext ctx = null)
    {
        string buffer = "";
        if (isUsersHp)
        {
            buffer +="사용자의HP가";
        }
        else
        {
            buffer +="피격자의HP가";
        }
        buffer += number.ToString();
        if(isUpper)
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
