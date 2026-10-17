using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifierOwner
{
    public enum UserType
    {
        player,
        enemy


    }
    UserType type { get; }

    public void AddBuff(Modifier newBuff);

    public void RemoveBuff(int i);


   IReadOnlyList<Modifier> currBuff { get; }
}
