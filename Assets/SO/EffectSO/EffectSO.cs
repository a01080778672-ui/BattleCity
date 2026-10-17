using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public abstract void Execute(CardContext ctx);

    public abstract string GetCardScript(CardContext ctx=null);

}
