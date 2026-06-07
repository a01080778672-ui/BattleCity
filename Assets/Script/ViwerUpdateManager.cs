using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViwerUpdateManager : MonoBehaviour //무덤창, 덱 창이 열리려 할때 현 정보를 바탕으로 업데이트를 해준다.
{

    [SerializeField] GameLoopData _data;
    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.DeckOpenButtonClicked>(e_DeckOpen);
        EventBus.Subscribe<EventBus.GraveOpenButtonClicked>(e_GraveOpen);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.DeckOpenButtonClicked>(e_DeckOpen);
        EventBus.Unsubscribe<EventBus.GraveOpenButtonClicked>(e_GraveOpen);
    }
    void e_DeckOpen(EventBus.DeckOpenButtonClicked e)
    {
        EventBus.Publish<EventBus.UpdateDeck>(new EventBus.UpdateDeck { cards = _data.player.DeckCards });
    }
    void e_GraveOpen(EventBus.GraveOpenButtonClicked e)
    {
        EventBus.Publish<EventBus.UpdateGrave>(new EventBus.UpdateGrave { cards = _data.player.GraveCards });
    }
}
