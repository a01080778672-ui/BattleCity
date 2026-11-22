using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectSO : ScriptableObject
{

   
    public abstract void Execute(CardContext ctx,int effectPower);

    public abstract string GetCardScript(int effectPower,CardContext ctx= null);

}
