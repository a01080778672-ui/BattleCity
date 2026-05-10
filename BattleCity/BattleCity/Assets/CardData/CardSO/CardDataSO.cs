using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardDataSO", menuName = "SO/CardDataSO")]
public class CardDataSO : ScriptableObject
{
    /*CardData 안에는 
     * 카드 이름(name),
     * 비용(cost), 
     * 공격력(atk),
     * 방어력(def),
     * 색(color),
     * 카드 타입(type), 
     * 효과 목록(eff),
     * 효과 설명 텍스트(des)가 들어감
*/
    public enum CardType
    {
        None,
        Attack,
        Block
    }
    public enum ColorType
    {
        red,
        blue
    }
    public enum EffectType
    {
        attack,
        block   
    }


    [System.Serializable]
    public struct Cost
    {
        public ColorType color;
        public int cost;
    }

   


    public string cardName;
    public Cost[]cardCost;//코스트가 여러 종류일수도 있다 하였으므로..
    public int attackPower;
    public int defendPower;
    public ColorType colorType;
    public CardType type;
    public EffectType[] simpleEffect;//즉시 공격, 즉시 쉴드 추가 같은 간단한 효과
    //public EffectSO[] effects;//똥카드 넣기나 디버프 추가같은 복잡한 효과
    public string effectScriptTemplate;




    public int cardId;//내가 따로 추가함. 카드의 고유 아이디
   
    public string cardDescriptionTemplate;//내가 따로 추가함. 카드에 적힐 설명글
   
}
