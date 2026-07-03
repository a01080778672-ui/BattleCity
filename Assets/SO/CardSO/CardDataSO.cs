using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardDataSO", menuName = "SO/CardDataSO")]
public class CardDataSO : ScriptableObject
{
    /*CardData 안에는 
     
카드 이름(name),
비용(cost), 
공격력(atk),
방어력(def),
색(color),
카드 타입(type),
효과 목록(eff),
효과 설명 텍스트(des)가 들어감
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



    [System.Serializable]
    public struct Cost
    {
        public ColorType color;
        public int cost;
    }

    [System.Serializable]
    public struct Bouns
    {
        public EffectSO effects;
        public ConditionSO[] conditions;
    }


    public int attack;//적중시 줄 기본 데미지
    public int power;//파워(내구도)
    public int blockPower;


    public string cardName;
    public Cost[] cardCost;//코스트가 여러 종류일수도 있다 하였으므로..
    public Sprite illustration;//일러스트
    public ColorType colorType;
    public CardType type;//카드의 타입 설정. 이것을 기반해 이름이 정해진다.
    public List<Bouns> hitEffects;//적중 효과. 이제는 조건과 함께 묶여서 지낸다.


    public int cardId;//내가 따로 추가함. 카드의 고유 아이디

    public string cardDescription;//카드에 적힐 잡다한 설명글. 주의!: 카드 효과 string은 각 EffectSO에 있음. 다형성이 있기에 효과별 다른 설명을 제공함

}