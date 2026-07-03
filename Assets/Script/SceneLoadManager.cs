using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private string gameClearSceneName = "GameClearScene";  // 게임 클리어 씬
    [SerializeField] private string gameOverSceneName = "GameOverScene";    // 게임 오버 씬
    [SerializeField] GameLoopData gameLoopData;

    public void Update()
    {
        if (gameLoopData.player.currHp <= 0)
        {
            LoadGameOverScene();
        }
        else if(gameLoopData.enemy.currHp <= 0)
        {
            LoadGameClearScene();
        }
    }

    public void LoadGameClearScene()
    {
        SceneManager.LoadScene(gameClearSceneName);
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

}
