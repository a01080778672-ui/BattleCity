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
    [SerializeField] TextMeshProUGUI cardCostText;
    [SerializeField] TextMeshProUGUI cardEffectText;

    [SerializeField] TextMeshProUGUI BottomDamageNumberText;
    [SerializeField] TextMeshProUGUI BottomPowerNumbercardEffectText;

    [SerializeField] Transform selectedPos;//카드가 선택되어 약간 위로 가야할때의 위치
    [SerializeField] Transform oriPos;//일반 위치


    [SerializeField] GameObject m_Card;//보이는 카드 자체를 말함
    [SerializeField] GameObject m_FrontSide;//앞면
    [SerializeField] GameObject m_backSide;//카드의 뒷면
    [SerializeField] GameObject m_costIcon;     // 카드 코스트 아이콘
    [SerializeField] Image illustration;//일러스트 부분


    public CardInstance cardInstance { get; private set; }//카드 인스턴스 클래스를 has a 함

    public bool clickAble;


    private bool CLEARMOD_DO_NOT_USE = false;

    public bool clearMod
    {
        get
        {
            return CLEARMOD_DO_NOT_USE;
        }

        set
        {

            if (cardtype is CardType.viewerCard) return;
            CLEARMOD_DO_NOT_USE = value;


            reSizeCard(0.5f);

        }

    }




    private bool HOVERMOD_DO_NOT_USE=false;
    public bool HoverMod
    {
        get
        {
            return HOVERMOD_DO_NOT_USE;
        }

        set
        {
         
            if ( cardtype is CardType.viewerCard) return;
            HOVERMOD_DO_NOT_USE = value;

        
            reSizeCard( 0.1f);

        }

    }

    private bool ATTACKMOD_DO_NOT_USE = false;
    public bool AttackMod //카드를 축소하거나 확대하기 위해서 사용하기위해 만들려했으나 사용하지 않음.
    {
        get
        {
            return ATTACKMOD_DO_NOT_USE;
        }
        set
        {
            if ( cardtype is CardType.viewerCard) return;



            ATTACKMOD_DO_NOT_USE = value;

 

            reSizeCard( 0.5f);
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


              
               
              HoverMod = false;
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

    void reSizeCard(float speed)
    {
        float finalSize = 1.0f;
        if (HoverMod) finalSize *= 1.3f;
        if (AttackMod) finalSize *= 1.65f;  // 배경 일러스트 수정으로 인해 공격 카드 사용 시 커지는 비율 수정 (2.0 -> 1.65)



        if(clearMod) finalSize = 0.1f;

 
        transform.DOScale(Vector3.one*finalSize, speed);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EventBus.FSMChanged>(e_FSMchanged);
        EventBus.Subscribe<EventBus.CardBuffChanged>(e_BuffChanged);
        EventBus.Subscribe<EventBus.EntityBuffChanged>(e_BuffChanged);
        
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventBus.FSMChanged>(e_FSMchanged);
        EventBus.Unsubscribe<EventBus.CardBuffChanged>(e_BuffChanged);
        EventBus.Unsubscribe<EventBus.EntityBuffChanged>(e_BuffChanged);
    }

    void e_FSMchanged(EventBus.FSMChanged e)
    {
        this.selected = false; //상태 변이때마다 선택한 것을 취소시키기로 함.
    }
    void e_BuffChanged(EventBus.EntityBuffChanged e)
    {
        if (e.entity == cardInstance.owner)
        {
            Init(this.cardInstance);
        }
    }

    void e_BuffChanged(EventBus.CardBuffChanged e)
    {
        if(e.card==this.cardInstance)
        {
            Init(this.cardInstance);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)//마우스 올라왔을때
    {
        if(clickAble==false)return;
        EventBus.Publish<EventBus.CardMouseIn>(new CardMouseIn { card = this });

        if (cardtype == CardType.handCard && selected == false)
        {


            HoverMod = true;


        }

      
    }

    public void OnPointerExit(PointerEventData eventData)//마우스 내려왔을때
    {
        if (clickAble == false) return;
        EventBus.Publish<EventBus.CardMouseOut>(new CardMouseOut { card = this });


        if (cardtype == CardType.handCard)
        {
            HoverMod = false;

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

        

        if (cardInstance.isCardDataSOVaild()==true)
        {


            DisplayCost(cardInstance.GetCardCost()[0].cost, m_costIcon);
            CardNameText.text=cardInstance.GetCardName();
            BottomDamageNumberText.text = cardInstance.GetAttack().ToString();
            BottomPowerNumbercardEffectText.text = cardInstance.GetPower().ToString();



            cardEffectText .text = "";
            illustration.sprite = cardInstance.GetIllustration();


            foreach (var effect in cardInstance.GetHitEffects())
            {
                if (effect.conditions.Length != 0)
                {
                    foreach (var condition in effect.conditions)
                    {
                        cardEffectText.text += condition.GetEvaluateScript() + "\n";//우선 조건이 있으면 조건을 적음
                    }
             
                }
                cardEffectText.text += effect.effects.GetCardScript() + "\n";//적중효과를 적음
               
            }

            if (cardInstance.GetCardType() is CardDataSO.CardType.Block)
            {
   
                cardEffectText.text += string.Format("{0}방어력", cardInstance.GetBlockPower()) + "\n";
            }
            switch (cardInstance.GetCardType())
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

    // 카드 코스트 아이콘을 출력하는 함수 추가 ================================== 26.07.03 이재우 업데이트
    public void DisplayCost(int cost, GameObject costIconObj)
    {
        if (costIconObj == null)
            return;

        // 원본 코스트 아이콘의 위치 가져오기
        RectTransform baseRect = costIconObj.GetComponent<RectTransform>();

        if (baseRect == null)
        {
            Debug.LogWarning("costIconObj에 RectTransform이 없습니다.");
            return;
        }

        // 원본 코스트 아이콘을 복사 생성하기 위한 부모 지정
        Transform parent = costIconObj.transform.parent;

        string clonePrefix = "CostIconClone_";

        // 만약 이전에 코스트 아이콘이 이미 생성되어 있었다면 모두 지우기
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);

            if (child.gameObject != costIconObj && child.name.StartsWith(clonePrefix))
            {
                Destroy(child.gameObject);
            }
        }

        // 코스트가 0인 카드는 아이콘 출력하지 않음.
        if (cost <= 0)
        {
            costIconObj.SetActive(false);
            return;
        }

        costIconObj.SetActive(true);

        // 원본 코스트 아이콘의 현재 위치
        Vector2 startPos = baseRect.anchoredPosition;
        float xSpacing = -11f;  // 코스트 아이콘 사이의 간격

        // 카드의 코스트가 1 이라면 원본 아이콘 그대로 사용. 그 이상이라면 코스트 만큼 반복해서 생성
        for (int i = 0; i < cost; i++)
        {
            GameObject iconObj;

            if (i == 0)
            {
                iconObj = costIconObj;
            }
            else
            {
                iconObj = Instantiate(costIconObj, parent);
                iconObj.name = $"{clonePrefix}{i + 1}";
            }

            RectTransform rect = iconObj.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(
                startPos.x + xSpacing * i,
                startPos.y
            );

            rect.localScale = baseRect.localScale;
            rect.sizeDelta = baseRect.sizeDelta;

            iconObj.SetActive(true);
        }
    }





}