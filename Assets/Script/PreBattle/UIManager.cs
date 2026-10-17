using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private string[] enemyNameList = { "Enemy1", "Enemy2", "Enemy3" };
    public TextMeshProUGUI NextEnemyNameText;

    int i = 0;

    private void Update()
    {
        NextEnemyNameText.text = enemyNameList[i];
    }

    public void InventoryOpen()
    {
        Debug.Log("인벤토리 창 활성화");
    }

    public void OptionOpen()
    {
        Debug.Log("옵션 창 활성화");
    }

    public void NextEnemy()
    {
        if (i < enemyNameList.Length - 1)
            i++;
    }

    public void PrevEnemy()
    {
        if (i > 0)
            i--;
    }
}
