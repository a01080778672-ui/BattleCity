using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
