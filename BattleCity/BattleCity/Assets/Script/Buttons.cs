using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Buttons : MonoBehaviour
{
    [SerializeField] GameObject deckViewer;

    [SerializeField] GameObject graveViewer;

    [SerializeField] CardSetter cardSetter;

    [SerializeField] UISetter uisetter;
    //필요한 것들만 대입해줘도 됩니다.


    public void EndPlayerTurnButtonPutted()
    {
        EventBus.Publish<EventBus.EndPlayerTurnEvent>(new EventBus.EndPlayerTurnEvent());
    }

    public void StartPlayerTurnButtonPutted()
    {
        EventBus.Publish<EventBus.StartPlayerTurnEvent>(new EventBus.StartPlayerTurnEvent());
    }


    public void OpenDeckButtonPutted()
    {
        if (deckViewer == null||cardSetter==null) return;
        cardSetter.UpdateAllDeckViewer();
        deckViewer.transform.localScale = Vector3.one;
    }

    public void CloseDeckButtonPutted()
    {
        if (deckViewer == null ) return;

        deckViewer.transform.localScale = Vector3.zero;
    }

    public void OpenGraveButtonPutted()
    {
        if (graveViewer == null || cardSetter == null) return;
        cardSetter.UpdateAllGraveViewer();
        graveViewer.transform.localScale = Vector3.one;
    }
    public void CloseGraveButtonPutted()
    {
        if (graveViewer == null) return;
        graveViewer.transform.localScale = Vector3.zero;
    }

    public void UIAlarmButtonPutted(string text)
    {
        EventBus.Publish<EventBus.AlarmText>(new EventBus.AlarmText { alarmText = text });
    }




    public void AttackPlayerButtonPutted(int dam)
    {
        GameDataManager gameDataManager=MasterManager.Instance.GetManager<GameDataManager>();
        gameDataManager.SetPlayerHp(gameDataManager.currPlayerHp-dam);
    }
    public void AttackEnemyButtonPutted(int dam)
    {
        GameDataManager gameDataManager = MasterManager.Instance.GetManager<GameDataManager>();
        gameDataManager.SetOtherHp(gameDataManager.currOtherHp - dam);
    }
    public void EnergySetButtonPutted(int delta)
    {
        GameDataManager gameDataManager = MasterManager.Instance.GetManager<GameDataManager>();
        gameDataManager.SetPlayerEnergy(gameDataManager.currPlayerEnergy+delta);
    }




}
