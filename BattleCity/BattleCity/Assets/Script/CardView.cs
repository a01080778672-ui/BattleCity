using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; 
using static EventBus;


/*
 
    IPointerClickHandler,    // 클릭
    IPointerEnterHandler,    // 마우스 올라왔을 때
    IPointerExitHandler,     // 마우스 나갔을 때
    IPointerDownHandler,     // 누르는 순간
    IPointerUpHandler,       // 떼는 순간
    IDragHandler,            // 드래그 중
    IBeginDragHandler,       // 드래그 시작
    IEndDragHandler          // 드래그 끝
 
 */

public class CardView : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler     //카드 하나하나마다 부착될 카드 컴포넌트이다.
{
    enum CardType
    {
        handCard,
        viewerCard
    }
    [SerializeField] CardType cardtype;


 
    [SerializeField] TextMeshProUGUI CardNameText;
    [SerializeField] TextMeshProUGUI CardTypeText;
    [SerializeField] TextMeshProUGUI cardCostText;//임시로 만듦
    [SerializeField] TextMeshProUGUI cardPowerText;//임시로 만듦


    CardDataSO cardDataSO;//현제 내가 보일 카드의 데이터를 여기에 저장한다.

    [SerializeField] Transform selectedPos;//카드가 선택되어 약간 위로 가야할때의 위치
    [SerializeField] Transform oriPos;//일반 위치

    [SerializeField] Image m_cardImg;//내 자식인 카드 이미지

    public CardInstance cardInstance { get; private set; }//카드 인스턴스 클래스를 has a 함

    public bool clickAble;


    public int myID//id를 얻어올때는 현재 SO의 카드 데이터를 가져오도록 한다.
    {
        get
        {
            if (cardDataSO == null) return -1;
            return cardDataSO.cardId;
        }
    }

    private bool SELECTED_DO_NOT_USE=false;
    public bool selected {
        get
        {
            return SELECTED_DO_NOT_USE;
        }
        set
        {
            if (SELECTED_DO_NOT_USE == value || cardtype == CardType.viewerCard) return;
            if (clickAble == false) return;

            SELECTED_DO_NOT_USE = value;

            if(value==true)//여기로 왔다면 거짓->참 이 되었을것.
            {
                m_cardImg.transform.position=oriPos.position;
                m_cardImg.transform.DOMove(selectedPos.position, 0.2f);

               transform.localScale = Vector3.one;
            }
            else if(value==false)
            {
                m_cardImg.transform.DOMove(oriPos.position, 0.2f);
            }


        }}//프로퍼티를 이용해,함수가 true가 되거나 false가 될때 특정한 로직을 같이 곁들어 실행하도록 강제할 수 있다



    public void OnPointerEnter(PointerEventData eventData)//마우스 올라왔을때
    {
        if(clickAble==false)return;

       if(cardtype == CardType.handCard&& selected == false)
       transform.localScale=Vector3.one*1.3f;

      
    }

    public void OnPointerExit(PointerEventData eventData)//마우스 내려왔을때
    {
        if (clickAble == false) return;

        if (cardtype == CardType.handCard )
            transform.localScale = Vector3.one;
    }
    public void OnPointerDown(PointerEventData eventData)//그것을 누르는 순간
    {
        if (clickAble == false) return;

        if (cardtype == CardType.handCard)//만약 손카드에 나온 카드라면
        {
            if (eventData.button == PointerEventData.InputButton.Left)//좌클릭
            {
                EventBus.Publish<CardLeftClickedEvent>(new CardLeftClickedEvent { card = this });
            }
            else if (eventData.button == PointerEventData.InputButton.Right)//우클릭
            {
                EventBus.Publish<CardRightClickedEvent>(new CardRightClickedEvent { card = this });
            }
        }
    }
    public void Init(CardDataSO cardso,CardInstance cardInstance)
    {
        selected = false;//프로퍼티에 초기화
        clickAble = true;

        if (cardso != null)
        {
            cardCostText.text = cardso.cardCost[0].cost.ToString() + " 코스트";
            CardNameText.text=cardso.cardName;
            switch (cardso.type)
            {
                case CardDataSO.CardType.Attack:
                  CardTypeText.text = "공격";
                  cardPowerText.text = cardso.attackPower.ToString() + " 데미지";
                    break;


                case CardDataSO.CardType.Block:
                    CardTypeText.text = "수비";
                    cardPowerText.text = cardso.defendPower.ToString() + " 수비력";
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
        if(cardInstance != null)
        {
            this.cardInstance = cardInstance;
        }
        else
        {
            Debug.Log("카드데이터가 안왔다");
        }

        this.cardDataSO = cardso;
    }

  
}
