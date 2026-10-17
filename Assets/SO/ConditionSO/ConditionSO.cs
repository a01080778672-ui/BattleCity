using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionSO : ScriptableObject
{
    public abstract bool Evaluate(CardContext ctx);

    public abstract string GetEvaluateScript(CardContext ctx = null);
}
