using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMManager : MonoBehaviour   //이 매니저는 주입 당할것 같다.
{
    GameState currGameState;

    StartSettingState startState;

    EnemyMainPhaseState enemyPhase;//적의 메인 턴
    EnemySettingBlockPhaseState enemySettingBlockState;//적이 방어존에 카드 놓는 시간
    EnemyTryBlockPhaseState enemyTryBlockState;//적이 방어 시도하는 시간

    PlayerMainPhaseState playerPhase;//플레이어의 메인 턴
    PlayerSettingBlockPhaseState playerSettingBlockState;//플레이어가 방어존에 카드 놓는 시간
    PlayerTryBlockPhaseState playerTryBlockState;//플레이어가 방어 시도하는 시간


    private void OnEnable()
    {
       

        EventBus.Subscribe<EventBus.StartPlayerMainPhaseEvent>(e_StartPlayerTurn);
        EventBus.Subscribe<EventBus.StartPlayerSettingBlockCardPhaseEvent>(e_StartPlayerSettingBlockPhase);
        EventBus.Subscribe<EventBus.StartPlayerTryBlockPhaseEvent>(e_StartPlayerTryBlockPhase);
        EventBus.Subscribe<EventBus.StartEnemyMainPhaseEvent>(e_StartEnemyTurn);
        EventBus.Subscribe<EventBus.StartEnemySettingBlockCardPhaseEvent>(e_StartEnemySettingBlockPhase);
        EventBus.Subscribe<EventBus.StartEnemyTryBlockPhaseEvent>(e_StartEnemyTryBlockPhase);
        EventBus.Subscribe<EventBus.StartInitPhaseEvent>(e_StartSettingState);

    }


    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.StartPlayerMainPhaseEvent>(e_StartPlayerTurn);
        EventBus.Unsubscribe<EventBus.StartPlayerSettingBlockCardPhaseEvent>(e_StartPlayerSettingBlockPhase);
        EventBus.Unsubscribe<EventBus.StartPlayerTryBlockPhaseEvent>(e_StartPlayerTryBlockPhase);
        EventBus.Unsubscribe<EventBus.StartEnemyMainPhaseEvent>(e_StartEnemyTurn);
        EventBus.Unsubscribe<EventBus.StartEnemySettingBlockCardPhaseEvent>(e_StartEnemySettingBlockPhase);
        EventBus.Unsubscribe<EventBus.StartEnemyTryBlockPhaseEvent>(e_StartEnemyTryBlockPhase);
        EventBus.Unsubscribe<EventBus.StartInitPhaseEvent>(e_StartSettingState);
    }
    

    void e_StartSettingState(EventBus.StartInitPhaseEvent e)
    {
        ChangeGameState(startState);
    }
    void e_StartPlayerTurn(EventBus.StartPlayerMainPhaseEvent e)
    {
        ChangeGameState(playerPhase);
    }
    void e_StartPlayerSettingBlockPhase(EventBus.StartPlayerSettingBlockCardPhaseEvent e)
    {
        ChangeGameState(playerSettingBlockState);
    }
    void e_StartPlayerTryBlockPhase(EventBus.StartPlayerTryBlockPhaseEvent e)
    {
        ChangeGameState(playerTryBlockState);
    }
    void e_StartEnemyTurn(EventBus.StartEnemyMainPhaseEvent e)
    {
        ChangeGameState(enemyPhase);
    }
    void e_StartEnemySettingBlockPhase(EventBus.StartEnemySettingBlockCardPhaseEvent e)
    {
        ChangeGameState(enemySettingBlockState);
    }
    void e_StartEnemyTryBlockPhase(EventBus.StartEnemyTryBlockPhaseEvent e)
    {
        ChangeGameState(enemyTryBlockState);
    }

    private void Awake()
    {
        currGameState = null;
        startState=new StartSettingState(this); 

        enemyPhase = new EnemyMainPhaseState(this);
        enemySettingBlockState = new EnemySettingBlockPhaseState(this);
        enemyTryBlockState = new EnemyTryBlockPhaseState(this);

        playerPhase = new PlayerMainPhaseState(this);
        playerSettingBlockState = new PlayerSettingBlockPhaseState(this);
        playerTryBlockState = new PlayerTryBlockPhaseState(this);
    }


    private void Start()
    {
      
    }
    private void Update()
    {
        if(currGameState!=null)currGameState.OnUpdate();
    }

    public void ChangeGameState(GameState newGameState)
    {
        if (newGameState == null||currGameState== newGameState) return;


     
        EventBus.Publish(new EventBus.FSMChanged { prev = currGameState, curr = newGameState });

       currGameState?.OnExit();
       newGameState.OnEnter(currGameState);
       currGameState = newGameState;
    

    }

    public GameState GetCurrState()
    {
        return currGameState;
    }
  
}
