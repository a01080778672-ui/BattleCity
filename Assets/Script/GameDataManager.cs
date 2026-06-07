using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager: MonoBehaviour //게임 루프 데이터를 이용하는 매니저. 
{

    [SerializeField] GameLoopData _data;



   
    void OnEnable()
    {
        EventBus.Subscribe<EventBus.RequestPlayerDamage>(e_RequestPlayerDamage);
        EventBus.Subscribe<EventBus.RequestOtherDamage>(e_RequestOtherDamage);
        EventBus.Subscribe<EventBus.RequestUsePlayerEnergy>(e_RequestUsePlayerEnergy);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.RequestPlayerDamage>(e_RequestPlayerDamage);
        EventBus.Unsubscribe<EventBus.RequestOtherDamage>(e_RequestOtherDamage);
        EventBus.Unsubscribe<EventBus.RequestUsePlayerEnergy>(e_RequestUsePlayerEnergy);
    }

    void e_RequestPlayerDamage(EventBus.RequestPlayerDamage e)
    {
        SetPlayerHp(_data.player.currHp - e.damage);
    }
    void e_RequestOtherDamage(EventBus.RequestOtherDamage e)
    {
        SetOtherHp(_data.enemy.currHp - e.damage);
    }
    void e_RequestUsePlayerEnergy(EventBus.RequestUsePlayerEnergy e)
    {
        SetPlayerEnergy(_data.player.currEnergy - e.energy);
    }

    void SetPlayerHp(int newHp)
    {
        _data.player.currHp = Mathf.Clamp(newHp, 0, _data.player.maxHp);
        EventBus.Publish(new EventBus.UpdatedPlayerHp
        {
            newHp = _data.player.currHp,  // 이미 계산된 값
            maxHp = _data.player.maxHp
        });
    }



    void SetOtherHp(int newHp)
    {
        _data.enemy.currHp = Mathf.Clamp(newHp, 0, _data.enemy.maxHp);
        EventBus.Publish(new EventBus.UpdatedOtherHp
        {
            newHp = _data.enemy.currHp,
            maxHp = _data.enemy.maxHp
        });
    }



    void SetPlayerEnergy(int newEnergy)
    {

        if (newEnergy <= 0)
        {
            newEnergy = 0;
            EventBus.Publish(new EventBus.AlarmText
            {
                alarmText = "에너지가 0이 되었습니다!"
            });
        }
        if (newEnergy > _data.player.maxEnergy)
        {
            newEnergy = _data.player.maxEnergy;
            EventBus.Publish(new EventBus.AlarmText
            {
                alarmText = "에너지는 최대 보유 수량보다 높을 수 없습니다"
            });
        }

        _data.player.currEnergy = newEnergy;
        EventBus.Publish(new EventBus.UpdatedPlayerEnergy
        {
            newEnergy = _data.player.currEnergy,
            maxEnergy = _data.player.maxEnergy
        });

    }

    IEnumerator Start()
    {
        yield return null;
        EventBus.Publish(new EventBus.UpdatedPlayerHp
        {
            newHp = _data.player.currHp,
            maxHp = _data.player.maxHp
        });
        EventBus.Publish(new EventBus.UpdatedOtherHp
        {
            newHp = _data.enemy.currHp,
            maxHp = _data.enemy.maxHp
        });

        _data.player.currEnergy = 2;//테스트를 위해 2로 세팅
    
        EventBus.Publish(new EventBus.UpdatedPlayerEnergy { maxEnergy = _data.player.maxEnergy, newEnergy = _data.player.currEnergy });
    }

}
