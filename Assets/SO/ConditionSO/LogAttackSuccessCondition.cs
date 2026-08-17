using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Conditions/LogAttackSuccess")]
public class LogAttackSuccessCondition : ConditionSO//최근의 공격 성공만 일단 감지합니다.
{
    public override bool Evaluate(CardContext ctx)
    {
        if (ctx.logs == null) return false;
        if (ctx.gameLoopData == null) return false;


        int logCount = ctx.logs.Count;
        int currTurn = ctx.gameLoopData.currTurn;
        if (logCount-1 < 0) return false;

        if (ctx.logs[logCount - 1].turn == currTurn && ctx.logs[logCount - 1].type is GameLoopData.LogType.AttackSuccess && ctx.logs[logCount - 1].actor == ctx.usedEntity)
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
