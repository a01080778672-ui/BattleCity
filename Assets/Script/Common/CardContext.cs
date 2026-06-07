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
public class CardContext  //일단 필수정보만 있으며 나중에 추가할수도 있음
{
   
    public CardInstance usedCard;//쓰여진 카드는?
    public GameState fsmState;//현재 게임이 무슨 상태인지를 담는다FSM으로 만들어져있다.




    public EntityInstance usedEntity;//카드를 쓴 사람 (아직 사용되지 않음)
    public EntityInstance targetEntity;//선택당한 대상 (아직 사용되지 않음)
   
}
