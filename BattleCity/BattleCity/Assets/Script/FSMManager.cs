using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMManager : MonoBehaviour,IManager
{
    GameState currGameState;
    OtherTurn otherTrun;
    PlayerTurn playerTurn;

    private void Awake()
    {
        otherTrun = new OtherTurn(this);
        playerTurn = new PlayerTurn(this);
    }

    private void Update()
    {
        if(currGameState!=null)currGameState.OnUpdate();
    }

    public void ChangeGameState(GameState newGameState)
    {
        if (newGameState == null||currGameState== newGameState) return;

        GameState prevState = newGameState;

       currGameState?.OnExit(newGameState);
       currGameState = newGameState;
        currGameState.OnEnter(prevState);

    }

    public void Register()
    {
        if(MasterManager.Instance!=null)
        {
            MasterManager.Instance.RegisterManager<FSMManager>(this);
        }
    }
}
