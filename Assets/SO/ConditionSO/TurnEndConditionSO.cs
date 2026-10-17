using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Conditions/TurnEnd")]
public class TurnEndConditionSO : ConditionSO
{
    [SerializeField] bool isUser;

    public override bool Evaluate(CardContext ctx)
    {
       if(isUser)
        {
            if (ctx.logs == null) return false;
            if (ctx.gameLoopData == null) return false;


            int logCount = ctx.logs.Count;
            int currTurn = ctx.gameLoopData.currTurn;

            if (ctx.logs[logCount - 1].type is GameLoopData.LogType.TurnEnd && ctx.logs[logCount - 1].actor == ctx.usedEntity)
            {
              
                return true;
            }


        

            return false;
        }
        else
        {
            if (ctx.logs == null) return false;
            if (ctx.gameLoopData == null) return false;


            int logCount = ctx.logs.Count;
            int currTurn = ctx.gameLoopData.currTurn;

            if (ctx.logs[logCount - 1].type is GameLoopData.LogType.TurnEnd && ctx.logs[logCount - 1].actor != ctx.usedEntity)
            {
                Debug.Log("로그통과");
                return true;
            }




            return false;
        }

    }

    public override string GetEvaluateScript(CardContext ctx = null)
    {
        string buffer = "";
        if (isUser)
        {
            buffer += "사용자의";
        }
        else
        {
            buffer += "상대가";
        }
        buffer += "턴이 종료시";
      

        return buffer;
    }
}
