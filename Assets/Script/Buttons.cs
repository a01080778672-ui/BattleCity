using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Buttons : MonoBehaviour //테스트용 스크립트. 실전에서 사용하진 않을예정
{
    [SerializeField]FSMManager fsmmanager;
    public void StartInitFSMButtonPutted()
    {
        if(fsmmanager.GetCurrState()==null)
        EventBus.Publish<EventBus.StartInitPhaseEvent>(new EventBus.StartInitPhaseEvent());
    }


    public void GiveEnemyMainPhaseTurnButtonPutted()
    {
        if (fsmmanager.GetCurrState() is PlayerMainPhaseState)
        {
            EventBus.Publish<EventBus.StartEnemyMainPhaseEvent>(new EventBus.StartEnemyMainPhaseEvent());
            EventBus.Publish(new EventBus.AlarmText { alarmText = "적에게 턴 주기 테스트 버튼 눌림." });
        }
        else
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "플레이어 메인 페이즈에서만 넘길수 있다. " });

        }
    }



    public void OpenDeckButtonPutted()
    {
       
        EventBus.Publish<EventBus.DeckOpenButtonClicked>(new EventBus.DeckOpenButtonClicked());
       
    }



    public void UIAlarmButtonPutted(string text)
    {
        EventBus.Publish<EventBus.AlarmText>(new EventBus.AlarmText { alarmText = text });
    }




    public void AttackPlayerButtonPutted(int dam)
    {
        EventBus.Publish<EventBus.RequestPlayerDamage>(new EventBus.RequestPlayerDamage {damage=dam });
    }
    public void AttackEnemyButtonPutted(int dam)
    {
        EventBus.Publish<EventBus.RequestOtherDamage>(new EventBus.RequestOtherDamage { damage = dam });
    }
    public void EnergySetButtonPutted(int delta)
    {
        EventBus.Publish<EventBus.RequestUsePlayerEnergy>(new EventBus.RequestUsePlayerEnergy { energy=delta });

    }




}
