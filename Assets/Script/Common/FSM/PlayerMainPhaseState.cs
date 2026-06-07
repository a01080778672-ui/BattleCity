using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainPhaseState : GameState
{

    public PlayerMainPhaseState(FSMManager manager) : base(manager)
    {

    }
    public override void OnEnter(GameState prevState)
    {
        if (prevState is EnemyMainPhaseState)//적턴->플레이어 턴 인경우
        {
            EventBus.Publish(new EventBus.RequestEnemyDrawCards { });
            EventBus.Publish(new EventBus.RequestUsePlayerEnergy { energy = -2 });
        }
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {

    }
}
