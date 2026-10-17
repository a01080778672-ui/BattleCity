using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState //게임 상태 
{
    protected FSMManager fsmManager;//상태 변환에 영향을 줄 매니저를 알아야한다.

    public GameState(FSMManager manager)
    {
        this.fsmManager = manager;//매니저 세팅
    }


    public abstract void OnEnter(GameState prevState);  // 진입
    public abstract void OnUpdate(); // 매 프레임
    public abstract void OnExit(GameState prevState);   // 나올 때
}
