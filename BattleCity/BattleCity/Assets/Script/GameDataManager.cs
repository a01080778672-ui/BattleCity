using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager: MonoBehaviour,IManager //카드 이외에 게임에 저장해야 할 것들을 데이터로 담는 아이
{
    public int currPlayerHp {  get; private set; }
    public int currOtherHp { get; private set; }
    public int currPlayerEnergy { get; private set; }


    public  Action<int, int> energeUpdate;
    public  Action<int, int> myHpUpdate;
    public  Action<int, int> otherHpUpdate;//UIsetter가 마스터매니저 통해서 구독할것이다.

    [SerializeField] int maxPlayerHp = 40;
    [SerializeField] int maxOtherHp=40;
    [SerializeField] int maxPlayerEnergy=5;


    private void Awake()
    {
        Register();
      
    }


    

    public void SetPlayerHp(int hp)
    {
        if(hp<=0)hp=0;
        if(hp>maxPlayerHp)hp=maxPlayerHp;

        this.currPlayerHp = hp;
       myHpUpdate?.Invoke(currPlayerHp, maxPlayerHp);
    }

    public void SetOtherHp(int hp)
    {
        if (hp <= 0) hp = 0;
        if( hp>maxOtherHp)hp=maxOtherHp;

        currOtherHp = hp;
        otherHpUpdate?.Invoke(currOtherHp, maxOtherHp);
    }

    public void SetPlayerEnergy(int energy)
    {
        if(energy<=0)
        {
            energy=0;
            EventBus.Publish<EventBus.AlarmText>(new EventBus.AlarmText { alarmText = "에너지가 0이 되었습니다! 카드를 사용하기 위해선 에너지가 필요한점 주의해주세요" });
        }
        if (energy > maxPlayerEnergy)
        {
            EventBus.Publish<EventBus.AlarmText>(new EventBus.AlarmText { alarmText="주의! 에너지는 최대 보유 수량보다 높을수 없습니다"});

            energy = maxPlayerEnergy;
        }

        currPlayerEnergy = energy;
        energeUpdate?.Invoke(currPlayerEnergy, maxPlayerEnergy);
    }

    public void Register()
    {
        if (MasterManager.Instance != null)
        {
            MasterManager.Instance.RegisterManager<GameDataManager>(this);
        }
        else
        {
            StartCoroutine(Registering());
        }
    }
    IEnumerator Registering()
    {
        yield return null;
        if (MasterManager.Instance != null)
        {
            MasterManager.Instance.RegisterManager<GameDataManager>(this);
        }
    }


    IEnumerator Start()
    {
        yield return null;
 
        SetOtherHp(40);
        SetPlayerHp(40);
        SetPlayerEnergy(2);
        

    }

}
