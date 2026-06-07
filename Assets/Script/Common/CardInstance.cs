using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance  //카드의 정보 카드의 오염 여부나 카드 인스턴스 id가 추가될 수 있음
{
    static int currPointInstanceId=0;
    public CardInstance(CardDataSO cardDataSO)
    {
        instanceId = currPointInstanceId;
        this.CardDataSO = cardDataSO;
        currPointInstanceId++;
    }

    public int instanceId;
    public CardDataSO CardDataSO;

}
