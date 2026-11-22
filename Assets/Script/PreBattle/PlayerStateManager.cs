using UnityEngine;
using TMPro;

public class PlayerStateManager : MonoBehaviour
{
    [Header("Player Status UI")]
    [SerializeField] private TextMeshProUGUI winCountText;
    [SerializeField] private TextMeshProUGUI weekText;
    [SerializeField] private TextMeshProUGUI playerLPText;
    [SerializeField] private TextMeshProUGUI playerGoldText;

    // 지금 플레이어 스탯을 매 프레임마다 업데이트하도록 되어있는데 추후에 수정 필요
    private void Update()
    {
        RunManager run = RunManager.Instance;

        if (run == null)
            return;

        winCountText.text = $"X {run.PlayerWin}";
        weekText.text = $"Week {run.Week}";
        playerLPText.text = $"LP: {run.CurrentLP} / {run.MaxLP}";
        playerGoldText.text = $"{run.PlayerGold}";
    }
}