using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviour
{

    public static MasterManager Instance { get; private set; }//싱글톤에서 적히는 것. public으로 누구든 접근하며, static으로 하나만 존재하는 나 자신의 함수. 

    public CardDB cardDB { get; private set; }

    private Dictionary<System.Type, IManager> managers = new Dictionary<System.Type, IManager>();//모든 매니저를 딕셔너리로 저장함.
    //이 딕셔너리는 키값이 타입 미지정에 값 부분은 매니저들을 넣는다. 키값은 자유분방. 값 영역은 매니저만 들어간다.
    public bool isQuitting { get; private set; }

    public void RegisterManager<T>(T a) where T : MonoBehaviour, IManager
    {
        //c++의 템플릿 타입네임이랑 비슷하다. 다만 where이 있는데 T가 위의 것들을 상속받은 자식이여야 한다는 의미


        if (managers.ContainsKey(typeof(T))) return;//이미 있는 매니저일시 추가 안함

        Debug.Log("managerRegister");
        managers[typeof(T)] =a;//딕셔너리의 경우 새로운 키에 값을 아예 대입하면 새롭게 생깁니다.
    }

    public T GetManager<T>()//메인 매니저가 가진 매니저중 T라는 종류의 매니저를 가져와라 라는 함수
    {
   
        if (managers.TryGetValue(typeof(T), out IManager manager))//매니저들 딕셔너리에서->키와 값 중 값을 가져오기 시도하는데, T라는 종류의 매니저를 가져올 것이며 가져온 것은 out manager 에 담아진다.
        {
            return (T)manager;//매니저에 담아지는데 성공하면 if문 통과후 여기서 그것을 반환.
        }

        return default;//찾지 못할시 디폴트를 반환합니다. null을 반환하는 느낌입니다.
    }

    public void UnregisterManager<T>() where T : MonoBehaviour, IManager
    {
        if (managers.ContainsKey(typeof(T)))//T라는 타입이 딕셔너리에 있나요? 있으면 IF문 통과
        {
            managers.Remove(typeof(T));//T라는 타입이 키값으로 있으면 딕셔너리에서 삭제.
        }
    }
    
    public void UnregisterAllManager()//모든 매니저를 삭제함. 씬의 변경때마다 실행하면 됩니다.
    {
        managers.Clear();
    }



    private void Awake()//부트스트랩에서 싱글톤을 만들고 이건 이제 안전장치로써 역할
    {
        if (Instance != null && Instance != this)//싱글톤인데 이미 다른게 있으면 그걸 지우
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        cardDB= Resources.Load<CardDB>("CardDB");

    }


    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

}
