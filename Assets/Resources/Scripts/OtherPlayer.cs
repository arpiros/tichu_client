using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : Player {

    List<List<CardData>> m_drawCardTypeList = new List<List<CardData>>();

    //public void Init()
    //{
    //    m_cardCountText.text = m_myCard.ToString();
    //}

    public override void SetCardCount()
    {
        m_cardCountText.text = m_myCard.Count.ToString() + "장";
    }

    // Use this for initialization
    protected void Start() {
        m_cardCountText.text = m_myCard.Count.ToString() + "장";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator FindDrawCardType()
    {
        //시작할 때 낼 수 있는 타입을 검사해보자.
        //미리 들고 있을까... 할때 바로 검사할까..
        //음.. 바로 검사하는게 재미있을지도.
        //타입이 None인 경우 이 함수로 찾아서 내는걸로 해보자.
        List<List<CardData>> drawCardTypeList = new List<List<CardData>>();

        //CardData phoenix = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);
        int phoenixIdx = m_myCard.FindIndex(item => item.type == CARD_TYPE.PHOENIX);
        bool havePhoenix = (phoenixIdx >= 0);
        bool usePhoenix = havePhoenix;

        List<int> idxList = new List<int>();
        int count = 0;

        //장수가 많은것부터 찾자
        //스트레이트 -> 연속페어 -> 풀하우스 -> 트리플 -> 페어 -> 싱글

        //스트레이트 찾기
        for (int i = 0; i < m_myCard.Count - 5; ++i)
        {
            idxList.Clear();
            idxList.Add(i);
            count = 1;

            for (int j = 1; j < m_myCard.Count; ++j)
            {
                if (m_myCard[i].value + count - 1 == m_myCard[j].value)
                {
                    continue;
                }

                if (m_myCard[i].value + count == m_myCard[j].value)
                {
                    idxList.Add(j);
                    ++count;
                }
                else if (usePhoenix)
                {
                    idxList.Add(phoenixIdx);
                    ++count;
                }
                else
                {
                    //장수가 5장 이상이면 스트레이트로 등록을 해주자.
                    if (count > 5)
                    {
                        List<CardData> drawCardType = new List<CardData>();
                        for (int k = 0; k < idxList.Count; ++k)
                        {
                            drawCardType.Add(m_myCard[idxList[k]]);
                        }
                        drawCardTypeList.Add(drawCardType);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        //연속페어 찾기 그리고 페어 찾기
        List<int> cardIdxList = new List<int>();
        List<int> pariValueList = new List<int>();
        List<PairList> pairLists = new List<PairList>();

        for (int i = 0; i < m_myCard.Count - 1; ++i)
        {
            if (m_myCard[i].value == m_myCard[i + 1].value)
            {
                List<CardData> drawCardType = new List<CardData>();
                PairList pair = new PairList();

                pair.cardIdx1 = i;
                pair.cardIdx2 = i + 1;
                pair.pairValue = m_myCard[i].value;

                pairLists.Add(pair);

                drawCardType.Add(m_myCard[i]);
                drawCardType.Add(m_myCard[i + 1]);

                drawCardTypeList.Add(drawCardType);
                ++i;
            }
        }

        int pairCount = 0;

        for (int i = 0; i < pairLists.Count - 1; ++i)
        {
            pairCount = 1;
            cardIdxList.Clear();
            cardIdxList.Add(i);

            for (int j = i + 1; j < pairLists.Count; ++j)
            {
                if (pairLists[i].pairValue + pairCount == pairLists[j].pairValue)
                {
                    ++pairCount;
                    cardIdxList.Add(j);
                }
                else
                {
                    if (pairCount >= 2)
                    {
                        List<CardData> drawCardType = new List<CardData>();
                        //연속페어다.
                        for (int k = 0; k < cardIdxList.Count; ++k)
                        {
                            drawCardType.Add(m_myCard[pairLists[cardIdxList[k]].cardIdx1]);
                            drawCardType.Add(m_myCard[pairLists[cardIdxList[k]].cardIdx2]);
                        }
                        drawCardTypeList.Add(drawCardType);
                    }
                }
            }
        }

        //트리플 찾기
        for (int i = 0; i < m_myCard.Count - 2; ++i)
        {
            if (m_myCard[i].value == m_myCard[i + 1].value &&
                m_myCard[i + 1].value == m_myCard[i + 2].value)
            {
                List<CardData> drawCardType = new List<CardData>();

                drawCardType.Add(m_myCard[i]);
                drawCardType.Add(m_myCard[i + 1]);
                drawCardType.Add(m_myCard[i + 2]);

                drawCardTypeList.Add(drawCardType);
            }
            else
            {
                if (m_myCard[i].value == m_myCard[i + 1].value && usePhoenix)
                {
                    List<CardData> drawCardType = new List<CardData>();

                    drawCardType.Add(m_myCard[i]);
                    drawCardType.Add(m_myCard[i + 1]);
                    drawCardType.Add(m_myCard[phoenixIdx]);

                    drawCardTypeList.Add(drawCardType);
                }
            }
        }

        //풀하우스 찾기
        idxList.Clear();
        for (int i = 0; i < m_myCard.Count - 2; ++i)
        {
            if (m_myCard[i].value == m_myCard[i + 1].value &&
                m_myCard[i + 1].value == m_myCard[i + 2].value)
            {
                idxList.Add(i);
                idxList.Add(i + 1);
                idxList.Add(i + 2);
            }
            else
            {
                if (m_myCard[i].value == m_myCard[i + 1].value && usePhoenix)
                {
                    idxList.Add(i);
                    idxList.Add(i + 1);
                    idxList.Add(phoenixIdx);
                }
            }

            if (idxList.Count > 0)
            {
                for (int j = 0; j < m_myCard.Count - 1; ++j)
                {
                    if (m_myCard[j].value == m_myCard[j + 1].value)
                    {
                        if (idxList.FindIndex(iteam => iteam == j) < 0)
                        {
                            List<CardData> drawCardType = new List<CardData>();

                            drawCardType.Add(m_myCard[idxList[0]]);
                            drawCardType.Add(m_myCard[idxList[1]]);
                            drawCardType.Add(m_myCard[idxList[2]]);
                            drawCardType.Add(m_myCard[j]);
                            drawCardType.Add(m_myCard[j + 1]);

                            drawCardTypeList.Add(drawCardType);
                        }
                    }
                }
            }

            idxList.Clear();
        }

        //싱글 찾기
        for (int i = 0; i < m_myCard.Count; ++i)
        {
            List<CardData> drawCardType = new List<CardData>();
            drawCardType.Add(m_myCard[i]);

            drawCardTypeList.Add(drawCardType);
        }

        int choose = Random.Range(0, drawCardTypeList.Count);
        //int choose = 0;

        for (int i = 0; i < drawCardTypeList[choose].Count; ++i)
        {
            AddSelectedCard(drawCardTypeList[choose][i]);
        }

        if (m_selectedCard.GetCardList().Find(item => item.type == CARD_TYPE.MAHJONG))
        {
            m_bDrawMajjong = true;
        }

        yield return StartCoroutine(AICardDraw());
    }

    public override IEnumerator CardExchange(float deltaTime = 0.5F)
    {
        //내가 받는 카드 숨김을 풀고 뒷면이 보이도록 하자
        for (int i = 0; i < 3; ++i)
        {
            m_exchagnedCard[i].Hide(false);
            m_exchagnedCard[i].Flip(true);
        }

        float time = 0;
        List<Vector3> exchangeCardPosList = new List<Vector3>();
        Vector3[] endPosList = new Vector3[3];

        endPosList[0] = m_leftPlayer.transform.position;
        endPosList[1] = m_teamPlayer.transform.position;
        endPosList[2] = m_rightPlayer.transform.position;

        for (int i = 0; i < 3; ++i)
        {
            exchangeCardPosList.Add(m_exchagnedCard[i].transform.position);
        }

        while (time <= deltaTime)
        {
            for (int i = 0; i < 3; ++i)
            {
                Vector3 startPos = exchangeCardPosList[i];
                Vector3 endPos = endPosList[i];
                Vector3 moveVec = endPos - startPos;

                m_exchagnedCard[i].transform.position = startPos + (moveVec * (time / deltaTime));
            }
            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 3; ++i)
        {
            Vector3 startPos = exchangeCardPosList[i];
            Vector3 endPos = endPosList[i];
            Vector3 moveVec = endPos - startPos;

            m_exchagnedCard[i].transform.position = startPos + (moveVec * 1);
        }

        CardData leftCard = m_leftPlayer.GetCardExchangeData(2);
        CardData teamCard = m_teamPlayer.GetCardExchangeData(1);
        CardData rightCard = m_rightPlayer.GetCardExchangeData(0);

        Debug.Log(this.name + "\n" +
            "left : " + leftCard.GetColor() + leftCard.valueStr +
            " team : " + teamCard.GetColor() + teamCard.valueStr +
            " right : " + rightCard.GetColor() + rightCard.valueStr);

        //다른 플레이어들에게 카드를 받는다.
        m_myCard.Add(leftCard);
        m_myCard.Add(teamCard);
        m_myCard.Add(rightCard);

        //교환한 카드가 누구카드인지 설정
        leftCard.SetOwnerPlayer(this);
        teamCard.SetOwnerPlayer(this);
        rightCard.SetOwnerPlayer(this);
    }

    public override IEnumerator DivisionCardMove(Vector3 startPosition, bool isFirst, float deltaTime = 0.5F)
    {
        float time = 0;

        int size = m_myCard.Count;
        List<Vector3> startPosList = new List<Vector3>();

        for (int i = 0; i < size; ++i)
        {
            startPosList.Add(m_myCard[i].transform.position);
        }

        //int offset = -(size / 2);

        while (time <= deltaTime)
        {
            //offset = -(size / 2);

            for (int i = 0; i < size; ++i)
            {
                Vector3 startPos = startPosition;
                Vector3 endPos = transform.position;
                Vector3 moveVec = endPos - startPos;

                m_myCard[i].transform.position = startPos + (moveVec * (time / deltaTime));
                ///++offset;
            }

            time += Time.deltaTime;
            yield return null;
        }

        //offset = -(size / 2);

        for (int i = 0; i < size; ++i)
        {
            Vector3 startPos = startPosition;
            Vector3 endPos = transform.position;
            Vector3 moveVec = endPos - startPos;

            m_myCard[i].transform.position = startPos + (moveVec * 1);
            //++offset;
        }

        for (int i = 0; i < size; ++i)
        {
            m_myCard[i].Flip(false);
            m_myCard[i].Hide(true);
        }
    }

    public override void ArrangementCard()
    {
        //value 값으로 정렬
        m_myCard.Sort(delegate (CardData a, CardData b)
        {
            if (a == b)
            {
                return 0;
            }

            if (a.value == b.value)
            {
                if (a.color > b.color)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return a.value.CompareTo(b.value);
        }
        );

        int size = m_myCard.Count;
        int offset = -(size / 2);
        for (int i = 0; i < size; ++i)
        {
            m_myCard[i].MoveOriginRotate();

            m_myCard[i].Hide();
            ++offset;
        }
    }

    public override bool DrawCheck()
    {
        int size = m_selectedCard.GetSize();

        //리스트가 비어있으면 체크할 필요가 없다.
        if (size <= 0)
        {
            return false;
        }

        DRAWCARD_TYPE eDrawCardType = CardDrawHandler.Instance.GetDrawCardType();

        // NONE 타입이거나 나랑 타입이 같으면 낼수 있다.
        if (eDrawCardType == DRAWCARD_TYPE.NONE)
        {
            return true;
        }
        else if (eDrawCardType == m_selectedCard.GetDrawCardType())
        {
            if (CardDrawHandler.Instance.GetCardCount() == m_selectedCard.GetSize() && CardDrawHandler.Instance.GetTopValue() < m_selectedCard.GetTopValue())
            {
                return true;
            }
        }

        return false;
    }

    public IEnumerator AICardDraw()
    {
        if (m_bIsDraw)
        {
            GameManager.Instance.SetTopPlayer(this);

            List<CardData> selectedCardList = m_selectedCard.GetCardList();

            CardDrawHandler.Instance.CardDrawAtList(selectedCardList, m_selectedCard.GetDrawCardType(), m_selectedCard.GetTopValue());
            //내 카드 목록에서 빼주자.
            int size = m_selectedCard.GetSize();
            int offset = -(size / 2);

            string temp = this.name + "드로우 카드 : ";

            List<Vector3> endPosList = new List<Vector3>();

            for (int i = 0; i < selectedCardList.Count; ++i)
            {
                selectedCardList[i].Hide(false);
                selectedCardList[i].Flip(false);

                Vector3 endPos;
                endPos = CardDrawHandler.Instance.transform.position + CardDrawHandler.Instance.transform.right * (offset * 70);
                endPos = new Vector3(endPos.x + offsetX, endPos.y + offsetY, 0);
                endPosList.Add(endPos);

                temp += selectedCardList[i].value + " ";

                ++offset;
                m_myCard.Remove(selectedCardList[i]);
            }

            yield return StartCoroutine(m_selectedCard.CardDrawMove(endPosList));

            //for (int i = 0; i < selectedCardList.Count; ++i)
            //{
            //    selectedCardList[i].Hide(false);
            //    //selectedCardList[i].transform.position = CardDrawHandler.Instance.transform.position + CardDrawHandler.Instance.transform.right * (offset * 70);
            //    //selectedCardList[i].transform.position = new Vector3(selectedCardList[i].transform.position.x + (GameManager.Instance.RoundTurn * offsetX),
            //    //                                                    selectedCardList[i].transform.position.y + (GameManager.Instance.RoundTurn * offsetY),
            //    //                                                    CardDrawHandler.Instance.GetDepth());

            //    selectedCardList[i].transform.position = CardDrawHandler.Instance.transform.position + CardDrawHandler.Instance.transform.right * (offset * 70);
            //    selectedCardList[i].transform.position = new Vector3(selectedCardList[i].transform.position.x + offsetX,
            //                                                        selectedCardList[i].transform.position.y + offsetY,
            //                                                        0);
            //    selectedCardList[i].transform.rotation = CardDrawHandler.Instance.transform.rotation;

            //    temp += selectedCardList[i].value + " ";

            //    ++offset;

            //    m_myCard.Remove(selectedCardList[i]);
            //}

            Debug.Log(temp);

            m_selectedCard.Init();
            ArrangementCard();
            m_cardCountText.text = m_myCard.Count.ToString() + "장";
        }
    }

    public override void MyTurn()
    {
        StartCoroutine(MyTurnCoroutine());
        //Pass();
    }

    public IEnumerator MyTurnCoroutine()
    {
        m_bDrawMajjong = false;
        m_bIsMyTurn = true;

        Debug.Log(this.name + "의 턴\n");

        StartCoroutine(AnimationClip());

        if (m_myCard.Count == 14 && !m_isCallLargeTichu)
        {
            int luckNum = Random.Range(0, 10);

            if (luckNum > 9)
            {
                m_isCallTichu = true;

                m_tichu.gameObject.SetActive(true);
                m_tichu.sprite = m_tichuIcon;
            }
        }

        yield return StartCoroutine(DrawAI());
        //Invoke("Pass", 2f);
        Pass();

        m_selectedCard.Init();
    }

    public override IEnumerator ChooseEatDragon()
    {
        int choose = Random.Range(0, 1);

        if (choose == 0)
        {
            //GameManager.Instance.GiveLeftPlayerBtn();
            yield return StartCoroutine(GameManager.Instance.GiveLeftPlayer());
        }
        else
        {
            //GameManager.Instance.GiveRightPlayerBtn();
            yield return StartCoroutine(GameManager.Instance.GiveRightPlayer());
        }
    }

    public override bool IsExchagne()
    {
        AIExchange();
        return m_bIsExchange;
    }

    public override bool IsChooseLargeTichu()
    {
        if (!m_isChooseLargeTichu)
        {
            int luckNum = Random.Range(0, 10);

            if (luckNum > 9)
            {
                m_isCallLargeTichu = true;
                m_isCallTichu = false;

                m_tichu.gameObject.SetActive(true);
                m_tichu.sprite = m_grandTichuIcon;
            }
            else
            {
                m_isCallLargeTichu = false;
            }

            m_isChooseLargeTichu = true;
        }

        return m_isChooseLargeTichu;
    }

    public void AIExchange()
    {
        if (!m_bIsExchange)
        {
            m_exchagnedCard[0] = m_myCard[0];
            m_exchagnedCard[1] = m_myCard[1];
            m_exchagnedCard[2] = m_myCard[2];

            m_bIsExchange = true;
        }
    }

    IEnumerator DrawAI()
    {
        //항상 낼수 있다를 근거로...
        m_bIsDraw = true;
        //컴퓨터 인공지능
        //심플하게 짠다.
        //처음 시작이면 싱글로만 내도록
        //누가 낸적이 있으면 낼수 있는지 비교해서 제출

        bool isWish = GameManager.Instance.m_wishCardPopup.Selected();
        string wishCard = GameManager.Instance.m_wishCardPopup.GetWishCard();

        DRAWCARD_TYPE eDrawCardType = CardDrawHandler.Instance.GetDrawCardType();

        //낸적이 있는지 없는지 비교한다.
        if (eDrawCardType == DRAWCARD_TYPE.NONE)
        {
            if (isWish)
            {
                //위시 카드가 있다. 해당 카드를 내자.
                for (int i = 0; i < m_myCard.Count; ++i)
                {
                    if (m_myCard[i].valueStr == wishCard)
                    {
                        AddSelectedCard(m_myCard[0]);
                        yield return StartCoroutine(AICardDraw());

                        //위시 달성
                        GameManager.Instance.AchieveWish();
                        yield break;
                    }
                }
            }

            //위시 카드가 없다
            //아무거나 낸다.
            //낸적이 없다.
            //싱글로 제일 작은 수를 내자.
            //참새를 내는지 안내는지 판단하고 참새면 소원을 말하자
            //if (m_myCard[0].type == CARD_TYPE.MAHJONG)
            //{
            //    //string[] wishList = { "X", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            //    //int selectWish = Random.Range(0, 13);
            //    //GameManager.Instance.m_wishCardPopup.NPCPlayerCallWish(wishList[selectWish]);
            //    m_bDrawMajjong = true;
            //}

            ////싱글인데 그게 봉황이데 탑카드보다 0.5를 더해주자.
            //if (m_myCard[0].type == CARD_TYPE.PHOENIX)
            //{
            //    m_myCard[0].value = CardDrawHandler.Instance.GetTopValue() + 0.5f;
            //    m_bHavePhoenix = false;
            //}
            //AddSelectedCard(m_myCard[0]);
            //yield return StartCoroutine(AICardDraw());

            yield return StartCoroutine(FindDrawCardType());
        }
        else
        {
            //낸적이 있다.
            int cardCount = CardDrawHandler.Instance.GetCardCount();
            float topValue = CardDrawHandler.Instance.GetTopValue();

            //일단 카드 장수가 부족하면 무조건 패스다.
            if (m_myCard.Count < cardCount)
            {
                yield break;
                //m_gameManager.Pass();
            }
            else
            {
                //카드 타입에 따라서 낼수 있는지 확인해보자.
                switch (eDrawCardType)
                {
                    case DRAWCARD_TYPE.SINGLE:
                        {
                            if (isWish)
                            {
                                for (int i = 0; i < m_myCard.Count; ++i)
                                {
                                    if (m_myCard[i].valueStr == wishCard)
                                    {
                                        if (topValue < m_myCard[i].value)
                                        {
                                            AddSelectedCard(m_myCard[i]);
                                            yield return StartCoroutine(AICardDraw());

                                            GameManager.Instance.AchieveWish();
                                            yield break;
                                        }
                                    }
                                }
                            }

                            //내 카드중에 탑 벨류가 높으면 제출
                            for (int i = 0; i < m_myCard.Count; ++i)
                            {
                                if (topValue < m_myCard[i].value)
                                {
                                    if (topValue != 25 && m_myCard[i].type == CARD_TYPE.PHOENIX)
                                    {
                                        m_bHavePhoenix = false;
                                    }

                                    AddSelectedCard(m_myCard[i]);
                                    yield return StartCoroutine(AICardDraw());
                                    yield break;
                                }
                            }

                            //여기까지 왔으면 못 낸거
                            if (m_bHavePhoenix)
                            {
                                CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);

                                if (topValue != 25 && card != null)
                                {
                                    AddSelectedCard(card);
                                    yield return StartCoroutine(AICardDraw());
                                    m_bHavePhoenix = false;
                                    yield break;
                                }
                            }
                        }
                        break;
                    case DRAWCARD_TYPE.PAIR:
                        {
                            bool isDraw = false;

                            for (int i = 0; i < m_myCard.Count - 1; ++i)
                            {
                                if (m_myCard[i].value == m_myCard[i + 1].value)
                                {
                                    //페어다
                                    if (topValue < m_myCard[i].value)
                                    {
                                        AddSelectedCard(m_myCard[i]);
                                        AddSelectedCard(m_myCard[i + 1]);
                                        yield return StartCoroutine(AICardDraw());
                                        isDraw = true;
                                        break;
                                    }
                                }
                            }

                            //못 냈을 경우 봉황이 있으면 낼수 있는지 확인한다.
                            if (!isDraw && m_bHavePhoenix)
                            {
                                for (int i = 0; i < m_myCard.Count; ++i)
                                {
                                    if (topValue < m_myCard[i].value)
                                    {
                                        if (m_myCard[i].type == CARD_TYPE.NONE)
                                        {
                                            //특수카드가 아니고 봉황이랑 페어를 이루게 하자
                                            CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);
                                            if (m_myCard[i] != card)
                                            {
                                                AddSelectedCard(m_myCard[i]);
                                                AddSelectedCard(card);
                                                yield return StartCoroutine(AICardDraw());
                                                m_bHavePhoenix = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case DRAWCARD_TYPE.TRIPPLE:
                        {
                            bool isDraw = false;

                            for (int i = 0; i < m_myCard.Count - 2; ++i)
                            {
                                if (m_myCard[i].value == m_myCard[i + 1].value
                                    && m_myCard[i + 1].value == m_myCard[i + 2].value)
                                {
                                    if (topValue < m_myCard[i].value)
                                    {
                                        AddSelectedCard(m_myCard[i]);
                                        AddSelectedCard(m_myCard[i + 1]);
                                        AddSelectedCard(m_myCard[i + 2]);
                                        yield return StartCoroutine(AICardDraw());
                                        isDraw = true;
                                        break;
                                    }
                                }
                            }

                            //못 냈을 경우 봉황으로 낼수 있는지 확인
                            if (!isDraw && m_bHavePhoenix)
                            {
                                //일단 페어가 있는지 검사
                                for (int i = 0; i < m_myCard.Count - 1; ++i)
                                {
                                    if (m_myCard[i].value == m_myCard[i + 1].value)
                                    {
                                        //낼수 있는 페어가 있다.
                                        if (topValue < m_myCard[i].value)
                                        {
                                            CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);
                                            AddSelectedCard(m_myCard[i]);
                                            AddSelectedCard(m_myCard[i + 1]);
                                            AddSelectedCard(card);
                                            yield return StartCoroutine(AICardDraw());
                                            m_bHavePhoenix = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case DRAWCARD_TYPE.PAIRS:
                        {
                            //연속페어
                            //카드수 체크
                            List<int> cardIdxList = new List<int>();
                            int linkedNum = cardCount / 2;
                            List<int> pairValueList = new List<int>();
                            List<PairList> pairLists = new List<PairList>();

                            //페어들을 일단 다 찾는다.
                            for (int i = 0; i < m_myCard.Count - 1; ++i)
                            {
                                if (m_myCard[i].value == m_myCard[i + 1].value)
                                {
                                    PairList pair = new PairList();

                                    pair.cardIdx1 = i;
                                    pair.cardIdx2 = i + 1;
                                    pair.pairValue = m_myCard[i].value;

                                    pairLists.Add(pair);

                                    ++i;
                                }
                            }

                            //페어 갯수가 연속페어 갯수보다 작으면 패스
                            if (pairLists.Count < linkedNum)
                            {
                                break;
                            }

                            //위시 확인
                            if (isWish)
                            {
                                //위시카드가 있는 경우
                                bool haveWishCard = false;
                                float nWishValue = 0;
                                bool usePhoenix = m_bHavePhoenix;
                                CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);

                                //일단 위시 숫자가 있고 낼수 있는지 확인
                                for (int i = 0; i < m_myCard.Count; ++i)
                                {
                                    if (m_myCard[i].valueStr == wishCard)
                                    {
                                        nWishValue = m_myCard[i].value;
                                        if (topValue < nWishValue)
                                        {
                                            haveWishCard = true;
                                        }
                                        break;
                                    }
                                }

                                if (haveWishCard)
                                {
                                    bool bHaveWishCardPair = false;

                                    for (int i = 0; i < pairLists.Count; ++i)
                                    {
                                        if (pairLists[i].pairValue == nWishValue)
                                        {
                                            bHaveWishCardPair = true;
                                        }
                                    }

                                    //위시 카드 페어가 있다.
                                    if (bHaveWishCardPair)
                                    {
                                        //위시 카드가 없으면 내 카드중에 낼수 있는 경우를 찾아서 낸다.
                                        int pairCount = 0;

                                        for (int i = 0; i < pairLists.Count && pairLists.Count - i >= linkedNum; ++i)
                                        {
                                            pairCount = 1;
                                            cardIdxList.Clear();
                                            cardIdxList.Add(i);
                                            usePhoenix = m_bHavePhoenix;

                                            for (int j = i + 1; j < pairLists.Count; ++j)
                                            {
                                                if (pairLists[i].pairValue + pairCount == pairLists[j].pairValue)
                                                {
                                                    ++pairCount;
                                                    cardIdxList.Add(j);

                                                    if (pairCount == linkedNum)
                                                    {
                                                        if (topValue < pairLists[j].pairValue)
                                                        {
                                                            //연속페어를 찾았다.
                                                            for (int k = 0; k < cardIdxList.Count; ++k)
                                                            {
                                                                AddSelectedCard(m_myCard[pairLists[cardIdxList[i]].cardIdx1]);
                                                                AddSelectedCard(m_myCard[pairLists[cardIdxList[i]].cardIdx2]);

                                                                if (pairLists[cardIdxList[i]].pairValue == nWishValue)
                                                                {
                                                                    GameManager.Instance.AchieveWish();
                                                                }
                                                            }
                                                            yield return StartCoroutine(AICardDraw());
                                                            m_bHavePhoenix = usePhoenix;
                                                            yield break;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                }
                                                else if (usePhoenix)
                                                {
                                                    for (int idx = 0; idx < m_myCard.Count; ++idx)
                                                    {
                                                        if (m_myCard[idx].value == pairLists[i].pairValue + pairCount)
                                                        {
                                                            ++pairCount;
                                                            cardIdxList.Add(idx);
                                                            AddSelectedCard(card);
                                                            usePhoenix = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //위시카드 페어가 없다.
                                    else
                                    {
                                        //위시 카드가 없으면 내 카드중에 낼수 있는 경우를 찾아서 낸다.
                                        int pairCount = 0;

                                        for (int i = 0; i < pairLists.Count && pairLists.Count - i >= linkedNum; ++i)
                                        {
                                            pairCount = 1;
                                            cardIdxList.Clear();
                                            cardIdxList.Add(i);
                                            usePhoenix = m_bHavePhoenix;

                                            for (int j = i + 1; j < pairLists.Count; ++j)
                                            {
                                                if (pairLists[i].pairValue + pairCount == pairLists[j].pairValue)
                                                {
                                                    ++pairCount;
                                                    cardIdxList.Add(j);

                                                    if (pairCount == linkedNum)
                                                    {
                                                        if (topValue < pairLists[j].pairValue)
                                                        {
                                                            //연속페어를 찾았다.
                                                            for (int k = 0; k < cardIdxList.Count; ++k)
                                                            {
                                                                AddSelectedCard(m_myCard[pairLists[cardIdxList[i]].cardIdx1]);
                                                                AddSelectedCard(m_myCard[pairLists[cardIdxList[i]].cardIdx2]);
                                                            }
                                                            yield return StartCoroutine(AICardDraw());
                                                            m_bHavePhoenix = usePhoenix;
                                                            yield break;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                }
                                                else if (usePhoenix)
                                                {
                                                    for (int idx = 0; idx < m_myCard.Count; ++idx)
                                                    {
                                                        if (m_myCard[idx].value == pairLists[i].pairValue + pairCount)
                                                        {
                                                            ++pairCount;
                                                            cardIdxList.Add(idx);
                                                            AddSelectedCard(card);
                                                            usePhoenix = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //위시 카드가 없으면 내 카드중에 낼수 있는 경우를 찾아서 낸다.
                                    int pairCount = 0;

                                    for (int i = 0; i < pairLists.Count && pairLists.Count - i >= linkedNum; ++i)
                                    {
                                        pairCount = 1;
                                        cardIdxList.Clear();
                                        cardIdxList.Add(i);

                                        for (int j = i + 1; j < pairLists.Count; ++j)
                                        {
                                            if (pairLists[i].pairValue + pairCount == pairLists[j].pairValue)
                                            {
                                                ++pairCount;
                                                cardIdxList.Add(j);

                                                if (pairCount == linkedNum)
                                                {
                                                    if (topValue < pairLists[j].pairValue)
                                                    {
                                                        //연속페어를 찾았다.
                                                        for (int k = 0; k < cardIdxList.Count; ++k)
                                                        {
                                                            AddSelectedCard(m_myCard[pairLists[cardIdxList[i]].cardIdx1]);
                                                            AddSelectedCard(m_myCard[pairLists[cardIdxList[i]].cardIdx2]);
                                                        }
                                                        yield return StartCoroutine(AICardDraw());
                                                        m_bHavePhoenix = usePhoenix;
                                                        yield break;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                bool usePhoenix = m_bHavePhoenix;
                                CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);

                                //위시 카드가 없으면 내 카드중에 낼수 있는 경우를 찾아서 낸다.
                                int pairCount = 0;

                                for (int i = 0; i < pairLists.Count && pairLists.Count - i >= linkedNum; ++i)
                                {
                                    pairCount = 1;
                                    cardIdxList.Clear();
                                    cardIdxList.Add(i);

                                    for (int j = i + 1; j < pairLists.Count; ++j)
                                    {
                                        if (pairLists[i].pairValue + pairCount == pairLists[j].pairValue)
                                        {
                                            ++pairCount;
                                            cardIdxList.Add(j);

                                            if (pairCount == linkedNum)
                                            {
                                                if (topValue < pairLists[j].pairValue)
                                                {
                                                    //연속페어를 찾았다.
                                                    for (int k = 0; k < cardIdxList.Count; ++k)
                                                    {
                                                        AddSelectedCard(m_myCard[pairLists[cardIdxList[k]].cardIdx1]);
                                                        AddSelectedCard(m_myCard[pairLists[cardIdxList[k]].cardIdx2]);
                                                    }
                                                    yield return StartCoroutine(AICardDraw());
                                                    m_bHavePhoenix = usePhoenix;
                                                    yield break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case DRAWCARD_TYPE.BOMB:
                        {
                            List<int> cardIndexList = new List<int>();

                            for (int i = 0; i < m_myCard.Count - 4; ++i)
                            {
                                cardIndexList.Add(i);

                                for (int j = 1; j < m_myCard.Count; ++j)
                                {
                                    if (m_myCard[i].value == m_myCard[j].value)
                                    {
                                        cardIndexList.Add(j);
                                    }

                                    if (cardIndexList.Count == 4)
                                    {
                                        for (int k = 0; k < cardIndexList.Count; ++k)
                                        {
                                            AddSelectedCard(m_myCard[cardIndexList[k]]);
                                        }
                                        yield return StartCoroutine(AICardDraw());
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case DRAWCARD_TYPE.FULLHOUSE:
                        {
                            //포문을 2개 돌려서
                            //트리플과 페어를 따로 찾아보자.
                            bool isTripple = false;
                            List<int> cardIdx = new List<int>();
                            bool isDraw = false;

                            //트리플 부
                            for (int i = 0; i < m_myCard.Count - 2 && !isTripple; ++i)
                            {
                                if (m_myCard[i].value == m_myCard[i + 1].value
                                    && m_myCard[i + 1].value == m_myCard[i + 2].value)
                                {
                                    if (topValue < m_myCard[i].value)
                                    {
                                        //AddSelectedCard(m_myCard[i]);
                                        //AddSelectedCard(m_myCard[i + 1]);
                                        //AddSelectedCard(m_myCard[i + 2]);

                                        cardIdx.Add(i);
                                        cardIdx.Add(i + 1);
                                        cardIdx.Add(i + 2);

                                        isTripple = true;
                                    }
                                }
                            }

                            if (isTripple)
                            {
                                //페어를 찾는다.
                                for (int i = 0; i < m_myCard.Count - 1; ++i)
                                {
                                    if (m_myCard[i].value == m_myCard[i + 1].value)
                                    {
                                        //페어다
                                        if (cardIdx.Exists(x => x != i))
                                        {
                                            //트리플 추가
                                            AddSelectedCard(m_myCard[cardIdx[0]]);
                                            AddSelectedCard(m_myCard[cardIdx[1]]);
                                            AddSelectedCard(m_myCard[cardIdx[2]]);

                                            AddSelectedCard(m_myCard[i]);
                                            AddSelectedCard(m_myCard[i + 1]);
                                            yield return StartCoroutine(AICardDraw());
                                            isDraw = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!isDraw && m_bHavePhoenix)
                            {
                                //트리플이 있는지 검사하고
                                //트리플이 있는 경우가
                                //트리플이 없는 경우로 나눠서 체크를 해보자.
                                if (isTripple)
                                {
                                    CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);

                                    //트리플이 있는 경우 제일 작은 값을 봉황이랑 역어서 페어를 만들자.
                                    for (int i = 0; i < m_myCard.Count; ++i)
                                    {
                                        if (cardIdx.Exists(x => x != i) && m_myCard[i] != card)
                                        {
                                            //트리플 추가
                                            AddSelectedCard(m_myCard[cardIdx[0]]);
                                            AddSelectedCard(m_myCard[cardIdx[1]]);
                                            AddSelectedCard(m_myCard[cardIdx[2]]);
                                            AddSelectedCard(m_myCard[i]);
                                            AddSelectedCard(card);
                                            yield return StartCoroutine(AICardDraw());
                                            m_bHavePhoenix = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);

                                    //트리플이 없는 경우 페어를 2개를 찾는다.
                                    List<PairList> pairList = new List<PairList>();

                                    for (int i = 0; i < m_myCard.Count - 1; ++i)
                                    {
                                        if (m_myCard[i].value == m_myCard[i + 1].value)
                                        {
                                            PairList pair = new PairList();
                                            pair.pairValue = m_myCard[i].value;
                                            pair.cardIdx1 = i;
                                            pair.cardIdx2 = i + 1;

                                            pairList.Add(pair);
                                        }
                                    }

                                    if (pairList.Count >= 2)
                                    {
                                        PairList bigPair = pairList.Find(pair => pair.pairValue > topValue);

                                        if (bigPair != null)
                                        {
                                            AddSelectedCard(m_myCard[bigPair.cardIdx1]);
                                            AddSelectedCard(m_myCard[bigPair.cardIdx2]);
                                            AddSelectedCard(card);

                                            for (int i = 0; i < pairList.Count; ++i)
                                            {
                                                if (pairList[i].pairValue != bigPair.pairValue)
                                                {
                                                    AddSelectedCard(m_myCard[pairList[i].cardIdx1]);
                                                    AddSelectedCard(m_myCard[pairList[i].cardIdx2]);
                                                    yield return StartCoroutine(AICardDraw());
                                                    m_bHavePhoenix = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case DRAWCARD_TYPE.STRIGHT:
                        {
                            if (isWish)
                            {
                                //위시카드가 있는 경우
                                bool haveWishCard = false;
                                float nWishValue = 0;
                                int count = 0;
                                bool isStright = false;
                                List<int> idxList = new List<int>();
                                bool usePhoenix = m_bHavePhoenix;
                                CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);

                                //일단 위시숫자가 있는지 검사를 하자.
                                for (int i = 0; i < m_myCard.Count; ++i)
                                {
                                    if (m_myCard[i].valueStr == wishCard)
                                    {
                                        haveWishCard = true;
                                        nWishValue = m_myCard[i].value;
                                        break;
                                    }
                                }

                                //일단 스트레이트를 찾는다.
                                for (int i = 0; i < m_myCard.Count - cardCount && !isStright; ++i)
                                {
                                    idxList.Clear();
                                    count = 1;
                                    idxList.Add(i);
                                    m_selectedCard.Init();

                                    for (int j = i + 1; j < m_myCard.Count; ++j)
                                    {
                                        //같은 수면 넘어간다.
                                        if (m_myCard[i].value + count - 1 == m_myCard[j].value)
                                        {
                                            continue;
                                        }

                                        //연결된 수면 확인해본다.
                                        if (m_myCard[i].value + count == m_myCard[j].value)
                                        {
                                            idxList.Add(j);
                                            ++count;

                                            if (count == cardCount)
                                            {
                                                bool wishOk = false;
                                                //스트레이트를 찾았는데 위시가 있는지 보자.
                                                for (int k = 0; k < idxList.Count; ++k)
                                                {
                                                    //위시가 있는 스트레이트다
                                                    if (m_myCard[idxList[k]].valueStr == wishCard)
                                                    {
                                                        wishOk = true;
                                                        break;
                                                    }
                                                }

                                                //위시가 없는 스트레이트다 그냥 넘어가자
                                                if (!wishOk)
                                                {
                                                    continue;
                                                }

                                                //내자
                                                if (m_myCard[j].value > topValue)
                                                {
                                                    for (int k = 0; k < idxList.Count; ++k)
                                                    {
                                                        AddSelectedCard(m_myCard[idxList[k]]);
                                                    }
                                                    yield return StartCoroutine(AICardDraw());
                                                    m_bHavePhoenix = usePhoenix;

                                                    GameManager.Instance.AchieveWish();
                                                    yield break;
                                                }
                                            }
                                        }
                                        else if (usePhoenix)
                                        {
                                            //연결되지 않았지만 봉황이 대신할수 있는지 확인한다.
                                            //카운터는 올려준다
                                            //한번만 체크하도록 usePhoenix는 false로
                                            usePhoenix = false;

                                            card.value = m_myCard[i].value + count;
                                            AddSelectedCard(card);
                                            ++count;

                                            if (count == cardCount)
                                            {
                                                bool wishOk = false;
                                                //스트레이트를 찾았는데 위시가 있는지 보자.
                                                for (int k = 0; k < idxList.Count; ++k)
                                                {
                                                    //위시가 있는 스트레이트다
                                                    if (m_myCard[idxList[k]].valueStr == wishCard)
                                                    {
                                                        wishOk = true;
                                                        break;
                                                    }
                                                }

                                                //위시가 없는 스트레이트다 그냥 넘어가자
                                                if (!wishOk)
                                                {
                                                    continue;
                                                }

                                                //내자
                                                //봉황이 마지막이다?
                                                //봉황 값으로 낼 수 있는지 판단하자
                                                if (card.value > topValue)
                                                {
                                                    for (int k = 0; k < idxList.Count; ++k)
                                                    {
                                                        AddSelectedCard(m_myCard[idxList[k]]);
                                                    }
                                                    yield return StartCoroutine(AICardDraw());
                                                    m_bHavePhoenix = false;
                                                    GameManager.Instance.AchieveWish();
                                                    yield break;
                                                }
                                            }

                                            //다시 해당 카드를 확인하기 위해 J를 마이너스
                                            --j;
                                        }
                                    }
                                }
                            }

                            //위시카드가 없는 경우
                            { 
                                int count = 0;
                                bool isStright = false;

                                List<int> idxList = new List<int>();
                                bool usePhoenix = m_bHavePhoenix;
                                CardData card = m_myCard.Find(item => item.type == CARD_TYPE.PHOENIX);

                                for (int i = 0; i < m_myCard.Count - cardCount && !isStright; ++i)
                                {
                                    idxList.Clear();
                                    count = 1;
                                    //isStright = true;
                                    idxList.Add(i);
                                    m_selectedCard.Init();

                                    for (int j = i + 1; j < m_myCard.Count; ++j)
                                    {
                                        if (m_myCard[i].value + count - 1 == m_myCard[j].value)
                                        {
                                            continue;
                                        }

                                        if (m_myCard[i].value + count == m_myCard[j].value)
                                        {
                                            idxList.Add(j);
                                            ++count;

                                            if (count == cardCount)
                                            {
                                                if (m_myCard[j].value > topValue)
                                                {
                                                    for (int k = 0; k < idxList.Count; ++k)
                                                    {
                                                        AddSelectedCard(m_myCard[idxList[k]]);
                                                    }
                                                    yield return StartCoroutine(AICardDraw());
                                                    m_bHavePhoenix = usePhoenix;
                                                    yield break;
                                                }
                                            }
                                        }
                                        else if (usePhoenix)
                                        {
                                            usePhoenix = false;
                                            card.value = m_myCard[i].value + count;
                                            AddSelectedCard(card);
                                            ++count;

                                            if (count == cardCount)
                                            {
                                                if (m_myCard[j].value + 1 > topValue)
                                                {
                                                    for (int k = 0; k < idxList.Count; ++k)
                                                    {
                                                        AddSelectedCard(m_myCard[idxList[k]]);
                                                    }
                                                    yield return StartCoroutine(AICardDraw());
                                                    m_bHavePhoenix = false;
                                                    //GameManager.Instance.AchieveWish();
                                                    yield break;
                                                }
                                            }

                                            //다시 해당 카드를 확인하기 위해 J를 마이너스
                                            --j;
                                        }
                                    }
                                }
                            }
                            
                        }
                        break;
                    case DRAWCARD_TYPE.STRIGHT_BOMB:
                        {
                            //일단 이건 패스;
                        }
                        break;
                }

                m_selectedCard.Init();

                //m_gameManager.Pass();
            }
            
        }

    }
}
