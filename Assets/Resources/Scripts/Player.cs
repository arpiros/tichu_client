using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public Sprite m_tichuIcon;
    public Sprite m_grandTichuIcon;

    public Sprite m_teamNameBar;
    public Sprite m_enemyNameBar;
    public Image m_rotateTurnImage;
    public Text m_cardCountText;

    public Image m_namebar;
    public Image m_tichu;
    public Image m_wishCardImg;
    public Text m_wishText;

    public int offsetX;
    public int offsetY;

    protected bool m_isMajjong;             //참새를 냈는지 확인
    protected bool m_isCallTichu;
    protected bool m_isCallLargeTichu;

    protected bool m_isChooseLargeTichu;

    protected int m_point;
    protected int m_bonusPoint;

    public Text m_pointText;
    public Text m_scoreBoardPointText;
    public Text m_myName;

    protected List<CardData> m_myCard = new List<CardData>();
    protected SelectCard m_selectedCard = new SelectCard();
    protected List<CardData> m_eatCard = new List<CardData>();
    protected CardData[] m_exchagnedCard = new CardData[3];

    protected List<CardData> m_bombCard = new List<CardData>();
    protected List<CardData> m_strightBomCard = new List<CardData>();

    protected bool m_bIsDraw = false;
    protected bool m_bIsExchange = false;
    protected bool m_bDrawMajjong = false;
    protected bool m_bIsMyTurn = false;

    protected bool m_bHavePhoenix = false;

    protected Player m_leftPlayer;
    protected Player m_rightPlayer;
    protected Player m_teamPlayer;

    public bool NonPlayer { get; set; }

    public int TeamID { get; set; }

    protected int m_nPlayerIdx;

    public int PlayerIdx
    {
        get { return m_nPlayerIdx; }
        set { m_nPlayerIdx = value; }
    }

    public Image GetTurnImage()
    {
        return m_rotateTurnImage;
    }

    public bool IsMyTurn()
    {
        return m_bIsMyTurn;
    }

    public bool IsCallTichu()
    {
        return m_isCallTichu;
    }
    
    public bool IsCallLargeTichu()
    {
        return m_isCallLargeTichu;
    }

    public bool DrawBomb()
    {
        if (m_bombCard.Count > 0 || m_strightBomCard.Count > 0)
        {
            //폭탄이 있다.
            if (m_bombCard.Count > 0)
            {
                m_selectedCard.Init();

                //일반 폭탄
                for (int i = 0; i < m_bombCard.Count; ++i)
                {
                    m_selectedCard.Add(m_bombCard[i]);
                }
            }
            else
            {
                //스트레이트 폭탄
                for (int i = 0; i < m_strightBomCard.Count; ++i)
                {
                    m_selectedCard.Add(m_strightBomCard[i]);
                }
            }

            StartCoroutine(CardDraw());
            return true;
        }

        return false;
    }

    private void Awake()
    {
        m_isCallTichu = m_isCallLargeTichu = m_isMajjong = false;
        m_point = 0;
        m_bonusPoint = 0;
        m_pointText.text = name + "\n" + m_point.ToString();
        m_scoreBoardPointText.text = m_pointText.text;
        m_myName.text = name;
        NonPlayer = false;

        m_tichu.gameObject.SetActive(false);

        m_cardCountText.text = m_myCard.Count.ToString() + "장";
    }

    public void HavePhoenix()
    {
        m_bHavePhoenix = true;
    }

    public void InitPlayerInfo()
    {
        m_isCallTichu = m_isCallLargeTichu = m_isMajjong = false;
        m_point = 0;
        m_bonusPoint = 0;
        m_pointText.text = name + "\n" + m_point.ToString();
        m_scoreBoardPointText.text = m_pointText.text;
        NonPlayer = false;
        m_isChooseLargeTichu = false;
        m_bHavePhoenix = false;
        m_bIsExchange = false;
        m_bIsDraw = false;
        m_bDrawMajjong = false;
        GameManager.Instance.TichuBtnEnabled(true);

        m_tichu.gameObject.SetActive(false);
        m_wishCardImg.gameObject.SetActive(false);

        m_pointText.color = new Color(0, 0, 0);
        m_scoreBoardPointText.color = new Color(0, 0, 0);

        m_bombCard.Clear();
        m_strightBomCard.Clear();

        m_cardCountText.text = m_myCard.Count.ToString() + "장";

        for (int i = 0; i < 3; ++i)
        {
            m_exchagnedCard[i] = null;
        }

        m_myCard.Clear();
        m_eatCard.Clear();
        m_selectedCard.Init();

        StopCoroutine(AnimationClip());
    }

    public void SetCardOriginPosition()
    {
        for (int i = 0; i < m_myCard.Count; ++i)
        {
            m_myCard[i].SetOriginPosition();
        }
    }

    public virtual void SetCardCount()
    {
        m_cardCountText.text = m_myCard.Count.ToString() + "장";
    }

    public bool DrawMajjong()
    {
        return m_bDrawMajjong;
    }

    public Player GetTemaPlayer()
    {
        return m_teamPlayer;
    }

    public Player GetLeftPlayer()
    {
        return m_leftPlayer;
    }

    public Player GetRightPlayer()
    {
        return m_rightPlayer;
    }

    public void SetLeftPlayer(ref Player left)
    {
        m_leftPlayer = left;
    }

    public void SetRightPlayer(ref Player right)
    {
        m_rightPlayer = right;
    }

    public void RoundSet()
    {
        m_isCallTichu = m_isCallLargeTichu = m_isMajjong = false;
        m_point = 0;
        m_pointText.text = name + "\n" + m_point.ToString();
        m_scoreBoardPointText.text = m_pointText.text;
    }

    public void AddCard(CardData cardData)
    {
        //cardData.transform.position = transform.position;
        //cardData.transform.rotation = transform.rotation;

        m_myCard.Add(cardData);

        if (cardData.type == CARD_TYPE.MAHJONG)
        {
            m_isMajjong = true;
        }
        else if (cardData.type == CARD_TYPE.PHOENIX)
        {
            m_bHavePhoenix = true;
        }
    }

    virtual public void ArrangementCard()
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
            if (this.name == "Player" && !m_myCard[i].IsSelected())
            {
                m_myCard[i].transform.position = transform.position + m_myCard[i].transform.right * (offset * 110);

                m_myCard[i].SetSortingRayer(i);
            }
            ++offset;
        }
    }

    public void SetOriginTransform()
    {
        int size = m_myCard.Count;

        for (int i = 0; i < size; ++i)
        {
            m_myCard[i].SetOriginPosition();
        }
    }

    public void SetPlayers(ref Player left, ref Player team, ref Player right)
    {
        m_leftPlayer = left;
        m_teamPlayer = team;
        m_rightPlayer = right;
    }

    public bool GetStarter()
    {
        return m_isMajjong;
    }

    public void AddSelectedCard(CardData card)
    {
        if (GameManager.Instance.rutine == RutineState.RoundState)
        {
            //card.Hide(false);
            m_selectedCard.Add(card);
            m_bIsDraw = DrawCheck();
        }
        else if (GameManager.Instance.rutine == RutineState.ExchangeState)
        {
            int index = 0;

            for (int i = 0; i < 3; ++i)
            {
                if (m_exchagnedCard[i] == null)
                {
                    index = i;
                    break;
                }
            }

            m_exchagnedCard[index] = card;
            m_myCard.Remove(card);

            //카드의 위치를 옮겨줘야 된다.
            switch (index)
            {
                case 0:
                    {
                        m_exchagnedCard[index].transform.position = GameManager.Instance.m_ExchangeUi.m_leftCard.transform.position;
                        m_exchagnedCard[index].transform.rotation = GameManager.Instance.m_ExchangeUi.m_leftCard.transform.rotation;
                    }
                    break;

                case 1:
                    {
                        m_exchagnedCard[index].transform.position = GameManager.Instance.m_ExchangeUi.m_partnerCard.transform.position;
                        m_exchagnedCard[index].transform.rotation = GameManager.Instance.m_ExchangeUi.m_partnerCard.transform.rotation;
                    }
                    break;

                case 2:
                    {
                        m_exchagnedCard[index].transform.position = GameManager.Instance.m_ExchangeUi.m_rightCard.transform.position;
                        m_exchagnedCard[index].transform.rotation = GameManager.Instance.m_ExchangeUi.m_rightCard.transform.rotation;
                    }
                    break;
            }
            
            for (int i = 0; i < 3; ++i)
            {
                m_bIsExchange = true;
                if (m_exchagnedCard[i] == null)
                {
                    m_bIsExchange = false;
                    break;
                }
            }
        }
    }

    public void ReleaseCard(CardData card)
    {
        if (GameManager.Instance.rutine == RutineState.RoundState)
        {
            m_selectedCard.ReleaseCard(card);
            m_bIsDraw = DrawCheck();
            ArrangementCard();
        }
        else if (GameManager.Instance.rutine == RutineState.ExchangeState)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (m_exchagnedCard[i] == card)
                {
                    m_exchagnedCard[i] = null;
                    break;
                }
            }

            m_myCard.Add(card);

            card.MoveOriginPosition();
            //m_exchagnedCard[idx].MoveOriginPosition();

            //삭제하면 무조건 false지?
            m_bIsExchange = false;
        }
    }

    public void SwapExchangeCard(int idx1, int idx2)
    {
        //idx1 = 포지션 안 바꿔도 됨, idx2 = 포지션을 바꿔줘야 됨
        CardData temp = m_exchagnedCard[idx1];
        m_exchagnedCard[idx1] = m_exchagnedCard[idx2];
        m_exchagnedCard[idx2] = temp;

        if (m_exchagnedCard[idx2] == null)
            return;

        //카드의 위치를 옮겨줘야 된다.
        switch (idx2)
        {
            case 0:
                {
                    m_exchagnedCard[idx2].transform.position = GameManager.Instance.m_ExchangeUi.m_leftCard.transform.position;
                    m_exchagnedCard[idx2].transform.rotation = GameManager.Instance.m_ExchangeUi.m_leftCard.transform.rotation;
                }
                break;

            case 1:
                {
                    m_exchagnedCard[idx2].transform.position = GameManager.Instance.m_ExchangeUi.m_partnerCard.transform.position;
                    m_exchagnedCard[idx2].transform.rotation = GameManager.Instance.m_ExchangeUi.m_partnerCard.transform.rotation;
                }
                break;

            case 2:
                {
                    m_exchagnedCard[idx2].transform.position = GameManager.Instance.m_ExchangeUi.m_rightCard.transform.position;
                    m_exchagnedCard[idx2].transform.rotation = GameManager.Instance.m_ExchangeUi.m_rightCard.transform.rotation;
                }
                break;
        }
    }

    public void AddExchangeCard(CardData card, int index)
    {
        card.SetSelect(true);
        //만약 null인 경우는 그냥 카드를 등록만 해줌
        if (m_exchagnedCard[index] == null)
        {
            //만약 옆에서 옮겨간 경우는?
            if (m_myCard.Remove(card))
            {
                m_exchagnedCard[index] = card;
            }
            else
            {
                //내 카드중에 없다. 옆에서 옮겨간 경우다.
                for (int i = 0; i < 3; ++i)
                {
                    if (i != index)
                    {
                        if (m_exchagnedCard[i] == card)
                        {
                            //어디서 옮기고 있는지 위치를 찾음
                            //해당 위치랑 바꿔줘야 됨
                            //포지션도 바꿔줘야 될듯;
                            SwapExchangeCard(index, i);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            //만약 옆에서 옮겨간 경우는?
            if (m_myCard.Remove(card))
            {
                //null이 아닌 경우 카드가 있으니 교체를 해주자
                //일단 안에 값을 빼주자.
                m_exchagnedCard[index].MoveOriginPosition();
                m_exchagnedCard[index].SetSelect(false);
                m_myCard.Add(m_exchagnedCard[index]);
                m_exchagnedCard[index] = null;

                //그리고 새 값을 넣는다.
                m_exchagnedCard[index] = card;
            }
            else
            {
                //내 카드중에 없다. 옆에서 옮겨간 경우다.
                for (int i = 0; i < 3; ++i)
                {
                    if (i != index)
                    {
                        if (m_exchagnedCard[i] == card)
                        {
                            //어디서 옮기고 있는지 위치를 찾음
                            //해당 위치랑 바꿔줘야 됨
                            SwapExchangeCard(index, i);
                            break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < 3; ++i)
        {
            m_bIsExchange = true;
            if (m_exchagnedCard[i] == null)
            {
                m_bIsExchange = false;
                break;
            }
        }
    }

    public void RemoveExchangeCard(CardData card, int index)
    {
        m_exchagnedCard[index] = null;
        m_myCard.Add(card);

        m_bIsExchange = false;
    }

    public virtual bool DrawCheck()
    {
        int size = m_selectedCard.GetSize();

        //리스트가 비어있으면 체크할 필요가 없다.
        if (size <= 0)
        {
            GameManager.Instance.SetActivePlayBtn(false);
            return false;
        }

        GameManager.Instance.SetActivePlayBtn(true);

        if (GameManager.Instance.GetTurnPlayer() == this)
        {
            GameManager.Instance.m_drawBtn.Active();
        }
        else
        {
            GameManager.Instance.m_drawBtn.Inactive();
        }

        DRAWCARD_TYPE eDrawCardType = CardDrawHandler.Instance.GetDrawCardType();

        //참새가 있다.
        if (GameManager.Instance.m_wishCardPopup.Selected())
        {
            string wishCardStr = GameManager.Instance.m_wishCardPopup.GetWishCard();

            //내가 선택한 카드중에 소원이 있다 없다?
            //내 카드중에 위시카드가 있고 선택한 카드안에 소원이 있는지 체크
            if (HaveWishCard(wishCardStr))
            {
                //낼수가 있다.
                if (IsDrawWishCard())
                {
                    //선택카드에 wish가 없으면 못낸다.
                    if (!m_selectedCard.FindWishCard(wishCardStr))
                    {
                        return false;
                    }
                }
            }
        }

        // NONE 타입이거나 나랑 타입이 같으면 낼수 있다.
        if (eDrawCardType == DRAWCARD_TYPE.NONE)
        {
            return m_selectedCard.GetDrawCardType() != DRAWCARD_TYPE.NONE;
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

    public void CardDrawBtn()
    {
        StartCoroutine(CardDraw());
    }

    public IEnumerator CardDraw()
    {
        //if (!m_bIsMyTurn)
        //{
        //    yield break;
        //}

        //m_bIsMyTurn = false;

        if (m_bIsDraw)
        {
            //카드를 내면 티츄를 포기하게 된다.
            if (GameManager.Instance.m_tichuBtn.IsActive())
            {
                //GameManager.Instance.m_tichuBtn.gameObject.SetActive(false);
                GameManager.Instance.m_tichuBtn.Inactive();
            }

            //카드를 냈으면 바로 비활성화 시켜주자.
            //GameManager.Instance.m_drawBtn.SetActive(false);
            GameManager.Instance.m_drawBtn.Hide();

            GameManager.Instance.SetTopPlayer(this);

            List<CardData> selectedCardList = m_selectedCard.GetCardList();
            CardDrawHandler.Instance.CardDrawAtList(selectedCardList, m_selectedCard.GetDrawCardType(), m_selectedCard.GetTopValue());

            //참새가 있다.
            if (GameManager.Instance.m_wishCardPopup.Selected())
            {
                string wishCardStr = GameManager.Instance.m_wishCardPopup.GetWishCard();

                //내가 선택한 카드중에 소원이 있다 없다?
                //내 카드중에 위시카드가 있고 선택한 카드안에 소원이 있는지 체크
                if (HaveWishCard(wishCardStr) && m_selectedCard.FindWishCard(wishCardStr))
                {
                    GameManager.Instance.AchieveWish();
                }
            }

            for (int i = 0; i < selectedCardList.Count; ++i)
            {
                if (selectedCardList[i].value == 1)
                {
                    m_bDrawMajjong = true;
                    break;
                }
            }

            int size = m_selectedCard.GetSize();
            int offset = -(size / 2);

            string temp = "Palyer 드로우 카드 : ";

            List<Vector3> endPosList = new List<Vector3>();

            for (int i = 0; i < selectedCardList.Count; ++i)
            {
                Vector3 endPos;
                endPos = CardDrawHandler.Instance.transform.position + CardDrawHandler.Instance.transform.right * (offset * 70);
                endPos = new Vector3(endPos.x + offsetX, endPos.y + offsetY, 0);
                endPosList.Add(endPos);

                temp += selectedCardList[i].value + " ";

                ++offset;
                m_myCard.Remove(selectedCardList[i]);
            }

            yield return StartCoroutine(m_selectedCard.CardDrawMove(endPosList));

            ////내 카드 목록에서 빼주자.
            //for (int i = 0; i < selectedCardList.Count; ++i)
            //{
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

            //Invoke("Pass", 2f);
            Pass();
            ArrangementCard();

            m_cardCountText.text = m_myCard.Count.ToString() + "장";

            //GameManager.Instance.m_bombBtn.gameObject.SetActive(CheckBomb());
            if (CheckBomb())
            {
                GameManager.Instance.m_bombBtn.Active();
            }
            else
            {
                GameManager.Instance.m_bombBtn.Inactive();
            }
        }
        else
        {
            //못 내는데 눌렸다?
            //일단 선택한건 없애줘야 되지 않을까?
            //초기화 하면서 버튼도 다시 내린다.
            m_selectedCard.Init();
            ArrangementCard();
            //GameManager.Instance.m_drawBtn.SetActive(false);
            GameManager.Instance.m_drawBtn.View();
            //GameManager.Instance.m_passBtn.SetActive(true);
        }
    }

    public void Pass()
    {
        //GameManager.Instance.Pass();
        //if (!m_bIsMyTurn)
        //{
        //    return;
        //}

        //m_bIsMyTurn = false;

        if (CardDrawHandler.Instance.GetDrawCardType() == DRAWCARD_TYPE.NONE)
        {
            //없는 경우는 내가 선이다 선일때 패스를 하면 안된다
            //그냥 리턴하자
            return;
        }

        if (m_rotateTurnImage.gameObject.activeSelf)
        {
            m_rotateTurnImage.gameObject.SetActive(false);
        }

        StartCoroutine(GameManager.Instance.Pass());
    }

    virtual public void MyTurn()
    {
        if (CardDrawHandler.Instance.GetDrawCardType() == DRAWCARD_TYPE.NONE)
        {
            //NONE인 경우 아무것도 안 나왔다.
            //패스를 비활성화 시켜주자.
            //GameManager.Instance.m_passBtn.SetActive(false);
        }

        //GameManager.Instance.m_bombBtn.gameObject.SetActive(CheckBomb());

        GameManager.Instance.m_drawBtn.Active();
        GameManager.Instance.m_passBtn.Active();

        m_bIsMyTurn = true;
        m_bDrawMajjong = false;
        DrawCheck();
        Debug.Log(this.name + "의 턴\n");

        StartCoroutine(AnimationClip());
    }

    public void CalcPoint()
    {
        int eatCardSize = m_eatCard.Count;

        for (int i = 0; i < eatCardSize; ++i)
        {
            m_point += m_eatCard[i].point;
        }

        m_pointText.text = name + "\n" + m_point.ToString();
        m_scoreBoardPointText.text = m_pointText.text;
    }

    public int CalcHandCardPoint()
    {
        int point = 0;
        int size = m_myCard.Count;

        for (int i = 0; i < size; ++i)
        {
            point += m_myCard[i].point;
        }

        return point;
    }

    public void GivePoint(int point)
    {
        m_point += point;
    }

    public int GetPoint()
    {
        return m_point;
    }

    public void SetPoint(int point)
    {
        m_point = point;
    }

    public int GetBonusPoint()
    {
        return m_bonusPoint;
    }

    public int GetCardCount()
    {
        return m_myCard.Count;
    }

    virtual public bool IsExchagne()
    {
        return m_bIsExchange;
    }

    public void CallTichu()
    {
        m_isCallTichu = true;
        GameManager.Instance.TichuBtnEnabled(false);

        m_tichu.gameObject.SetActive(true);
        m_tichu.sprite = m_tichuIcon;
    }

    virtual public IEnumerator CardExchange(float deltaTime = 0.5f)
    {
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

        while (time >= deltaTime)
        {
            for (int i = 0; i < 3; ++i)
            {
                Vector3 startPos = exchangeCardPosList[i];
                //Vector3 startPos = m_exchagnedCard[i].transform.position;
                Vector3 endPos = endPosList[i];
                Vector3 moveVec = endPos - startPos;

                m_exchagnedCard[i].transform.position = startPos + (moveVec * (time / deltaTime));
            }
            time += Time.deltaTime;
            yield return null;
        }

        //마지막 도착 위치가 일치하도록 해주자.
        for (int i = 0; i < 3; ++i)
        {
            Vector3 startPos = exchangeCardPosList[i];
            Vector3 endPos = endPosList[i];
            Vector3 moveVec = endPos - startPos;

            m_exchagnedCard[i].transform.position = startPos + (moveVec * 1);
        }

        //보내고 나서 숨겨야 된다.
        //내가 보낼 카드들은 다 숨겨야 되니 전부 hide
        for (int i = 0; i < 3; ++i)
        {
            m_exchagnedCard[i].Hide();
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

    public CardData GetCardExchangeData(int index)
    {
        return m_exchagnedCard[index];
    }

    public void ViewExchangeCard()
    {
        //그냥 내 카드 전부다 보여주게 만들자.
        for (int i = 0; i < m_myCard.Count; ++i)
        {
            m_myCard[i].Flip(false);
            m_myCard[i].Hide(false);
        }
    }

    virtual public void clearExchangeData()
    {
        for (int i = 0; i < 3; ++i)
        {
            m_myCard.Remove(m_exchagnedCard[i]);
        }

        m_exchagnedCard[0] = null;
        m_exchagnedCard[1] = null;
        m_exchagnedCard[2] = null;
        ArrangementCard();
    }

    public IEnumerator SetEatCard(List<CardData> cardDrawList)
    {
        yield return StartCoroutine(CardDrawHandler.Instance.EatCardMove(this.transform.position));

        int cardDrawListSize = cardDrawList.Count;

        for (int i = 0; i < cardDrawListSize; ++i)
        {
            m_point += cardDrawList[i].point;

            //cardDrawList[i].Hide();
            cardDrawList[i].gameObject.SetActive(false);
        }

        m_eatCard.AddRange(cardDrawList);
        SetPoint();
    }

    private void SetPoint()
    {
        m_pointText.text = name + "\n" + m_point.ToString();
        m_scoreBoardPointText.text = m_pointText.text;

        //(m_point > 0) ? m_pointText.color = new Color(0, 0, 0) : m_pointText.color = new Color(255, 0, 0);

        if (m_point >= 0)
        {
            m_pointText.color = new Color(0, 0, 0);
            m_scoreBoardPointText.color = new Color(0, 0, 0);
        }
        else
        {
            m_pointText.color = new Color(248 / 255.0f, 84 / 255.0f, 84 / 255.0f);
            m_scoreBoardPointText.color = new Color(248 / 255.0f, 84 / 255.0f, 84 / 255.0f);
        }
    }

    virtual public bool IsChooseLargeTichu()
    {
        return m_isChooseLargeTichu;
    }

    public void SuccesFirst(bool isSucces)
    {
        //1등으로 나는데 성공했다.!
        if (isSucces)
        {
            //라지 티츄를 했는가?
            if (m_isCallLargeTichu)
            {
                //200점 추가!
                m_bonusPoint += 200;
            }
            else if(m_isCallTichu)
            {
                //티츄를 했는가?
                m_bonusPoint += 100;
            }
        }
        else
        {
            //티츄를 했는가?
            if (m_isCallLargeTichu)
            {
                //200점 마이너스
                m_bonusPoint -= 200;
            }
            else if (m_isCallTichu)
            {
                //티츄를 했는가?
                m_bonusPoint -= 100;
            }
        }
    }

    //라지티츄를 불렀다.
    public void SelectYes()
    {
        m_isChooseLargeTichu = true;
        m_isCallLargeTichu = true;

        m_isCallTichu = false;

        //스몰 티츄 버튼을 없애자.
        GameManager.Instance.TichuBtnEnabled(false);

        m_tichu.gameObject.SetActive(true);
        m_tichu.sprite = m_grandTichuIcon;
    }

    //라지티츄를 안했다.
    public void SelectNo()
    {
        m_isChooseLargeTichu = true;
        m_isCallLargeTichu = false;
    }

    virtual public IEnumerator ChooseEatDragon()
    {
        yield break;
    }

    virtual protected IEnumerator AnimationClip()
    {
        //애니메이션 클립을 활성화 하고
        //종료 될때까지 계속 회전 시킨다.
        m_rotateTurnImage.gameObject.SetActive(true);

        while (m_rotateTurnImage.gameObject.activeSelf)
        {
            m_rotateTurnImage.transform.Rotate(Vector3.forward * Time.deltaTime * -300f);

            yield return null;
        }
    }

    public void CallWishCard(string wish)
    {
        m_wishCardImg.gameObject.SetActive(true);
        m_wishText.text = wish;
    }

    public void ClearWish()
    {
        m_wishCardImg.gameObject.SetActive(false);
    }

    public bool HaveWishCard(string wishCardStr)
    {
        return m_myCard.Find(card => card.valueStr == wishCardStr);
    }

    public bool CheckBomb()
    {
        //같은 숫자 폭탄과
        //연속된 숫자이면서 색깔이 똑같은 폭탄이 있는지 확인
        for (int i = 0; i < m_myCard.Count - 1; ++i)
        {
            m_bombCard.Clear();
            m_bombCard.Add(m_myCard[i]);

            for (int j = i + 1; j < m_myCard.Count; ++j)
            {
                if (m_myCard[i].value == m_myCard[j].value)
                {
                    m_bombCard.Add(m_myCard[j]);

                    if (m_bombCard.Count >= 4)
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        //같은 숫자 폭탄이 없다?
        //스트레이트 폭탄을 찾아보자.
        for (int i = 0; i < m_myCard.Count - 5; ++i)
        {
            m_strightBomCard.Clear();
            m_strightBomCard.Add(m_myCard[i]);
            CardData start = m_myCard[i];

            for (int j = i + 1; j < m_myCard.Count; ++j)
            {
                if (start.value == m_myCard[j].value)
                {
                    continue;
                }

                if (start.value + 1 == m_myCard[j].value)
                {
                    if (start.color == m_myCard[j].color)
                    {
                        m_strightBomCard.Add(m_myCard[j]);

                        if (m_strightBomCard.Count >= 5)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return false;
    }

    public bool IsPossibleDraw()
    {
        return false;
    }

    public bool IsDrawWishCard()
    {
        //소원 카드를 낼 수 있는지 없는지 확인하자
        DRAWCARD_TYPE eCardType = CardDrawHandler.Instance.GetDrawCardType();
        float topValue = CardDrawHandler.Instance.GetTopValue();
        float wishCardValue = GameManager.Instance.m_wishCardPopup.GetWishCardValue();
        string wishStr = GameManager.Instance.m_wishCardPopup.GetWishCard();
        int cardCount = CardDrawHandler.Instance.GetCardCount();

        if (topValue < wishCardValue)
        {
            //내가 들고만 있으면 무조건 내야 되는 경우
            //시작, 싱글, 페어, 트리플
            switch (eCardType)
            {
                case DRAWCARD_TYPE.NONE:
                case DRAWCARD_TYPE.SINGLE:
                    {
                        if (HaveWishCard(wishStr))
                        {
                            return true;
                        }

                        return false;
                    }
                case DRAWCARD_TYPE.PAIR:
                    {
                        int wishCardCount = 0;

                        //wish랑 같은 카드가 2장 이상이다 그럼 낼수 있다.
                        for (int i = 0; i < m_myCard.Count; ++i)
                        {
                            if (m_myCard[i].valueStr == wishStr)
                            {
                                ++wishCardCount;

                                if (wishCardCount >= 2)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }
                case DRAWCARD_TYPE.TRIPPLE:
                    {
                        int wishCardCount = 0;

                        //wish랑 같은 카드가 3장 이상이다 그럼 낼수 있다.
                        for (int i = 0; i < m_myCard.Count; ++i)
                        {
                            if (m_myCard[i].valueStr == wishStr)
                            {
                                ++wishCardCount;

                                if (wishCardCount >= 2)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }
            }
        }

        //위시랑 top이랑 상관없이 조합을 비교해야 되는 경우
        //연속 페어, 스트레이트, 풀하우스, 
        switch (eCardType)
        {
            case DRAWCARD_TYPE.FULLHOUSE:
                {
                    //위시 카드로 트리플이나 페어가 있는지 확인하고
                    //트리플이 top보다 높은지 확인을 해야 된다.
                    if (topValue > wishCardValue)
                    {
                        //탑이 위시보다 높은 경우
                        //위시는 트리플이 아니고 페어로 들고 있어야 되며
                        //트리플이 탑보다 높아야 된다.
                        int wishCardCount = 0;

                        for (int i = 0; i < m_myCard.Count; ++i)
                        {
                            if (m_myCard[i].valueStr == wishStr)
                            {
                                ++wishCardCount;

                                if (wishCardCount >= 2)
                                {
                                    break;
                                }
                            }
                        }

                        if (wishCardCount < 2)
                        {
                            return false;
                        }
                        else
                        {
                            //트리플을 찾아보자 트리플을 찾아도 top벨류보다 커야 된다.
                            for (int i = 0; i < m_myCard.Count - 2; ++i)
                            {
                                if (m_myCard[i].value == m_myCard[i + 1].value
                                    && m_myCard[i + 1].value == m_myCard[i + 2].value)
                                {
                                    if (topValue < m_myCard[i].value)
                                    {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        }
                    }
                    else
                    {
                        //위시로 트리플이나 페어가 있는지 확인한다.
                        int wishCardCount = 0;

                        for (int i = 0; i < m_myCard.Count; ++i)
                        {
                            if (m_myCard[i].valueStr == wishStr)
                            {
                                ++wishCardCount;

                                if (wishCardCount >= 3)
                                {
                                    break;
                                }
                            }
                        }

                        if (wishCardCount == 3)
                        {
                            //위시카드로 트리플이 있다. 페어가 있는지 찾자
                            for (int i = 0; i < m_myCard.Count - 1; ++i)
                            {
                                if (m_myCard[i].value == m_myCard[i + 1].value)
                                {
                                    if (m_myCard[i].valueStr != wishStr)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else if (wishCardCount == 2)
                        {
                            //위시로 페어가 있다. 트리플이 있는지 찾아보자.
                            //트리플을 찾아보자 트리플을 찾아도 top벨류보다 커야 된다.
                            for (int i = 0; i < m_myCard.Count - 2; ++i)
                            {
                                if (m_myCard[i].value == m_myCard[i + 1].value
                                    && m_myCard[i + 1].value == m_myCard[i + 2].value)
                                {
                                    if (topValue < m_myCard[i].value)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }

                        return false;
                    }
                }
            case DRAWCARD_TYPE.STRIGHT:
                {
                    int straightCount = 0;
                    List<int> idxList = new List<int>();

                    //스트레이트를 찾아보자.
                    for (int i = 0; i < m_myCard.Count - cardCount; ++i)
                    {
                        straightCount = 1;
                        idxList.Clear();
                        idxList.Add(i);

                        for (int j = i + 1; j < m_myCard.Count; ++j)
                        {
                            if (m_myCard[i].value + straightCount - 1 == m_myCard[j].value)
                            {
                                continue;
                            }

                            if (m_myCard[i].value + straightCount == m_myCard[j].value)
                            {
                                ++straightCount;

                                if (straightCount == cardCount)
                                {
                                    bool wishOk = false;
                                    //스트레이트를 찾았는데 위시가 있는지 보자.
                                    for (int k = 0; k < idxList.Count; ++k)
                                    {
                                        //위시가 있는 스트레이트다
                                        if (m_myCard[idxList[k]].valueStr == wishStr)
                                        {
                                            wishOk = true;
                                        }
                                    }

                                    //top보다 크면 낼수 있다.
                                    if (wishOk && m_myCard[j].value > topValue)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }

                    }

                    //볼짱 다 봤는데도 리턴이 안된경우면 없다고 판단
                    return false;
                }
            case DRAWCARD_TYPE.PAIRS:
                {
                    //이건 버그 해결되면 찾아보자.
                    return true;
                }
        }

        return false;
    }

    virtual public IEnumerator DivisionCardMove(Vector3 startPosition, bool isFirst, float deltaTime = 0.5f)
    {
        float time = 0;

        int size = m_myCard.Count;
        List<Vector3> startPosList = new List<Vector3>();

        for (int i = 0; i < size; ++i)
        {
            startPosList.Add(m_myCard[i].transform.position);
        }

        int offset = -(size / 2);

        while (time <= deltaTime)
        {
            offset = -(size / 2);

            for (int i = 0; i < size; ++i)
            {
                Vector3 endPos = transform.position + (transform.right * (offset * 110));
                Vector3 moveVec = endPos - startPosList[i];

                m_myCard[i].transform.position = startPosList[i] + (moveVec * (time / deltaTime));
                ++offset;
            }

            time += Time.deltaTime;
            yield return null;
        }

        offset = -(size / 2);

        for (int i = 0; i < size; ++i)
        {
            Vector3 endPos = transform.position + (transform.right * (offset * 110));
            Vector3 moveVec = endPos - startPosList[i];

            m_myCard[i].transform.position = startPosList[i] + (moveVec * 1);
            ++offset;
        }

        for (int i = 0; i < size; ++i)
        {
            m_myCard[i].Flip(false);
        }
    }
}
