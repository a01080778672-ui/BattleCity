using DG.Tweening; 
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CardDataSO;
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
    [SerializeField] TextMeshProUGUI cardCostText;
    [SerializeField] TextMeshProUGUI cardEffectText;

    [SerializeField] Transform selectedPos;//카드가 선택되어 약간 위로 가야할때의 위치
    [SerializeField] Transform oriPos;//일반 위치


    [SerializeField] GameObject m_Card;//보이는 카드 자체를 말함
    [SerializeField] GameObject m_FrontSide;//앞면
    [SerializeField] GameObject m_backSide;//카드의 뒷면
    [SerializeField] GameObject m_cardIllustration;//카드 일러스트

    [SerializeField] Sprite m_costIcon;//코스트 아이콘

    public CardInstance cardInstance { get; private set; }//카드 인스턴스 클래스를 has a 함

    public bool clickAble;
    private bool REDUCTION_DO_NOT_USE = false;
    public bool reduction //카드를 축소하거나 확대하기 위해서 사용하기위해 만들려했으나 사용하지 않음.
    {
        get
        {
            return REDUCTION_DO_NOT_USE;
        }
        set
        {
            if (SELECTED_DO_NOT_USE == value || cardtype == CardType.viewerCard) return;

            REDUCTION_DO_NOT_USE = value;
            if (value==true)//
            {
                RectTransform rect = GetComponent<RectTransform>();
                rect.DOSizeDelta(new Vector2(100.0f, 150.0f), 0.5f); // 너비, 높이를 150으로 0.5초에
                
              
            }
            else//
            {
                RectTransform rect = GetComponent<RectTransform>();
                rect.DOSizeDelta(new Vector2(100.0f, 150.0f), 0.5f); // 너비, 높이를 150으로 0.5초에

               
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
                m_Card.transform.localPosition = new Vector3(0, 0, 0);
                m_Card.transform.DOLocalMove(new Vector3(0, 20f, 0), 0.2f);


              

               transform.localScale = Vector3.one;
            }
            else if(value==false)
            {
                m_Card.transform.DOLocalMove(new Vector3(0,0,0), 0.2f);


          
            }


        }}//프로퍼티를 이용해,함수가 true가 되거나 false가 될때 특정한 로직을 같이 곁들어 실행하도록 강제할 수 있다

    private bool ISFRONT_DO_NOT_USE = false;
    public bool isFront //카드 회전 연출용
    {
        get
        {
            return ISFRONT_DO_NOT_USE;
        }
        set
        {
            if (ISFRONT_DO_NOT_USE == value || cardtype == CardType.viewerCard|| m_backSide==null) return;

            if (value == true)//여기오면 앞으로 뒤집기
            {
                RectTransform rect = m_Card. GetComponent<RectTransform>();

                Image backImg = m_backSide.GetComponent<Image>();
     

                rect.DOLocalRotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() => { backImg.enabled = false; rect.DOLocalRotate(new Vector3(0, 0, 0), 0.25f); });
         

            }
            else//여기오면 뒤로 뒤집기
            {
                RectTransform rect = m_Card.GetComponent<RectTransform>();
    
                Image backImg = m_backSide.GetComponent<Image>();

                rect.DOLocalRotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() => {  backImg.enabled = true; rect.DOLocalRotate(new Vector3(0, 0, 0), 0.25f);  } );
            }


            ISFRONT_DO_NOT_USE=value;
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMchanged);
        
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMchanged);
    
    }

    void e_FSMchanged(EventBus.FSMChanged e)
    {
        this.selected = false; //상태 변이때마다 선택한 것을 취소시키기로 함.
    }



    public void OnPointerEnter(PointerEventData eventData)//마우스 올라왔을때
    {
        if(clickAble==false)return;
        EventBus.Publish<EventBus.CardMouseIn>(new CardMouseIn { card = this });

        if (cardtype == CardType.handCard && selected == false)
        {
            transform.localScale = Vector3.one * 1.3f;
   
        }

      
    }

    public void OnPointerExit(PointerEventData eventData)//마우스 내려왔을때
    {
        if (clickAble == false) return;
        EventBus.Publish<EventBus.CardMouseOut>(new CardMouseOut { card = this });


        if (cardtype == CardType.handCard)
        {
            transform.localScale = Vector3.one;
           
        }
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
            // cardCostText.text = cardso.cardCost[0].cost.ToString() + " 코스트";

            // 카드 코스트 아이콘 출력 함수
            DisplayCost(cardso.cardCost[0].cost, m_costIcon, m_FrontSide.transform);
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

    // 카드 우측 상단에 코스트만큼 아이콘 띄우는 함수 추가 - 26.06.30 이재우
    public void DisplayCost(int cost, Sprite costIcon, Transform parent)
    {
        if (cost <= 0 || costIcon == null)
            return;

        // 기존 코스트 아이콘 제거
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (parent.GetChild(i).name.StartsWith("코스트 아이콘"))
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        Vector2 startPos = new Vector2(48f, 73.5f);
        float xSpacing = -11f;

        for (int i = 0; i < cost; i++)
        {
            GameObject iconObj = new GameObject($"CostIcon_{i + 1}");

            iconObj.transform.SetParent(parent, false);

            RectTransform rect = iconObj.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                startPos.x + xSpacing * i,
                startPos.y
            );

            rect.localScale = Vector3.one * 0.1f;

            Image image = iconObj.AddComponent<Image>();
            image.sprite = costIcon;
            image.raycastTarget = false;
        }
    }
    
   


  
}
