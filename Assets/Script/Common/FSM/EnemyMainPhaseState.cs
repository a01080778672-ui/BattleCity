using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMainPhaseState : GameState
{

    public EnemyMainPhaseState(FSMManager manager):base(manager)
    {

    }

    public override void OnEnter(GameState prevState)
    {
       if(prevState is PlayerMainPhaseState)//플레이어턴->적턴 인 경우
        {
            EventBus.Publish(new EventBus.RequestPlayerDrawCards { });
            EventBus.Publish(new EventBus.RequestUseEnemyEnergy { energy = -2 });
        }

       
    }

    public override void OnExit()
    {
       
    }

    public override void OnUpdate()
    {
       
    }
}
