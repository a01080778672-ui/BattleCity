using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using static UnityEditor.Progress;

public class ModifierSystem : MonoBehaviour
{
    // 플레이어와 이너미
    private List<IModifierOwner> entities = new();

    // Modifier가 붙은 카드만 관리
    private List<IModifierOwner> cards = new();

    [SerializeField] GameLoopData _data;
    public void EntityRegister(IModifierOwner owner)
    {
        if (!entities.Contains(owner))
            entities.Add(owner);
    }


    public void EntityUnRegister(IModifierOwner owner)
    {
        entities.Remove(owner);
    }


    public void CardRegister(IModifierOwner owner)
    {
        if (!cards.Contains(owner))
            cards.Add(owner);
    }


    public void CardUnRegister(IModifierOwner owner)
    {
        cards.Remove(owner);
    }



    private void OnEnable()
    {
      
        EventBus.Subscribe<EventBus.LogUpdatedComplete>(e_LogUpdated);
    }


    private void OnDisable()
    {
     
        EventBus.Unsubscribe<EventBus.LogUpdatedComplete>(e_LogUpdated);
    }
    
     
    

    void e_LogUpdated(EventBus.LogUpdatedComplete e)//로그가 업데이트 될 때마다 여기서 확인
    {
        //그리고, 각 버프별 로그 업데이트시마다 스택감소,삭제,효과발동 을 처리하도록 해야함
        foreach (var item in cards)//관리하는 카드들(굳이 2개로 나뉘어졌다.)
        {
            if (item is CardInstance currCardInstance)//다운캐스팅?
            {

                for (int i = item.currBuff.Count - 1; i >= 0; i--)//그 카드가 가진 모든 버프를 순회함
                {
                    Modifier modifier = item.currBuff[i];

                    CardContext newContext = new CardContext(currCardInstance, null, _data, currCardInstance.owner, _data.GetOtherEntity(currCardInstance.owner), _data.battleLogs);

                    foreach (var currTrigger in modifier.consumptionTrigger)//모든 감소 트리거를 일단 확인함
                    {
                        if (currTrigger.Evaluate(newContext))
                        {
                            modifier.stack--;//조건에 당하면 스택 1감소
                            continue;//1스택 감소는 1번만 
                        }
                    }

                    foreach (var currTrigger in modifier.deadTrigger)//즉시 감소 트리거도 전부 확인
                    {
                        if (currTrigger.Evaluate(newContext))
                        {
                            modifier.stack = 0;//데드트리거는 즉시 끝
                            continue;//1번만 
                        }
                    }

                    if (modifier.stack <= 0)
                    {
                        item.RemoveBuff(i);
                    }
                    else
                    {
                        //만약 살아있다면, 각 버프는 추가 발동효과도 가지고 있는것을 계산합니다.
                        foreach (var currTrigger in modifier.playTrigger)
                        {
                            if (currTrigger.Evaluate(newContext))
                            {
                                foreach (var effect in modifier.buffEffect)
                                {
                                    effect.Execute(newContext);
                                }

                                continue;
                            }
                        }
                    }
                       

                }
            }
        }

        foreach (var item in entities)// 엔티티들도 나중에 해준다
        {
            if (item is EntityInstance currEntity)//다운캐스팅?
            {
                for (int i = item.currBuff.Count - 1; i >= 0; i--)//그 엔티티가 가진 모든 버프를 순회함
                {
                    Modifier modifier = item.currBuff[i];

                    CardContext newContext = new CardContext(null, null, _data,currEntity, _data.GetOtherEntity(currEntity), _data.battleLogs);

                    foreach (var currTrigger in modifier.consumptionTrigger)//모든 감소 트리거를 일단 확인함
                    {
                        if (currTrigger.Evaluate(newContext))
                        {
                            modifier.stack--;//조건에 당하면 스택 1감소
                            continue;//1스택 감소는 1번만 
                        }
                    }

                    foreach (var currTrigger in modifier.deadTrigger)//즉시 감소 트리거도 전부 확인
                    {
                        if (currTrigger.Evaluate(newContext))
                        {
                            modifier.stack = 0;//데드트리거는 즉시 끝
                            continue;//1번만 
                        }
                    }

                    if (modifier.stack <= 0)
                    {
                        item.RemoveBuff(i);
                    }
                    else
                    {
                        //만약 살아있다면, 각 버프는 추가 발동효과도 가지고 있는것을 계산합니다.
                        foreach (var currTrigger in modifier.playTrigger)
                        {
                            if (currTrigger.Evaluate(newContext))
                            {
                                foreach (var effect in modifier.buffEffect)
                                {
                                    effect.Execute(newContext);
                                }

                                continue;
                            }
                        }
                    }
                }

            }

        }

        foreach (var card in cards)//관리중인 카드 모두를 확인 카드에 로그가 업데이트 될떄마다 발동할까 말까 하는 그런게 있어서 그걸 처리함
        {
            if (card is CardInstance currCardInstance)//다운캐스팅?
            {

                CardContext context = new CardContext(currCardInstance, null, _data, currCardInstance.owner, null, _data.battleLogs);


                foreach (var effect in currCardInstance.GetLogUpdateEffects())
                {
                    bool pass = true;
                    foreach (var item in effect.conditions)
                    {
                        if (item.Evaluate(context) == false)
                        { pass = false; break; }

                    }
                    if (pass)
                    {

                        effect.effects.Execute(context);//조건이 맞다면 그  효과를  수행합니다.
                    }

                }


            }




        }
        //관리중인 카드 모두를 확인 카드에 로그가 업데이트 될떄마다 발동할까 말까 하는 그런게 있어서 그걸 처리함

    }







