using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DRAWCARD_TYPE
{
    NONE        = 0,
    SINGLE      = 1,
    PAIR        = 2,
    TRIPPLE     = 3,
    PAIRS       = 4,
    FULLHOUSE   = 5,
    STRIGHT     = 6,
    BOMB        = 7,
    STRIGHT_BOMB,
}

public class CardDrawHandler : MonoBehaviour {

    private static CardDrawHandler _instance = null;

    public static CardDrawHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(CardDrawHandler)) as CardDrawHandler;

                if (_instance == null)
                {
                    Debug.LogError("카드 드로우 생성 불가");
                }
            }

            return _instance;
        }
    }

    private DRAWCARD_TYPE m_drawCardType = DRAWCARD_TYPE.NONE;
    private float m_topValue = 0;
    private int m_cardCount = 0;
    private int m_depth = 0;
    private bool m_bHaveDragon = false;
    private bool m_bDrawDog = false;

    List<CardData> m_drawCardList = new List<CardData>();

    //몇장씩 내야 되는지 결정하는 값
    public int GetCardCount()
    {
        return m_cardCount;
    }

    public float GetTopValue()
    {
        return m_topValue;
    }

    public DRAWCARD_TYPE GetDrawCardType()
    {
        return m_drawCardType;
    }

    public void CardDrawAtList(List<CardData> cardList, DRAWCARD_TYPE eDrawCardType, float fTopValue)
    {
        //기존 카드 비활성화 해볼까?
        foreach (CardData card in m_drawCardList)
        {
            card.m_gray.SetActive(true);
        }

        foreach (CardData card in cardList)
        {
            if (!m_bHaveDragon && card.type == CARD_TYPE.DRACHE)
            {
                //용이 있다.
                m_bHaveDragon = true;
            }
            else if (card.type == CARD_TYPE.DASHUND)
            {
                //개를 냈다.
                m_bDrawDog = true;
            }

            //int siblingIndex = card.transform.GetSiblingIndex();

            card.transform.SetSiblingIndex(70 + m_depth++);

            card.SetOwnerPlayer(null);
        }

        m_drawCardList.AddRange(cardList);
        m_cardCount = cardList.Count;
        m_drawCardType = eDrawCardType;
        m_topValue = fTopValue;

        //++m_depth;
    }

    public int GetDepth()
    {
        return m_depth;
    }

    public void initCardDraw()
    {
        m_drawCardType = DRAWCARD_TYPE.NONE;
        m_topValue = 0;
        m_cardCount = 0;
        m_depth = 0;
        m_bDrawDog = false;
        m_bHaveDragon = false;

        m_drawCardList.Clear();
    }

    public int TotalPoint()
    {
        int totalPoint = 0;

        for (int i = 0; i < m_drawCardList.Count; ++i)
        {
            totalPoint += m_drawCardList[i].point;
        }

        return totalPoint;
    }

    public List<CardData> GetDrawCardList()
    {
        return m_drawCardList;
    }

    public bool HaveDragon()
    {
        return (m_drawCardType == DRAWCARD_TYPE.SINGLE && m_bHaveDragon);
    }

    public bool DrawDog()
    {
        return m_bDrawDog;
    }

    public IEnumerator EatCardMove(Vector3 endPos, float totalTime = 0.5f)
    {
        float time = 0;
        List<Vector3> startPosList = new List<Vector3>();

        for (int i = 0; i < m_drawCardList.Count; ++i)
        {
            startPosList.Add(m_drawCardList[i].transform.position);
        }

        while (time <= totalTime)
        {
            for (int i = 0; i < m_drawCardList.Count; ++i)
            {
                Vector3 moveVec = endPos - startPosList[i];

                m_drawCardList[i].transform.position = startPosList[i] + (moveVec * (time / totalTime));
            }

            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < m_drawCardList.Count; ++i)
        {
            Vector3 moveVec = endPos - startPosList[i];

            m_drawCardList[i].transform.position = startPosList[i] + (moveVec * 1);
        }

    }

    //낼 수 있는 지 없는지 구분하자
    //같은 타입과 탑 숫자를 비교한다
    public void DarwCheck()
    {

    }
}
