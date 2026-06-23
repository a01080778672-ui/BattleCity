using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UISetter : MonoBehaviour//카드만 만지는 CardUISetter와 다르게 얘는 다른 UI를 업데이트 해줍니다.
{
    [SerializeField] Canvas canvas;




    [SerializeField] TextMeshProUGUI[] alarmText;//여러개의 알람이 필요할때를 대비하여 배열로 받음
    [SerializeField] RectTransform alarmPos;
    [SerializeField] TextMeshProUGUI FSMuiText;


    [SerializeField] float gap=1.0f;
    [SerializeField] float alarmFadeTime = 1.0f;
    [SerializeField] float alarmInitTime = 2.0f;


 



    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.AlarmText>(e_Alarm);
        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMTextChange);

    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.AlarmText>(e_Alarm);
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMTextChange);

    }


    void e_FSMTextChange(EventBus.FSMChanged e)
    {
        if (FSMuiText == null) return;
        switch (e.curr)
        {
            case PlayerMainPhaseState:
                FSMuiText.text = "플레이어 메인 페이즈";
                break;
            case PlayerSettingBlockPhaseState:
                FSMuiText.text = "플레이어 방어세팅 페이즈";
                break;
            case PlayerTryBlockPhaseState:
                FSMuiText.text = "플레이어 방어시도 페이즈";
                break;
            case EnemyMainPhaseState:
                FSMuiText.text = "적의 메인 페이즈";
                break;
            case EnemySettingBlockPhaseState:
                FSMuiText.text = "적의 방어세팅 페이즈";
                break;
            case EnemyTryBlockPhaseState:
                FSMuiText.text = "적의 방어시도 페이즈";
                break;
            case StartSettingState:
                FSMuiText.text = "게임시작 페이즈";
                break;


        }

    }


    void e_Alarm(EventBus.AlarmText e)
    {
        SetAlarm(e.alarmText);
    }
    void SetAlarm(string text)
    {
      
        foreach (var item in alarmText)//알람 텍스트들 중에서 반복문을 돌림
        {
            if (!item.gameObject.activeSelf)//그 텍스트 중에서 살아있지 않은 놈이라면
            {
                item.gameObject.SetActive(true);//다시 활성화 시킴
                item.text = text;//텍스트도 초기화
                item.rectTransform.position = alarmPos.position;//위치도 첫번째로
                PlayAlarmFade(item, alarmInitTime, alarmFadeTime);//두튜윈 재생 시작! 
             
                foreach (var item2 in alarmText)
                {
                    if(item2.gameObject.activeSelf==true&&item!=item2)
                    {
                        item2.rectTransform.position += new Vector3(0, gap, 0);
                    }
                }


                return;
            }
        }


        //여기 밑으로 갔다는 뜻은 쉬고있는 알림 텍스트가 없다는 뜻임.



        TextMeshProUGUI remindText = null;
        foreach(var item in alarmText)
        {
            if(remindText == null)
            {
                remindText = item;
                continue;
            }

            if(remindText.rectTransform.position.y<item.rectTransform.position.y)
            {
                remindText = item;
            }
        }
        remindText.text = text;//가장 위에 있던 놈을 가져와 텍스트 초기화
        remindText.rectTransform.position = alarmPos.position;//위치도 첫번째로
        PlayAlarmFade(remindText, alarmInitTime, alarmFadeTime);//두튜윈 재생 시작! 
        
        
        foreach (var item in alarmText)
        {
            if (item.gameObject.activeSelf == true && remindText != item)
            {
                item.rectTransform.position += new Vector3(0, gap, 0);
            }
        }

    }
    void PlayAlarmFade(TextMeshProUGUI text,float initTime,float fadeTime)//두튜윈을 이용해 만들어짐
    {
        DOTween.Kill(text);//이미 붙어있던 두튜윈을 제거해야함
        text.alpha = 1.0f;

        Sequence seq = DOTween.Sequence();
        seq.SetTarget(text);
        
       
        seq.AppendInterval(initTime); // 5초 대기

        seq.Append(
            text.DOFade(0f, fadeTime) // 1초 동안 페이드 아웃
        );

        seq.OnComplete(() =>
        {
            text.gameObject.SetActive(false);//전부 실행 완료시 비활성화함
        });
    }



  



}
