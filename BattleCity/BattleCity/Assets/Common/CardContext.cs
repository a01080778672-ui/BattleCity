using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/*
 * CardContext는 카드 사용 가능 판정과 효과 실행에 필요한 상황 정보를 담는다.
함수보다는 상황 데이터 객체로 보는 게 자연스럽다.
사용 플레이어,
상대 플레이어,
사용하려는 카드,
선택한 대상,
현재 에너지,
현재 턴 정보, 
현재 게임 상태 등의 카드를 사용할 때 필요한 주변 정보를 제공한다.
*/
public class CardContext 
{
    public GameObject usedPlayer;//나
    public GameObject otherPlayer;//다른 플레이어
    public CardDataSO usedCard;//쓰여진 카드는?
    public GameObject selected;//선택당한 대상
    public GameState gameState;//현재 게임이 무슨 상태인지를 담는다FSM으로 만들어져있다.
    public int currEnerge;//현재 에너지
    public int currTurnCount;//현재 몇턴인가



}
