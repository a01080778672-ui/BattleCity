using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameLoopData;

public static class EventBus 
{
    private static Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();//딕셔너리. 아무 타입이 키값이고 delegate이 값이다.  delegate는 함수를 담는다.



    // 구독
    // 이벤트의 구독은 템플릿으로, 밑에 모여있는 구조체중 하나를 선택하고,  ( ) 안엔 함수의 주소값을 전달. 함수는 T와 같은 인자 하나를 가져야한다.
    public static void Subscribe<T>(Action<T> handler)
    {
        if (_events.TryGetValue(typeof(T), out var existing))
            _events[typeof(T)] = Delegate.Combine(existing, handler);
        else
            _events[typeof(T)] = handler;
    }

    // 해제
    // 이벤트의 구독헤제 역시 템플릿으로 진행된다. 구독을 끊을 대상을 밑에 구조체중 하나 선택하고 ( ) 안엔 해제할 함수 대상의 주소를 전달한다. 
    public static void Unsubscribe<T>(Action<T> handler)
    {
        if (_events.TryGetValue(typeof(T), out var existing))
            _events[typeof(T)] = Delegate.Remove(existing, handler);
    }

    // 발행
    //이벤트를 실행시킨다. 
    public static void Publish<T>(T eventData)
    {
        if (_events.TryGetValue(typeof(T), out var handler))
            (handler as Action<T>)?.Invoke(eventData);
    }

    public static void Clear()//그동안 구독된 모든 이벤트를 구독 해제 시킬수 있는 버튼... 씬이 바뀔때 자동 실행되도록 만들었다.
    {
        _events.Clear();
    }







    // 이벤트는 템플릿으로 정의된다. 템플릿에 들어갈 수 있는 자료형들을 여기에 모아둔다.







    /////////////////////////////////////////////////////////// ///////////////////////////////////////////////////////////
    /// <summary>
    /// 뭔가 일이 발생했어요! 하는 이벤트들 
    /// 매니저들은 무슨 일이 발생하면 무슨 행동을 할지 지정받을 수 있는 구조가 됨
    /// </summary>
    public struct CardLeftClickedEvent { public CardView card; }   // 카드 왼쪽 클릭 됐어요
    public struct CardRightClickedEvent { public CardView card; } //카드 오른쪽 클릭 됐어요

    public struct CardMouseIn {  public CardView card; }//마우스위에 카드가 올라왔어요

    public struct CardMouseOut {  public CardView card; }//카드가 내려왔어요



    public struct FSMChanged { public GameState prev; public GameState curr; }//상태기 ???에서 ??? 로 바뀌었어요

    public struct GraveOpenButtonClicked { }//무덤 버튼이 눌렸어요
    public struct DeckOpenButtonClicked { }//덱 버튼이 눌렸어요

    public struct LogOpenButtonClicked { }//로그 보기 버튼이 눌렸어요
    public struct CheckSelectedCardLive { }//무슨 카드가 손패에서 나갔어요(현재 선택한 카드를 null처리 할까말까 결정하기 위해서 있음)

    public struct BlockButtonClicked { }//방어하기 버튼이 눌렸어요(적의 공격을 막기위해 방어를 하는데 그거 말한거)


    public struct PlayerAttackSuccess { public CardContext card;public IModifierOwner player; public IModifierOwner enemy; }//플레이어의 공격 카드가 적중했을시 이걸 실행시킨다.

    public struct EnemyAttackSuccess { public CardContext card; public IModifierOwner player; public IModifierOwner enemy; }//적의 공격 카드가 적중했을시 이걸 실행시킨다.

    public struct CardBuffChanged { public CardInstance card; }//카드의 버프 내역이 변경되었어요! (카드의UI숫자 갱신을 위해서)

    public struct EntityBuffChanged { public EntityInstance entity; }//카드의 버프 내역이 변경되었어요! (카드의UI숫자 갱신을 위해서)

    public struct LogUpdatedComplete { public GameLoopData.BattleLog newBattleLog; }//로그가 업데이트 됐어요(카드의 패시브 버프 갱신을 위해)

    /////////////////////////////////////////////////////////// ///////////////////////////////////////////////////////////



    /// <summary>
    /// 연출 관련 이벤트들. UI에 보여주는 것만을 바꾸거나, 이펙트 등이 들어갈수 있을듯함 
    /// 해당 이벤트들은 실행되어도 게임내 데이터를 바꾸지는 않음.
    /// </summary>
    public struct AlarmText { public string alarmText; }//중앙에 크게 UI 특정한 알람을 보여줍니다.
    public struct UpdateGrave { public IReadOnlyList<CardInstance> cards; }//무덤 UI업데이트 하라 합니다
    public struct UpdateDeck { public IReadOnlyList<CardInstance> cards; }//덱 UI업데이트 하라고 합니다.

    public struct LogUpdate { public string log; }//로그 업데이트 하라 합니다(나중엔 로그 List를 통으로 보낼듯함.)
    public struct RelocateCardUI {  public CardInstance card; public CommonClass.ZoneType from;public CommonClass.ZoneType to; }//실제 데이터는 안 바뀌고, UI에서 카드를 여기서 여기로 보내달라고 하는 이벤트


    /////////////////////////////////////////////////////////// ///////////////////////////////////////////////////////////




    /// <summary>
    /// 무언가를 해달라고 요청하는 이벤트. ) 플레이어 hp나 적hp또는 에너지를 바꿔달라고 요청하는 이벤트들이 있다. 
    /// //이 이벤트가 실행되면 실제로 데이터가 바뀐뒤, UI를 업데이트 해달라는 이벤트가 다음에 배치되어있을 가능성이 높다.
    /// </summary>

    public struct RequestRelocateCard { public CardInstance card; public CommonClass.ZoneType to; }//1개의 카드를 특정 지점에 보내달라 요청하는 이벤트.


    public struct StartInitPhaseEvent { };//게임 시작 세팅 모드 (게임 시작시 자동으로 발동할듯)
    public struct StartPlayerSettingBlockCardPhaseEvent { }   // 플레이어 방어 세팅 턴으로 
    public struct StartPlayerMainPhaseEvent { }   // 플레이어 턴 시작 처리
    public struct StartPlayerTryBlockPhaseEvent { }   // 플레이어의 방어 시도 시작 턴으로 
    public struct StartEnemyMainPhaseEvent { }   // 적 턴 시작 됐어요
    public struct StartEnemySettingBlockCardPhaseEvent { }   // 플레이어 방어 세팅 턴으로 이동합니다
    public struct StartEnemyTryBlockPhaseEvent { }   // 플레이어의 방어 시도 시작 턴으로
    
    public struct RequestDrawCards {public EntityInstance who; public int number; }//카드를 드로우 해달라 요청하는 이벤트

    public struct RequestAddLog { public GameLoopData.BattleLog newBattleLog; }  //전투로그를 추가해달라 요청하는 이벤트이다.

   


    /////////////////////////////////////////////////////////// ///////////////////////////////////////////////////////////










}
