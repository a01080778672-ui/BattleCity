using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Player Status")]
    [SerializeField] private int maxHP = 10;
    [SerializeField] private int currentHP = 10;

    [SerializeField] private int maxLP = 20;
    [SerializeField] private int currentLP = 20;

    [SerializeField] private int playerWin = 0;

    [SerializeField] private int week = 0;
    [SerializeField] private int playerGold = 0;

    [Header("Player Deck")]
    [SerializeField] private List<CardInstance> playerDeck = new List<CardInstance>();

    [Header("Player Inventory")]
    [SerializeField] private List<CardInstance> playerInventory = new List<CardInstance>();

    [Header("Game Option")]
    [SerializeField, Range(0f, 1f)] private float volumeBGM = 1f;
    [SerializeField, Range(0f, 1f)] private float volumeSFX = 1f;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public int MaxLP => maxLP;
    public int CurrentLP => currentLP;
    public int PlayerWin => playerWin;
    public int Week => week;
    public int PlayerGold => playerGold;
    public float VolumeBGM => volumeBGM;
    public float VolumeSFX => volumeSFX;

    public IReadOnlyCollection<CardInstance> PlayerDeck => playerDeck;
    public IReadOnlyCollection<CardInstance> PlayerInventory => playerInventory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 체력 변경
    public void ChangeHP(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }

    // 골드 변경
    public void ChangeGold(int amount)
    {
        playerGold = Mathf.Max(0, playerGold + amount);
    }

    // 덱에 카드 추가
    // 추후에 매개변수를 카드 ID로 받도록 수정 필요
    public void AddCardAtDeck(CardInstance card)
    {
        playerDeck.Add(card);
    }

    // 덱에서 카드 제거
    // 추후에 매개변수를 카드 ID로 받도록 수정 필요
    public void RemoveCardAtDeck(CardInstance card)
    {
        playerDeck.Remove(card);
    }

    public void AddCardAtInventory(CardInstance card)
    {
        playerInventory.Add(card);
    }

    public void RemoveCardAtInventory(CardInstance card)
    {
        playerInventory.Remove(card);
    }

    public void SetBGMVolume(float volume)
    {
        volumeBGM = volume;
    }

    public void SetSFXVolume(float volume)
    {
        volumeSFX = volume;
    }
}
