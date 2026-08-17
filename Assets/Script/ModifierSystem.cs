using System.Collections;
using System.Collections.Generic;
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
        EventBus.Subscribe<EventBus.FSMChanged> (e_FsmChanged);
        EventBus.Subscribe<EventBus.PlayerAttackSuccess>(e_PlayerAttackSuccess);
        EventBus.Subscribe<EventBus.EnemyAttackSuccess>(e_EnemyAttackSuccess);
        EventBus.Subscribe<EventBus.LogUpdatedComplete>(e_LogUpdated);
    }


    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FsmChanged);
        EventBus.Unsubscribe<EventBus.PlayerAttackSuccess>(e_PlayerAttackSuccess);
        EventBus.Unsubscribe<EventBus.EnemyAttackSuccess>(e_EnemyAttackSuccess);
        EventBus.Unsubscribe<EventBus.LogUpdatedComplete>(e_LogUpdated);
    }
    
     
    

    void e_LogUpdated(EventBus.LogUpdatedComplete e)//로그가 업데이트 될 때마다 여기서 확인
    {
        foreach (var card in cards)//관리중인 카드 모두를 확인
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
    }


    // fsm의 변화에 따라 스텍 감소 처리
    private void e_FsmChanged(EventBus.FSMChanged e)
    {
        if (e.prev == null || e.curr == null) return;

        Debug.Log( $"{e.prev.GetType().Name} -> {e.curr.GetType().Name}");
        if (e.prev is EnemyMainPhaseState&&e.curr is PlayerMainPhaseState || (e.prev is EnemySettingBlockPhaseState && e.curr is PlayerMainPhaseState))
        {
            Debug.Log("1");
            ProcessModifier( IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnStart, false );
            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnStart, true);

            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnEnd, false);
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnEnd, true);

        }
        else if((e.prev is PlayerMainPhaseState && e.curr is EnemyMainPhaseState)||(e.prev is PlayerSettingBlockPhaseState && e.curr is EnemyMainPhaseState))
        {
            Debug.Log("1");
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnStart, false);
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnStart, true);

            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnEnd, false);
            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnEnd, true);
           
        }
        else if (e.prev is PlayerTryBlockPhaseState && e.curr is EnemyMainPhaseState)
        {
            Debug.Log("1");
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TryUseCard, false);
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TryUseCard, true);
        }
        else if (e.prev is EnemyTryBlockPhaseState && e.curr is PlayerMainPhaseState)
        {
            Debug.Log("1");
            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TryUseCard, false);
            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TryUseCard, true);
        }


    }
    /*
    public enum ModifierTrigger
    {
        TurnStart,
        TurnEnd,
        TryUseCard,
        AttackSuccess,
        DamageTaken
    }*/


    // 공격 성공한 대상의 Modifier 처리
    private void e_PlayerAttackSuccess(EventBus.PlayerAttackSuccess e)
    {
        ProcessModifier(
           IModifierOwner.UserType.player,
           Modifier.ModifierTrigger.AttackSuccess,true
        );
        ProcessModifier(
         IModifierOwner.UserType.player,
           Modifier.ModifierTrigger.AttackSuccess, false
      );

    }
    private void e_EnemyAttackSuccess(EventBus.EnemyAttackSuccess e)
    {
        ProcessModifier(
          IModifierOwner.UserType.enemy,
             Modifier.ModifierTrigger.AttackSuccess,true
        );
        ProcessModifier(
       IModifierOwner.UserType.enemy,
          Modifier.ModifierTrigger.AttackSuccess, false
     );

    }




    // 실제 Modifier 감소 처리 
    private void ProcessModifier(
         IModifierOwner.UserType owner,
        Modifier.ModifierTrigger trigger,bool isEntity)
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

                        if ((IModifierOwner.UserType)modifier.trigger != (IModifierOwner.UserType)trigger)
                            continue;//조건이 아니면 넘겨서 생존함.

                        modifier.stack--;//조건에 당하면 스택 1감소

                        if ((IModifierOwner.UserType)modifier.deadTrigger == (IModifierOwner.UserType)trigger)
                            modifier.stack = 0;

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

                        if ((IModifierOwner.UserType)modifier.trigger != (IModifierOwner.UserType)trigger)
                            continue;//조건이 아니면 넘겨서 생존함

                        modifier.stack--;//조건에 당하면 스택 1 감소

                        if ((IModifierOwner.UserType)modifier.deadTrigger == (IModifierOwner.UserType)trigger)
                            modifier.stack = 0;


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