using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardDB", menuName = "SO/CardDB")]
public class CardDB : ScriptableObject
{

    [SerializeField] private CardDataSO[] cards;

    private Dictionary<int, CardDataSO> cardTable;//딕셔너리


    private void OnEnable()//awake쓰려했는데 스크립트블이면 onenable이 더 안전하다고 하여 씀
    {
        cardTable = new Dictionary<int, CardDataSO>();

        for (int i = 0; i < cards.Length; i++)
        {
            if(cardTable.ContainsKey(cards[i].cardId))
            {
                Debug.Log("저런... 인스펙터에 이상하게 연결했구나 ");
            }


            cardTable[cards[i].cardId] = cards[i]; //딕셔너리의 키값엔 카드 아이디가, 값 부분엔 카드 자체가 매칭된다.
        }
    }
    public CardDataSO GetCardSO(int id)
    {
        if (cardTable.TryGetValue(id, out var card))
            return card;

        return null;
    }
}
