using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Conditions/LogAttackFail")]
public class LogAttackFailCondition : ConditionSO//최근의 공격 실패만 일단 감지합니다.
{
    
    public override bool Evaluate(CardContext ctx)
    {
        if(ctx.logs==null)return false;
        if(ctx.gameLoopData==null) return false;


        int logCount=ctx.logs.Count;
        int currTurn = ctx.gameLoopData.currTurn;

        if (ctx.logs[logCount-1].turn==currTurn && ctx.logs[logCount - 1].type is GameLoopData.LogType.AttackFail&& ctx.logs[logCount - 1].actor==ctx.usedEntity)
        {
            Debug.Log("로그통과");
            return true;
        }




        return false;
    }

    public override string GetEvaluateScript(CardContext ctx = null)
    {
        string buffer = "";
   

        return buffer;
    }
}
