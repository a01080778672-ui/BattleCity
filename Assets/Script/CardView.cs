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
    [SerializeField] TextMeshProUGUI cardEffectText;//임시로 만듦

    [SerializeField] Transform selectedPos;//카드가 선택되어 약간 위로 가야할때의 위치
    [SerializeField] Transform oriPos;//일반 위치

    [SerializeField] Image m_cardImg;//내 자식인 카드 이미지

    public CardInstance cardInstance { get; private set; }//카드 인스턴스 클래스를 has a 함

    public bool clickAble;
    private bool REDUCTION_DO_NOT_USE = false;
    public bool reduction //카드를 축소하거나 확대하기 위해서 사용
    {
        get
        {
            return REDUCTION_DO_NOT_USE;
        }
        set
        {
            if (SELECTED_DO_NOT_USE == value || cardtype == CardType.viewerCard) return;
            if (clickAble == false) return;
            if (value==true)//여기오면 축소해야함
            {
                RectTransform rect = GetComponent<RectTransform>();
                rect.DOSizeDelta(new Vector2(100.0f, 100.0f), 0.5f); // 너비, 높이를 100으로 0.5초에

                CardNameText.gameObject.SetActive(false);
                CardTypeText.gameObject.SetActive(false);
            }
            else//여기오면 확대해야함
            {
                RectTransform rect = GetComponent<RectTransform>();
                rect.DOSizeDelta(new Vector2(100.0f, 150.0f), 0.5f); 

                CardNameText.gameObject.SetActive(true);
                CardTypeText.gameObject.SetActive(true);

            }
        }
    }


    private bool SELECTED_DO_NOT_USE=false;
    public bool selected {//참일시 카드가 살짝 위로 감
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
                m_cardImg.transform.localPosition = new Vector3(0, 0, 0);
                m_cardImg.transform.DOLocalMove(new Vector3(0, 20f, 0), 0.2f);


                //m_cardImg.transform.position=oriPos.position;
                //m_cardImg.transform.DOMove(selectedPos.position, 0.2f);

               transform.localScale = Vector3.one;
            }
            else if(value==false)
            {
                m_cardImg.transform.DOLocalMove(new Vector3(0,0,0), 0.2f);


                //m_cardImg.transform.DOMove(oriPos.position, 0.2f);
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

        if (cardtype == CardType.handCard)//만약 손카드라면
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
    public void Init(CardInstance cardInstance)//카드 초기화 함수
    {
        selected = false;//프로퍼티에 초기화
        clickAble = true;

        CardDataSO cardso = cardInstance.CardDataSO;

        if (cardso != null)
        {
            cardCostText.text = cardso.cardCost[0].cost.ToString() + " 코스트";
            CardNameText.text=cardso.cardName;

            cardEffectText .text = "";
            foreach (var effect in cardso.hitEffects)
            {
                cardEffectText.text += effect.GetCardScript() + "\n";
                if(cardso.type==CardDataSO.CardType.Block)
                {
                    cardEffectText.text += string.Format("{0}방어력",cardso.blockPower) + "\n";
                }
            }
            switch (cardso.type)
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
        if(cardInstance != null)
        {
            this.cardInstance = cardInstance;
        }
        else
        {
            Debug.Log("카드데이터가 안왔다");
        }
    }

  
}
