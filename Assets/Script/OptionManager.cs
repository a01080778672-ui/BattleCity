using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionManager : MonoBehaviour
{
    [Header("Option UI")]
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;


    private void Start()
    {
        RunManager run = RunManager.Instance;

        if (run == null)
        {
            Debug.LogError("RunManager가 존재하지 않습니다.");
            return;
        }

        // 저장된 볼륨을 슬라이더에 적용
        BGMSlider.SetValueWithoutNotify(run.VolumeBGM);
        SFXSlider.SetValueWithoutNotify(run.VolumeSFX);
    }

    public void OpenOption()
    {
        optionMenu.SetActive(true);
    }

    public void CloseOption()
    {
        optionMenu.SetActive(false);
    }

    public void SetBGMVolume(float volume)
    {
        if (RunManager.Instance == null)
            return;

        RunManager.Instance.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (RunManager.Instance == null)
            return;

        RunManager.Instance.SetSFXVolume(volume);
    }
}