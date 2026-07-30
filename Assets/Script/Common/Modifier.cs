using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier //기본 능력치에 버프를 걸어주기 위해 추가한 느낌이다.
{
    public Modifier(StatType type,int value, ModifierTrigger trigger,int stack)
    {
        this.stat=type;
        this.value=value;
        this.trigger=trigger;
        this.stack=stack;
    }




    public StatType stat;//무슨 보너스가 붙는가
    public int value;//수치





    public ModifierTrigger trigger;//언제 남은 횟수가 소모,삭제 되는가

    public int stack; // 남은 횟수

    public enum StatType
    {
        Attack, //추가 공격력 (플레이어한테 걸리면, 추가로 더 데미지 카드한테 걸려도 동일)
        //Block, //추가 방어력 (플레이어한테 걸리면, 데미지를 덜 받 카드한테 걸리면 방어카드의 효과 증가,또는 공격카드더라도 증가하는 식으로 갈듯)
        //EnergyEffency, //에너지 효율 (플레이어한테 걸리면 그만큼 에너지를 덜 씀 카드한테 걸리면 에너지를 그만큼 덜씀)
        Power//내구도 증가(플레이어한테 걸릴일은 없을듯함. 카드한테 걸리면 내구도 증가)

    }
    public enum ModifierTrigger
    {
        TurnStart,
        TurnEnd,
        TryUseCard,
        AttackSuccess
    }

}
