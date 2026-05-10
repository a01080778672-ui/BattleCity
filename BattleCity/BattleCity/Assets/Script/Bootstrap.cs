using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]//부트스트렙은 게임이 실행될때 누구보다도 먼저 실행된다고 알고있음.
                                                                              //세이브 class뒤에 적었던 []처럼 이건 뒤에 있는 함수가 게임 시작시 awake보다 먼저 한번 실행하게 합니다 어디에도 존재하지 않아도 실행된다는 점이
    private static void MainInit()
    {
        if (MasterManager.Instance == null)
        {
            GameObject singleTon = new GameObject("MasterManager");
            singleTon.AddComponent<MasterManager>();
        }




        // SceneManager.sceneUnloaded 는 씬이 바뀌면 실행되는 유니티 제공 이벤트 함수이다. 여기에 적절한 것을 구독하면 된다.
        SceneManager.sceneUnloaded += _ => EventBus.Clear(); //이벤트 버스 클래스의 모든 구독 상태를 제거함
        SceneManager.sceneUnloaded += _ =>//  " _=> "  는 넘길게 없는데 구독할수 있게 해줌
        {
            if (MasterManager.Instance != null)
            {
                MasterManager.Instance.UnregisterAllManager();//마스터 매니저가 가진 모든 매니저를 삭제함
            }
        };

    }

   


}
