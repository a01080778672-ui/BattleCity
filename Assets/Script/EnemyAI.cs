using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour //적의 ai이다. 여기서 상태변이도 하게 만들어야 하려나? 일단 그렇게 해보자.
{
    [SerializeField]GameLoopData _data;
    Coroutine TryBlockCor;
    Coroutine MainPhaseCor;
    Coroutine InitPhaseCor;
    private void Awake()
    {
        TryBlockCor = null;
        MainPhaseCor = null;
        InitPhaseCor = null;
    }
    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.StartInitPhaseEvent>(e_StartAI);
        EventBus.Subscribe<EventBus.StartEnemyTryBlockPhaseEvent>(e_TryBlock);
        EventBus.Subscribe<EventBus.StartEnemyMainPhaseEvent>(e_MainEnemyPhase);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.StartInitPhaseEvent>(e_StartAI);
        EventBus.Unsubscribe<EventBus.StartEnemyTryBlockPhaseEvent>(e_TryBlock);
        EventBus.Unsubscribe<EventBus.StartEnemyMainPhaseEvent>(e_MainEnemyPhase);
    }

    void e_StartAI(EventBus.StartInitPhaseEvent e)//게임 시작시 ai가 할 것
    {

        if (InitPhaseCor != null) return;
        InitPhaseCor = StartCoroutine(InitPhase());
    }
    IEnumerator InitPhase()
    {

        //카드 스왑 매니저에서 이미 드로우는 있음.

        //어떤 카드를 방어존에 놓을지 세팅합니다.

        //일단 세팅 안 한채 플레이어에게 턴을 줍니다.
        yield return new WaitForSeconds(5.0f);

        EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
        InitPhaseCor = null;
    }

    void e_TryBlock(EventBus.StartEnemyTryBlockPhaseEvent e)
    {
        if (TryBlockCor != null) return;
       TryBlockCor=StartCoroutine(TryBlock());

    }
    IEnumerator TryBlock()
    {
        //플레이어가 공격을 해와서, 방어를 시도합니다.


        //일단 아무것도 방어 안하고 즉시 포기합니다.
        bool blockSuccessed = false;


        yield return new WaitForSeconds(5.0f);

        if (blockSuccessed == false) EventBus.Publish(new EventBus.PlayerAttackSuccess { });//무조껀 공격에 맞음...



        EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });
        TryBlockCor = null;
    }

    void e_MainEnemyPhase(EventBus.StartEnemyMainPhaseEvent e)
    {
        if (MainPhaseCor != null) return;
        MainPhaseCor =StartCoroutine(EnemyMainPhase());  
    }
    IEnumerator EnemyMainPhase()
    {
        //적의 메인 페이즈때 할 행동을 적습니다.
        EventBus.Publish(new EventBus.RequestUseEnemyEnergy { energy = -2 });//턴을 받으면 2 에너지를 충전합니다.
        yield return new WaitForSeconds(5.0f);
        EventBus.Publish(new EventBus.StartPlayerMainPhaseEvent { });//그냥 바로바로 플레이어 턴으로 넘겨주는 것으로....
        MainPhaseCor=null;
    }


}
