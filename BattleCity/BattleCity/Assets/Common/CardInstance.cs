using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardDataManager;

public class CardInstance 
{
  
    public CardInstance(CardDataSO cardDataSO,int instanceID)
    {
        this.CardDataSO = cardDataSO;
        this.instanceId = instanceID;



    }
    

    public CardDataSO CardDataSO;
    public int instanceId;
}
