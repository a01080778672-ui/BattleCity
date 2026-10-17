using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EventBus //이벤트 버스는 하나만 있어도 됨 싱글톤과 다른점은, monobehaviour를 쓸수 없고 앱 시작~종료때까지 메모리에 드러누워버리기 때문에 가급적 싱글톤이 나음
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

    public static void Clear()
    {
        _events.Clear();
    }







    // 이벤트는 템플릿으로 정의된다. 템플릿에 들어갈 수 있는 자료형들을 여기에 모아둔다.
    public struct CardPeachEvent { public int index; }   // n번째 카드 버리고 코스트 획득
    public struct CardSelectEvent { public int index; }   // n번째 카드 선택
    public struct CardLeftClickedEvent { public CardView card; }   // 카드 왼쪽 클릭 이벤트
    public struct CardRightClickedEvent { public CardView card; } //카드 오른쪽 클릭 이벤트
    public struct EndPlayerTurnEvent { }                   // 플레이어 턴 종료
    public struct StartPlayerTurnEvent { }                  // 플레이어 턴 시작
    public struct AlarmText { public string alarmText; }//중앙에 크게 UI 특정한 알람을 보여줍니다.





  





   

}
