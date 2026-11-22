using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BigHoverUICard : MonoBehaviour
{

    //호버시 보일 큰 카드
    private CardView currShowCard;

    [SerializeField] GameObject BigHoverUiCard;
    [SerializeField] TextMeshProUGUI CardNameText;
    [SerializeField] TextMeshProUGUI CardTypeText;
    [SerializeField] TextMeshProUGUI CardCostText;
    [SerializeField] TextMeshProUGUI CardEffectText;
    [SerializeField] TextMeshProUGUI CardBottomAttackNumberText;
    [SerializeField] TextMeshProUGUI CardBottomPowerNumberText;
    [SerializeField] GameObject m_cardIcon;
    [SerializeField] Image illustration;

    [SerializeField]CardDB cardDB;
    //호버시 보일 큰 카드





    public void ChangeCard(CardView card)
    {
        if (card == null || card.isFront == false) return;

        BigHoverUiCard.gameObject.SetActive(true);
        currShowCard=card;


        CardDataSO cardso = cardDB.GetCardSO(card.cardInstance.GetCardId());

        if (card.cardInstance.isCardDataSOVaild() == true)
        {
            CardCostText.text = cardso.cardCost[0].cost.ToString() + " 코스트";

             card.DisplayCost(card.cardInstance.GetCardCost()[0].cost, m_cardIcon);
             CardNameText.text = card.cardInstance.GetCardName();
             CardBottomAttackNumberText.text = card.cardInstance.GetAttack().ToString();
            CardBottomPowerNumberText.text = card.cardInstance.GetPower().ToString();


              CardEffectText.text = "";
              illustration.sprite = card.cardInstance.GetIllustration();

            foreach (var effect in card.cardInstance.GetHitEffects())
            {
                if (effect.conditions.Length != 0)
                {
                     foreach (var condition in effect.conditions)
                    CardEffectText.text += condition.GetEvaluateScript() + "\n";//조건이 있으면 먼저 적음
                }


                 CardEffectText.text += effect.effects.GetCardScript(effect.effectPower) + "\n";//그 카드의 적중효과를 적음


            }
            if (card.cardInstance.GetCardType() == CardDataSO.CardType.Block)
            {
                CardEffectText.text += string.Format("{0}방어력", card.cardInstance.GetBlockPower()) + "\n";
            }
            switch (card.cardInstance.GetCardType())
            {
                case CardDataSO.CardType.Attack:
                     CardTypeText.text = "공격";
                    break;
                case CardDataSO.CardType.Block:
                  CardTypeText.text = "수비";
                    break;
                default:
                   CardTypeText.text = "타입없음";
                    break;
            }
        }
        else
        {
            Debug.Log("카드데이터가 안왔다");
        }
    }

    public void HideCard()
    {
        BigHoverUiCard.gameObject.SetActive(false);
    }
}
