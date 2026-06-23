using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Buttons : MonoBehaviour //테스트용 스크립트. 
{
    [SerializeField]FSMManager fsmmanager;
    public void StartInitFSMButtonPutted()
    {
        if(fsmmanager.GetCurrState()==null)
        EventBus.Publish<EventBus.StartInitPhaseEvent>(new EventBus.StartInitPhaseEvent());

        Destroy(this.gameObject);

    }

    
    public void GiveEnemyMainPhaseTurnButtonPutted()
    {
        if (fsmmanager.GetCurrState() is  PlayerSettingBlockPhaseState)
        {
            EventBus.Publish<EventBus.StartEnemyMainPhaseEvent>(new EventBus.StartEnemyMainPhaseEvent());
            EventBus.Publish(new EventBus.AlarmText { alarmText = "적에게 턴 주기 테스트 버튼 눌림." });
        }
        else if (fsmmanager.GetCurrState() is PlayerMainPhaseState)
        {
            EventBus.Publish<EventBus.StartPlayerSettingBlockCardPhaseEvent>(new EventBus.StartPlayerSettingBlockCardPhaseEvent());
            EventBus.Publish(new EventBus.AlarmText { alarmText = "방어 세팅 페이즈 시작 버튼 눌림" });

        }
        else
        {
            EventBus.Publish(new EventBus.AlarmText { alarmText = "플레이어의 페이즈에서만 넘길수 있다. " });
        }
    }
  
    public void OpenDeckButtonPutted()
    {
       
        EventBus.Publish<EventBus.DeckOpenButtonClicked>(new EventBus.DeckOpenButtonClicked());
       
    }











}
