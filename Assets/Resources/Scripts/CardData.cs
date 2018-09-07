using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CARD_TYPE
{
    NONE = 0,
    MAHJONG,
    PHOENIX,
    DRACHE,
    DASHUND
}

public enum CARD_COLOR
{
    RED = 0,
    GREEN,
    BLACK,
    BLUE,
    NONE
}

public class CardData : MonoBehaviour
{
    public Text leftText;
    public Text rightText;

    public Image leftIcon;
    public Image rightIcon;
    public Image background;
    public Text centerText;

    public CARD_TYPE type;
    public CARD_COLOR color;
    public string valueStr;
    public float value;
    public int point;

    public int siblingIdx = 0;

    //백그라운드 리스트
    public List<Sprite> backgroundList;
    public List<Sprite> IconList;
    public List<Color> colorList;

    public GameObject m_front;
    public GameObject m_back;
    public GameObject m_gray;

    private Vector3 m_originPosition;
    private Quaternion m_originRotation;

    public Player m_ownerPlayer;

    private float m_fLastClick;

    private bool m_bIsSelect = false;

    private Dictionary<string, float> valueMap;

    private bool bIsBack = true;

    int m_siblingIdx = 0;

    private void Awake()
    {
        valueMap = new Dictionary<string, float>();

        valueMap.Add("2", 2);
        valueMap.Add("3", 3);
        valueMap.Add("4", 4);
        valueMap.Add("5", 5);
        valueMap.Add("6", 6);
        valueMap.Add("7", 7);
        valueMap.Add("8", 8);
        valueMap.Add("9", 9);
        valueMap.Add("10", 10);
        valueMap.Add("J", 11);
        valueMap.Add("Q", 12);
        valueMap.Add("K", 13);
        valueMap.Add("A", 14);
        valueMap.Add("새", 1);
        valueMap.Add("봉", 15);
        valueMap.Add("용", 25);
        valueMap.Add("개", 0);
    }

    public string GetColor()
    {
        switch (color)
        {
            case CARD_COLOR.RED:
                {
                    return "빨강 ";
                }
            case CARD_COLOR.BLACK:
                {
                    return "검정 ";
                }
            case CARD_COLOR.BLUE:
                {
                    return "파랑 ";
                }
            case CARD_COLOR.GREEN:
                {
                    return "녹색 ";
                }
        }

        return "";
    }

    private void Start()
    {
    }

    public void Init()
    {
        m_bIsSelect = false;
        Hide(false);
        Flip(true);
        m_gray.SetActive(false);
    }

    public void SetSortingRayer(int x)
    {
        transform.SetSiblingIndex(x);
    }

    public CardData(CARD_TYPE type, CARD_COLOR color, string value)
    {
        SetCard(type, color, value);
    }

    public void SetOriginPosition()
    {
        m_originPosition = transform.position;
        m_originRotation = transform.rotation;
    }

    public void MoveOriginRotate()
    {
        transform.rotation = m_originRotation;
    }

    public void MoveOriginPosition()
    {
        transform.position = m_originPosition;
        transform.rotation = m_originRotation;
    }

    public void SetCard(CARD_TYPE type, CARD_COLOR color, string value)
    {
        this.type = type;
        this.color = color;
        this.valueStr = value;
        this.value = valueMap[this.valueStr];

        switch (this.valueStr)
        {
            case "5":
                {
                    this.point = 5;
                }
                break;
            case "10":
            case "K":
                {
                    this.point = 10;
                }
                break;
            case "봉":
                {
                    this.point = -25;
                }
                break;
            case "용":
                {
                    this.point = 25;
                }
                break;
            default:
                {
                    this.point = 0;
                }
                break;
        }

        int colorValue = (int)color;
        centerText.text = valueStr;
        background.sprite = backgroundList[(int)type];

        leftIcon.sprite = rightIcon.sprite = IconList[colorValue];
        centerText.color =leftText.color = rightText.color = colorList[colorValue];

        leftText.text = rightText.text = valueStr.ToString();

        siblingIdx = transform.GetSiblingIndex();

        if (this.color == CARD_COLOR.NONE)
        {
            leftIcon.enabled = false;
            rightIcon.enabled = false;
        }
    }

    public void Flip(bool isBack)
    {
        bIsBack = isBack;

        m_front.SetActive(!bIsBack);
        m_back.SetActive(bIsBack);
    }

    public void Hide(bool isHide = true)
    {
        this.gameObject.SetActive(!isHide);
    }

    public void SetSelect(bool isSelect)
    {
        m_bIsSelect = isSelect;
    }

    public void RelaseSelect()
    {
        m_bIsSelect = false;
        this.transform.Translate(transform.up * -30.0f);
    }

    public bool IsSelected()
    {
        return m_bIsSelect;
    }

    public void OnMouseDown()
    {
        //Debug.Log("Mouse Down");
        if (m_ownerPlayer == null || m_ownerPlayer.name != "Player")
        {
            return;
        }

        float downTime = Time.time;
        
        if (GameManager.Instance.rutine == RutineState.RoundState)
        {
            if (!m_bIsSelect)
            {
                m_bIsSelect = !m_bIsSelect;
                m_siblingIdx = transform.GetSiblingIndex();

                transform.Translate(transform.up * 30.0f);
                m_ownerPlayer.AddSelectedCard(this);
            }
            else
            {
                m_bIsSelect = !m_bIsSelect;
                transform.SetSiblingIndex(m_siblingIdx);

                transform.Translate(transform.up * -30.0f);
                m_ownerPlayer.ReleaseCard(this);
            }
        }
        else if (GameManager.Instance.rutine == RutineState.ExchangeState)
        {
            //더블 클릭 일때만 돌아가도록 해보자
            if (downTime - m_fLastClick > 0.3)
            {
                //더클클릭이 아니니 바로 리턴
                m_fLastClick = downTime;
                return;
            }

            //3개 이상 선택이 되어있으면 더 이상 선택이 안되도록 해야됨
            if (!m_ownerPlayer.IsExchagne())
            {
                if (!m_bIsSelect)
                {
                    m_ownerPlayer.AddSelectedCard(this);
                }
                else
                {
                    m_ownerPlayer.ReleaseCard(this);
                }

                m_bIsSelect = !m_bIsSelect;
            }
            else                                    // 3개 이상이다?
            {
                //해제는 가능해야됨
                if (m_bIsSelect)
                {
                    m_ownerPlayer.ReleaseCard(this);
                    m_bIsSelect = !m_bIsSelect;
                }
            }
        }

        m_fLastClick = downTime;
    }

    public void SetOwnerPlayer(Player player)
    {
        m_ownerPlayer = player;
    }

    public IEnumerator CardMove(Vector3 startPosition, Vector3 endPosition, float deltaTime = 1.0f)
    {
        Vector3 moveVec = endPosition - startPosition;
        float time = 0;

        while (time <= deltaTime)
        {
            this.gameObject.transform.position = startPosition + (moveVec * (time / deltaTime));
            time += Time.deltaTime;
            yield return null;
        }

        this.gameObject.transform.position = startPosition + (moveVec * 1);
        time += Time.deltaTime;
    }
}

