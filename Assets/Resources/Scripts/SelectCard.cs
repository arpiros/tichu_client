using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCard : MonoBehaviour {

    private List<CardData> m_selectedCard = new List<CardData>();
    private float m_topValue = 0;
    private DRAWCARD_TYPE m_eDrawCardType = DRAWCARD_TYPE.NONE;
    private bool m_bHavePhoenix = false;
    float m_phoenixValue = 0;

    public void Add(CardData card)
    {
        if (m_topValue < card.value)
        {
            m_topValue = card.value;
        }

        if (card.type == CARD_TYPE.PHOENIX)
        {
            m_bHavePhoenix = true;
            card.value = 15;
        }

        m_selectedCard.Add(card);
        SetDrawCardType();

        m_selectedCard.Sort(delegate (CardData a, CardData b)
        {
            if (a.value == b.value)
            {
                return a.color.CompareTo(b.color);
            }
            return a.value.CompareTo(b.value);
        }
        );

        for (int i = 0; i < m_selectedCard.Count; ++i)
        {
            m_selectedCard[i].transform.SetSiblingIndex(i + 60);
        }
    }

    public void ReleaseCard(CardData card)
    {
        m_selectedCard.Remove(card);

        if (card.type == CARD_TYPE.PHOENIX)
        {
            m_bHavePhoenix = false;
        }

        if (m_topValue <= card.value)
        {
            m_topValue = 0;

            for (int i = 0; i < m_selectedCard.Count; ++i)
            {
                if (m_topValue < m_selectedCard[i].value)
                {
                    m_topValue = m_selectedCard[i].value;
                }
            }
        }
        SetDrawCardType();
    }

    public float GetTopValue()
    {
        return m_topValue;
    }

    public int GetSize()
    {
        return m_selectedCard.Count;
    }

    public DRAWCARD_TYPE GetDrawCardType()
    {
        return m_eDrawCardType;
    }

    public void Init()
    {
        if (m_bHavePhoenix)
        {
            CardData phoenixCard = m_selectedCard.Find(item => item.type == CARD_TYPE.PHOENIX);

            phoenixCard.value = 15;
        }

        for (int i = 0; i < m_selectedCard.Count; ++i)
        {
            m_selectedCard[i].RelaseSelect();
        }

        m_selectedCard.Clear();
        m_topValue = 0;
        m_eDrawCardType = DRAWCARD_TYPE.NONE;
        m_bHavePhoenix = false;
    }

    public List<CardData> GetCardList()
    {
        return m_selectedCard;
    }

    public bool FindWishCard(string wish)
    {
        return (m_selectedCard.Find(card => card.valueStr == wish) != null);
    }

    public IEnumerator CardDrawMove(List<Vector3> endPosList, float totalTime = 0.5f)
    {
        GameManager.Instance.rutine = RutineState.CardMoveState;

        if (m_bHavePhoenix)
        {
            CardData phoenixCard = m_selectedCard.Find(item => item.type == CARD_TYPE.PHOENIX);
            phoenixCard.value = m_phoenixValue;
        }

        m_selectedCard.Sort(delegate (CardData a, CardData b)
        {
            if (a.value == b.value)
            {
                return a.color.CompareTo(b.color);
            }
            return a.value.CompareTo(b.value);
        }
        );

        for (int i = 0; i < m_selectedCard.Count; ++i)
        {
            m_selectedCard[i].transform.SetSiblingIndex(i + 60);
        }

        List<Vector3> startPos = new List<Vector3>();
        float time = 0;

        for (int i = 0; i < m_selectedCard.Count; ++i)
        {
            startPos.Add(m_selectedCard[i].transform.position);
        }

        while (time <= totalTime)
        {
            for (int i = 0; i < m_selectedCard.Count; ++i)
            {
                Vector3 moveVec = endPosList[i] - startPos[i];

                m_selectedCard[i].transform.position = startPos[i] + (moveVec * (time / totalTime));
            }
            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < m_selectedCard.Count; ++i)
        {
            Vector3 moveVec = endPosList[i] - startPos[i];

            m_selectedCard[i].transform.position = startPos[i] + (moveVec * 1);
        }

        GameManager.Instance.rutine = RutineState.RoundState;
    }

    public void SetDrawCardType()
    {
        m_eDrawCardType = DRAWCARD_TYPE.NONE;

        CardData phoenixCard = m_selectedCard.Find(item => item.type == CARD_TYPE.PHOENIX);

        if (m_selectedCard.Count == 1)
        {
            m_eDrawCardType = DRAWCARD_TYPE.SINGLE;
            
            if (m_bHavePhoenix)
            {
                float topValue = CardDrawHandler.Instance.GetTopValue();

                if (topValue != 25)
                {
                    m_topValue = topValue + 0.5f;
                }
                else
                {
                    m_eDrawCardType = DRAWCARD_TYPE.NONE;
                }
            }
        }
        else if (m_selectedCard.Count == 2)
        {
            if (m_selectedCard[0].value == m_selectedCard[1].value)
            {
                m_eDrawCardType = DRAWCARD_TYPE.PAIR;
            }

            if (m_selectedCard[0].type == CARD_TYPE.PHOENIX)
            {
                m_eDrawCardType = DRAWCARD_TYPE.PAIR;
                m_selectedCard[0].value = m_selectedCard[1].value;
            }
            else if (m_selectedCard[1].type == CARD_TYPE.PHOENIX)
            {
                m_eDrawCardType = DRAWCARD_TYPE.PAIR;
                m_selectedCard[1].value = m_selectedCard[0].value;
            }

        }
        else if (m_selectedCard.Count == 3)
        {
            float num = m_selectedCard[0].value;
            float num2 = m_selectedCard[1].value;
            float num3 = m_selectedCard[2].value;

            if (num == num2 && num2 == num3)
            {
                m_eDrawCardType = DRAWCARD_TYPE.TRIPPLE;
            }

            if (m_bHavePhoenix)
            {
                //페어 검사를 하자.
                for (int i = 0; i < m_selectedCard.Count - 1; ++i)
                {
                    for (int j = i + 1; j < m_selectedCard.Count; ++j)
                    {
                        if (m_selectedCard[i] == m_selectedCard[j])
                        {
                            m_phoenixValue = m_selectedCard[i].value;

                            m_eDrawCardType = DRAWCARD_TYPE.TRIPPLE;
                        }
                    }
                }
            }
        }
        else if (m_selectedCard.Count == 4)
        {
            //4장인 경우 연속 페어와 폭탄이 있다
            //폭탄부터 검사를 하자
            float num = m_selectedCard[0].value;
            bool isBomb = true;

            for (int i = 1; i < m_selectedCard.Count && isBomb; ++i)
            {
                if (num != m_selectedCard[i].value)
                {
                    isBomb = false;
                }
            }

            if (isBomb)
            {
                m_eDrawCardType = DRAWCARD_TYPE.BOMB;
            }
            else
            {
                bool isPairs = true;
                int count = m_selectedCard.Count / 2;
                int idx = 0;

                {
                    //일단 정렬
                    m_selectedCard.Sort(delegate (CardData a, CardData b) { return a.value.CompareTo(b.value); });

                    if (m_bHavePhoenix)
                    {
                        List<PairList> pairLists = new List<PairList>();

                        for (int i = 0; i < m_selectedCard.Count - 1; ++i)
                        {
                            if (m_selectedCard[i].value == m_selectedCard[i + 1].value)
                            {
                                PairList pair = new PairList();

                                pair.cardIdx1 = i;
                                pair.cardIdx2 = i + 1;
                                pair.pairValue = m_selectedCard[i].value;
                                pairLists.Add(pair);
                                ++i;
                            }
                            else
                            {
                                if (m_selectedCard[i] == phoenixCard)
                                {
                                    m_phoenixValue = m_selectedCard[i + 1].value;
                                }
                                else if (m_selectedCard[i + 1] == phoenixCard)
                                {
                                    m_phoenixValue = m_selectedCard[i].value;
                                }
                            }
                        }

                        if (pairLists.Count >= count - 1)
                        {
                            m_eDrawCardType = DRAWCARD_TYPE.PAIRS;
                            return;
                        }
                    }
                    else
                    {
                        if (m_selectedCard.Count % 2 != 0)
                        {
                            isPairs = false;
                        }

                        float num1 = m_selectedCard[0].value;

                        do
                        {
                            if (m_selectedCard[(idx * 2)].value == m_selectedCard[(idx * 2) + 1].value)
                            {
                                num1 = m_selectedCard[idx * 2].value;
                            }
                            else
                            {
                                isPairs = false;
                            }

                            if (isPairs)
                            {
                                ++idx;

                                if (idx < count && num1 + 1 != m_selectedCard[idx * 2].value)
                                {
                                    isPairs = false;
                                }
                            }
                        }
                        while (isPairs && idx < count);
                    }
                }

                if (isPairs && idx == count)
                {
                    m_eDrawCardType = DRAWCARD_TYPE.PAIRS;
                }
            }

        }
        else if (m_selectedCard.Count == 5)
        {
            //5장인 경우는 스트레이트 폭탄
            //풀하우스
            //스트레이트

            //풀 하우스부터 체크를 해보자
            //일단 정렬
            m_selectedCard.Sort(delegate (CardData a, CardData b) { return a.value.CompareTo(b.value); });
            bool isFullHouse = true;

            {
                if (m_bHavePhoenix)
                {
                    int count = 0;

                    for (int i = 0; i < m_selectedCard.Count - 1; ++i)
                    {
                        for (int j = i + 1; j < m_selectedCard.Count; ++j)
                        {
                            if (m_selectedCard[i].value != m_selectedCard[j].value)
                            {
                                if (m_selectedCard[j].type != CARD_TYPE.PHOENIX)
                                {
                                    i = j;
                                    ++count;

                                    if (count >= 2)
                                    {
                                        //서로 다른숫자가 2개가 넘는다? 에러!
                                        isFullHouse = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //풀하우스인 경우는 2 / 3 혹은 3 / 2인 경우 말곤 없다.
                    //숫자순으로 정렬하면 첫장과 마지막장은 무조건 다르다.
                    float num1 = m_selectedCard[0].value;
                    float num2 = m_selectedCard[4].value;
                    int count1, count2;
                    count1 = count2 = 0;

                    for (int i = 0; i < m_selectedCard.Count && isFullHouse; ++i)
                    {
                        if (num1 == m_selectedCard[i].value)
                        {
                            ++count1;
                        }
                        if (num2 == m_selectedCard[i].value)
                        {
                            ++count2;
                        }

                        if (count1 + count2 != i + 1)
                        {
                            isFullHouse = false;
                        }
                    }
                }
            }

            if (isFullHouse)
            {
                m_eDrawCardType = DRAWCARD_TYPE.FULLHOUSE;
            }
            else
            {
                if (m_bHavePhoenix)
                {
                    float num1 = m_selectedCard[0].value;
                    bool isStright = true;
                    bool usePhoenix = m_bHavePhoenix;
                    int searchCard = 1;
                    int cardCount = 1;

                    while (isStright && cardCount < m_selectedCard.Count)
                    {
                        if (m_selectedCard[searchCard].type == CARD_TYPE.NONE && num1 + 1 == m_selectedCard[searchCard].value)
                        {
                            num1 = m_selectedCard[searchCard].value;
                            ++searchCard;
                        }
                        else if (usePhoenix)
                        {
                            usePhoenix = false;
                            num1 = num1 + 1;
                            m_phoenixValue = num1;
                        }
                        else
                        {
                            isStright = false;
                        }

                        ++cardCount;
                    }

                    if (isStright)
                    {
                        m_eDrawCardType = DRAWCARD_TYPE.STRIGHT;
                    }
                }
                else
                {
                    bool isBomb = true;
                    bool isStright = true;

                    float num1 = m_selectedCard[0].value;
                    CARD_COLOR eColor = m_selectedCard[0].color;

                    for (int i = 1; i < m_selectedCard.Count && isStright; ++i)
                    {
                        if (num1 + 1 == m_selectedCard[i].value)
                        {
                            num1 = m_selectedCard[i].value;

                            if (isBomb && eColor != m_selectedCard[i].color)
                            {
                                isBomb = false;
                            }
                        }
                        else
                        {
                            isStright = false;
                        }
                    }

                    if (isStright)
                    {
                        if (isBomb)
                        {
                            m_eDrawCardType = DRAWCARD_TYPE.STRIGHT_BOMB;
                        }
                        else
                        {
                            m_eDrawCardType = DRAWCARD_TYPE.STRIGHT;
                        }
                    }
                }
            }
        }
        else if (m_selectedCard.Count >= 6)
        {
            //6장 이상인 경우
            //스트레이트, 스트레이트 폭탄, 연속페어가 있다.
            //연속페어부터 검사를 하자.
            bool isPairs = true;
            int count = m_selectedCard.Count / 2;
            int idx = 0;

            {
                //일단 정렬
                m_selectedCard.Sort(delegate (CardData a, CardData b) { return a.value.CompareTo(b.value); });

                if (m_bHavePhoenix)
                {
                    List<PairList> pairLists = new List<PairList>();

                    for (int i = 0; i < m_selectedCard.Count - 1; ++i)
                    {
                        if (m_selectedCard[i].value == m_selectedCard[i + 1].value)
                        {
                            PairList pair = new PairList();

                            pair.cardIdx1 = i;
                            pair.cardIdx2 = i + 1;
                            pair.pairValue = m_selectedCard[i].value;
                            pairLists.Add(pair);
                            ++i;
                        }
                        else
                        {
                            if (m_selectedCard[i] == phoenixCard)
                            {
                                m_phoenixValue = m_selectedCard[i + 1].value;
                            }
                            else if (m_selectedCard[i + 1] == phoenixCard)
                            {
                                m_phoenixValue = m_selectedCard[i].value;
                            }
                        }
                    }

                    if (pairLists.Count >= count - 1)
                    {
                        m_eDrawCardType = DRAWCARD_TYPE.PAIRS;
                        return;
                    }
                }
                else
                {
                    if (m_selectedCard.Count % 2 != 0)
                    {
                        isPairs = false;
                    }

                    float num = m_selectedCard[0].value;

                    do
                    {
                        if (m_selectedCard[(idx * 2)].value == m_selectedCard[(idx * 2) + 1].value)
                        {
                            num = m_selectedCard[idx * 2].value;
                        }
                        else
                        {
                            isPairs = false;
                        }

                        if (isPairs)
                        {
                            ++idx;

                            if (idx < count && num + 1 != m_selectedCard[idx * 2].value)
                            {
                                isPairs = false;
                            }
                        }
                    }
                    while (isPairs && idx < count);
                }
            }

            if (isPairs && idx == count)
            {
                m_eDrawCardType = DRAWCARD_TYPE.PAIRS;
            }
            else
            {
                if (m_bHavePhoenix)
                {
                    bool isStright = true;
                    float num1 = m_selectedCard[0].value;
                    bool usePhoenix = m_bHavePhoenix;
                    int searchCard = 1;
                    int cardCount = 1;

                    while (isStright && cardCount < m_selectedCard.Count)
                    {
                        if (m_selectedCard[searchCard].type == CARD_TYPE.NONE && num1 + 1 == m_selectedCard[searchCard].value)
                        {
                            num1 = m_selectedCard[searchCard].value;
                            ++searchCard;
                        }
                        else if (usePhoenix)
                        {
                            usePhoenix = false;
                            num1 = num1 + 1;
                            m_phoenixValue = num1;
                        }
                        else
                        {
                            isStright = false;
                        }

                        ++cardCount;
                    }

                    if (isStright)
                    {
                        m_eDrawCardType = DRAWCARD_TYPE.STRIGHT;
                    }
                }
                else
                {
                    bool isBomb = true;
                    bool isStright = true;

                    float num1 = m_selectedCard[0].value;
                    CARD_COLOR eColor = m_selectedCard[0].color;

                    for (int i = 1; i < m_selectedCard.Count && isStright; ++i)
                    {
                        if (num1 + 1 == m_selectedCard[i].value)
                        {
                            num1 = m_selectedCard[i].value;

                            if (isBomb && eColor != m_selectedCard[i].color)
                            {
                                isBomb = false;
                            }
                        }
                        else
                        {
                            isStright = false;
                        }
                    }

                    if (isStright)
                    {
                        if (isBomb)
                        {
                            m_eDrawCardType = DRAWCARD_TYPE.STRIGHT_BOMB;
                        }
                        else
                        {
                            m_eDrawCardType = DRAWCARD_TYPE.STRIGHT;
                        }
                    }

                }
            }
        }
    }

}
