using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EventBus;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClip;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx { CardUse, Charge, Damage, Defense, DefenseSet}

    private void Awake()
    {
        instance = this;
        SoundInit();
    }

    private void Start()
    {
        PlayBGM(true);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.RelocateCardUI>(OnRelocateCard);
        EventBus.Subscribe<EventBus.PlayerAttackSuccess>(OnPlayerAttackSuccess);
        EventBus.Subscribe<EventBus.EnemyAttackSuccess>(OnEnemyAttackSuccess);
        EventBus.Subscribe<EventBus.AlarmText>(OnAlarmText);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.RelocateCardUI>(OnRelocateCard);
        EventBus.Unsubscribe<EventBus.PlayerAttackSuccess>(OnPlayerAttackSuccess);
        EventBus.Unsubscribe<EventBus.EnemyAttackSuccess>(OnEnemyAttackSuccess);
        EventBus.Unsubscribe<EventBus.AlarmText>(OnAlarmText);
    }

    private void SoundInit()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }
    //AudioManager.instance.PlaySfs(Auido.Manager.Sfx.CardUse)

    public void PlayBGM(bool isPlay)
    {
        if (isPlay && !bgmPlayer.isPlaying)
            bgmPlayer.Play();
        else
            bgmPlayer.Stop();
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++) 
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying) 
            {
                continue;
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClip[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
        
    }

    //카드의 위치가 변했을 때 나는 소리
    private void OnRelocateCard(EventBus.RelocateCardUI e)
    {
        // 피치 ( 패 -> 무덤 )
        if (e.from == CommonClass.ZoneType.PlayerHandZone &&
            e.to == CommonClass.ZoneType.PlayerGraveZone)
        {
            PlaySfx(Sfx.Charge);
        }
        if (e.from == CommonClass.ZoneType.EnemyHandZone &&
             e.to == CommonClass.ZoneType.EnemyGraveZone)
        {
            PlaySfx(Sfx.Charge);
        }

        // 공격 ( 패 -> 공격존 )
        if (e.from == CommonClass.ZoneType.PlayerHandZone &&
            e.to == CommonClass.ZoneType.PlayerAttackZone)
        {
            PlaySfx(Sfx.CardUse);
        }
        if (e.from == CommonClass.ZoneType.EnemyHandZone &&
             e.to == CommonClass.ZoneType.EnemyAttackZone)
        {
            PlaySfx(Sfx.CardUse);
        }

        // 방어 세트 ( 패 -> 방어존 )
        if (e.from == CommonClass.ZoneType.PlayerHandZone &&
            e.to == CommonClass.ZoneType.PlayerBlockZone)
        {
            PlaySfx(Sfx.DefenseSet);
        }
        if (e.from == CommonClass.ZoneType.EnemyHandZone &&
             e.to == CommonClass.ZoneType.EnemyBlockZone)
        {
            PlaySfx(Sfx.DefenseSet);
        }
    }

    // 공격 성공
    private void OnPlayerAttackSuccess(EventBus.PlayerAttackSuccess e)
    {
        PlaySfx(Sfx.Damage);
    }
    private void OnEnemyAttackSuccess(EventBus.EnemyAttackSuccess e)
    {
        PlaySfx(Sfx.Damage);
    }
    
    // 방어 성공
    private void OnAlarmText(EventBus.AlarmText e)
    {
        if (e.alarmText == "방어 성공")
            PlaySfx(Sfx.Defense);
    }
}
