using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class ModifierUISetter : MonoBehaviour
{
  
    [SerializeField] GameObject buffViewPref;//프리펩.

    [SerializeField] GameObject playerBuffParent;//이것위에 자식으로 존재할듯함
    [SerializeField] GameObject enemyBuffParent;//이것위에 자식으로 존재할듯함


    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.EntityBuffChanged>(e_UpdateEntityBuff);
    }


    void e_UpdateEntityBuff(EventBus.EntityBuffChanged e)
    {
        if(e.entity.type==IModifierOwner.UserType.player&&playerBuffParent!=null)
        {
            foreach (Transform prevBuff in playerBuffParent.transform)
            {
                Destroy(prevBuff.gameObject);
            }


            foreach (Modifier item in e.entity.currBuff)
            {
                GameObject newBuffIcon = Instantiate(buffViewPref);
                newBuffIcon.transform.SetParent(playerBuffParent.transform,false);

                newBuffIcon.GetComponentInChildren<TextMeshProUGUI>().text = item.stack.ToString();


            }



        }
        else if(e.entity.type == IModifierOwner.UserType.enemy&&enemyBuffParent!=null)
        {
            foreach (Transform prevBuff in enemyBuffParent.transform)
            {
                Destroy(prevBuff);
            }


            foreach (Modifier item in e.entity.currBuff)
            {
                GameObject newBuffIcon = Instantiate(buffViewPref);
                newBuffIcon.transform.SetParent(enemyBuffParent.transform, false);


                newBuffIcon.GetComponent<TextMeshProUGUI>().text = item.stack.ToString();

            }

        }
    }

}
