using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifierOwner //버프나 디버프를 가질수 있는 객체
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
