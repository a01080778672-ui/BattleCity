using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierSystem : MonoBehaviour
{
    // 전체 대상 처리가 필요한 경우 사용
    private List<IModifierOwner> entities = new();

    // Modifier가 붙은 카드만 관리
    private List<IModifierOwner> cards = new();


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
    }


    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FsmChanged);
        EventBus.Unsubscribe<EventBus.PlayerAttackSuccess>(e_PlayerAttackSuccess);
        EventBus.Unsubscribe<EventBus.EnemyAttackSuccess>(e_EnemyAttackSuccess);
    }



    // fsm의 변화에 따라 스텍 감소 처리
    private void e_FsmChanged(EventBus.FSMChanged e)
    {
        if(e.prev is EnemyMainPhaseState&&e.curr is PlayerMainPhaseState)
        {
            ProcessModifier( IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnStart, false );
            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnStart, true);

            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnEnd, false);
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnEnd, true);

        }
        else if(e.prev is PlayerMainPhaseState && e.curr is EnemyMainPhaseState)
        {
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnStart, false);
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TurnStart, true);

            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnEnd, false);
            ProcessModifier(IModifierOwner.UserType.player, Modifier.ModifierTrigger.TurnEnd, true);
        }
        else if (e.prev is PlayerTryBlockPhaseState && e.curr is EnemyMainPhaseState)
        {
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TryUseCard, false);
            ProcessModifier(IModifierOwner.UserType.enemy, Modifier.ModifierTrigger.TryUseCard, true);
        }
        else if (e.prev is EnemyTryBlockPhaseState && e.curr is PlayerMainPhaseState)
        {
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
                            continue;

                        modifier.stack--;

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
                            continue;

                        modifier.stack--;

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