    // 실제 Modifier 감소 처리 
    private void ProcessModifier(
         IModifierOwner.UserType owner,
        ConditionSO trigger,bool isEntity)
    {
  

        if (isEntity)
        {
            foreach (var item in entities)
            {
           
                if (item.type == owner)
                {
                
                    for (int i = item.currBuff.Count - 1; i >= 0; i--)
                    {
                        Modifier modifier = item.currBuff[i];

                        foreach (var currTrigger in modifier.consumptionTrigger)//모든 감소 트리거를 일단 확인함
                        {
                            if(currTrigger==trigger)
                            {
                                modifier.stack--;//조건에 당하면 스택 1감소
                                continue;//1스택 감소는 1번만 
                            }
                        }

                        foreach (var currTrigger in modifier.deadTrigger)//즉시 감소 트리거도 전부 확인
                        {
                            if (currTrigger == trigger)
                            {
                                modifier.stack = 0;//데드트리거는 즉시 끝
                                continue;//1번만 
                            }
                        }

                        if (modifier.stack <= 0)
                        {
                            item.RemoveBuff(i);
                        }
                    }




                }
            }
        }
        else
        {
            foreach (var item in cards)
            {

                if (item.type == owner)
                {
                    for (int i = item.currBuff.Count - 1; i >= 0; i--)
                    {
                        Modifier modifier = item.currBuff[i];

                        foreach (var currTrigger in modifier.consumptionTrigger)//모든 감소 트리거를 일단 확인함
                        {
                            if (currTrigger == trigger)
                            {
                                modifier.stack--;//조건에 당하면 스택 1감소
                                continue;//1스택 감소는 1번만 
                            }
                        }

                        foreach (var currTrigger in modifier.deadTrigger)//즉시 감소 트리거도 전부 확인
                        {
                            if (currTrigger == trigger)
                            {
                                modifier.stack = 0;//데드트리거는 즉시 끝
                                continue;//1번만 
                            }
                        }


                        if (modifier.stack <= 0)
                        {
                            item.RemoveBuff(i);
                        }
                    }




                }
            }
        }


      


    
    }



    /* 
    public void ProcessAllEntityModifier(
        ModifierTrigger trigger)
    {
        foreach (var entity in entities)
        {
            ProcessModifier(entity, trigger);
        }
    }


    //
    public void ProcessAllCardModifier(
        ModifierTrigger trigger)
    {
        foreach (var card in cards)
        {
            ProcessModifier(card, trigger);
        }
    }
    */

 

}