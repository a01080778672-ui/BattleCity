using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStateManager : MonoBehaviour
{
    private int winCount = 0;
    private int dayCount = 1;
    private int playerLP = 20;

    public TextMeshProUGUI winCountText;
    public TextMeshProUGUI dayCountText;
    public TextMeshProUGUI playerLPText;

    private void Start()
    {
        if (winCount < 0)
            winCount = 0;
        if (dayCount < 0)
            dayCount = 0;
        if (playerLP < 0)
            playerLP = 0;
    }

    private void Update()
    { 
        winCountText.text = $"X {winCount}";
        dayCountText.text = $"Day {dayCount}";
        playerLPText.text = $"LP: {playerLP} / 20";
    }


    // 디버그 용 함수
    public void WinCountAdd()
    {
        winCount++;
    }

    public void WinCountSub()
    {
        if (winCount > 0)
            winCount--;
    }

    public void DayCountAdd()
    {
        dayCount++;
    }

    public void DayCountSub()
    {
        if (dayCount > 1)
            dayCount--;
    }


    public void PlayerLPAdd()
    {
        if (playerLP < 20)
            playerLP++;
    }

    public void PlayerLPSub()
    {
        if (playerLP > 0)
            playerLP--;
    }
}
