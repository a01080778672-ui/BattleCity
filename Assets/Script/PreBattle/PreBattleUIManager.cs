using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreBattleUIManager : MonoBehaviour
{
    private string[] enemyNameList = { "Enemy1", "Enemy2", "Enemy3" };
    public TextMeshProUGUI NextEnemyNameText;

    int i = 0;

    [Header("Game Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Update()
    {
        NextEnemyNameText.text = enemyNameList[i];
    }

    public void InventoryOpen()
    {
        Debug.Log("인벤토리 창 활성화");
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

    public void BattleStart()
    {
        Debug.Log("게임 시작");
        SceneManager.LoadScene(gameSceneName);
    }
}
