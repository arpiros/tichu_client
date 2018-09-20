using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

public enum RutineState
{
    FirstDivisionState,
    ChooseLargeTichuState,
    SecondDivisionState,
    ExchangeState,
    RoundState,
    RoundEndState,
    CardMoveState,
}

public class GameManager : MonoBehaviour {

    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                {
                    Debug.LogError("게임매니저가 만들어지지 않았습니다.");
                }
            }

            return _instance;
        }
    }

    public Player playerPrefab;

    public Player m_player;
    public CardDeck m_cardDeck;
    const int playerSize = 4;
    public Player[] m_players = new Player[playerSize];

    public int[] m_teamFlag = { 0, 0 };

    private Player m_turnPlayer;        //현재 카드를 내는 플레이어
    private Player m_topPlayer;         //탑값을 낸 플레이어
    private Player m_firstPlayer;       //1등 플레이어 나중에 티츄여부와 꼴지 점수를 가져가기 위해 필요함
    private Player m_startPlayer;       //시작 플레이어 스택을 쌓기 위해 필요
    private Player m_wishCallPlayer;    //참새 소원을 외친 플레이어

    public ExchagneUI m_ExchangeUi;

    //티츄 관련 버튼들
    public RButton m_drawBtn;
    public RButton m_passBtn;
    public RButton m_bombBtn;
    public RButton m_tichuBtn;
    public Button m_exchangeBtn;

    //팝업창
    public Image m_LargeTichuPopup;
    public ScoreBoard m_scoreBoardPopup;
    public WishCardPopup m_wishCardPopup;
    public GivePointPopup m_givePointPopup;
    public GameObject m_askMultiPopup;
    public GameObject m_CreateRoomPopup;
    public GameObject m_JoinRoomPopup;
    public WaitUserPopup m_wiatUserPopup;

    public bool m_endRound = false;

    public Text m_teamAPoint;
    public Text m_teamBPoint;
    public Text m_scoreBoardTeamAPoint;
    public Text m_scoreBoardTeamBPoint;

    string m_roomCode;

    bool m_isExchange = false;
    bool m_isAllReady = false;
    bool m_isClosePointPopup = false;
    bool m_isMultiPlay = false;
    bool m_isStartMultiPlay = false;

    int m_nTeamAPoint = 0;
    int m_nTeamBPoint = 0;

    int count = 0;

    //이번 라운드에 돈 바퀴횟수
    int m_nRoundTurn = 1;

    public int RoundTurn
    {
        get { return m_nRoundTurn; }
        set { m_nRoundTurn = value; }
    }

    public Player GetTurnPlayer()
    {
        return m_turnPlayer;
    }

    private RutineState m_rutineState;

    public RutineState rutine
    {
        get { return m_rutineState; }
        set { m_rutineState = value; }
    }

    public void ChooseLargeTichuCofirm(bool bIsLargeTichu)
    {
        m_rutineState = RutineState.SecondDivisionState;
    }

    private void Awake()
    {
        m_isExchange = false;
        m_ExchangeUi.Hide();

        m_players[0].SetPlayers(ref m_players[1], ref m_players[2], ref m_players[3]);
        m_players[1].SetPlayers(ref m_players[2], ref m_players[3], ref m_players[0]);
        m_players[2].SetPlayers(ref m_players[3], ref m_players[0], ref m_players[1]);
        m_players[3].SetPlayers(ref m_players[0], ref m_players[1], ref m_players[2]);

        //팀 설정
        m_players[0].TeamID = m_players[2].TeamID = 0;
        m_players[1].TeamID = m_players[3].TeamID = 1;

        m_teamAPoint.text = "0";
        m_teamBPoint.text = "0";
        m_scoreBoardTeamAPoint.text = m_teamAPoint.text;
        m_scoreBoardTeamBPoint.text = m_teamBPoint.text;

        m_firstPlayer = null;
    }

    public void TichuBtnEnabled(bool isEnable)
    {
        if (isEnable)
        {
            m_tichuBtn.Active();
        }
        else
        {
            m_tichuBtn.Inactive();
        }
    }

    public void InitRound()
    {
        count = 0;
        for (int i = 0; i < 4; ++i)
        {
            m_players[i].InitPlayerInfo();
        }

        m_teamFlag[0] = 0;
        m_teamFlag[1] = 0;

        CardDrawHandler.Instance.initCardDraw();
        m_wishCardPopup.Init();
        m_endRound = false;
        m_cardDeck.Init();

        m_turnPlayer = null;
        m_startPlayer = null;
        m_topPlayer = null;
        m_firstPlayer = null;
        m_wishCallPlayer = null;

        m_isAllReady = false;
        m_isExchange = false;
        m_isClosePointPopup = false;

        m_rutineState = RutineState.FirstDivisionState;

        m_tichuBtn.Active();
        m_bombBtn.Active();
    }

    // Use this for initialization
    void Start () {
        //코루틴을 만든다.
        StartCoroutine(SelectSingleOrMulti());
        //StartCoroutine("TichuRoutine");
    }

    public void ChangeRoomCode(string roomCode)
    {
        m_roomCode = roomCode;
    }

    IEnumerator SelectSingleOrMulti()
    {
        yield return StartCoroutine(WaitSelectMulti());

        if (!m_isMultiPlay)
        {
            StartCoroutine(TichuRoutine());
        }
        else
        {
            StartCoroutine(ActiveMulti());
        }

        //yield return null;
    }

    IEnumerator ActiveMulti()
    {
        //서버와 연결을 해두고
        //방 생성과 방 입장을 해보자.
        m_CreateRoomPopup.SetActive(true);
        Network.Instance.Connect();

        while (!m_isStartMultiPlay)
        {
            yield return null;
        }
    }

    IEnumerator WaitJoinUser()
    {
        yield return null;
    }

    public void JoinRoomPopup()
    {
        m_CreateRoomPopup.SetActive(false);
        m_JoinRoomPopup.SetActive(true);
    }

    IEnumerator WaitSelectMulti()
    {
        m_askMultiPopup.SetActive(true);

        while (m_askMultiPopup.activeSelf)
        {
            yield return null;
        }
    }

    public void SelectPlayMulti()
    {
        m_isMultiPlay = true;
        m_askMultiPopup.SetActive(false);
    }

    public void CancelPlayMulti()
    {
        m_isMultiPlay = false;
        m_askMultiPopup.SetActive(false);
    }

    IEnumerator TichuRoutine()
    {
        ////메인 루틴 들어가기 전에 멀티 할지를 물어보자.
        //yield return StartCoroutine(WaitSelectMulti());

        //if (m_isMultiPlay)
        //{
        //    yield break;
        //}

        //다시 생각을 해보자.
        //일단 순서도 흐름대로 만들다.
        //1. 카드 분배 (8장 분배)
        //2. 라지 티츄 결정 여부
        //3. 2차 카드 분배 (6장 분배)
        //4. 카드 교환
        //5. 티츄 버튼 활성화
        //6. 게임 진행
        //7. 라운드 진행
        //8. 라운드 종료
        //9. 점수 계산
        //7로 돌아간다.
        //10 총점 합산이 1000점 이상 -> 게임 종료

        //일단 총점 합산이 1000점 이상 나올때까지 무한 루프
        while(m_nTeamAPoint < 1000 && m_nTeamBPoint < 1000)
        {
            //1차 카드 분배
            //서버에서 1차분배가 되고 온다.
            //기존 함수를 수정해서 분배만 시키자.
            yield return StartCoroutine(FirstDivision());

            //라지 티츄 결정하는 팝업창
            yield return StartCoroutine(ChooseLargeTichu());

            //카드 교환
            yield return StartCoroutine(WaitingCardExchange());

            //라운드 진행용 코루틴
            //코루틴 안에서 종료되면 밖으로 나오자
            yield return StartCoroutine(RoundPlay());

            m_teamAPoint.text = m_nTeamAPoint.ToString();
            m_teamBPoint.text = m_nTeamBPoint.ToString();
            m_scoreBoardTeamAPoint.text = m_teamAPoint.text;
            m_scoreBoardTeamBPoint.text = m_teamBPoint.text;

            InitRound();
        }

        if (m_nTeamAPoint > m_nTeamBPoint)
        {
            //승자 결정
            Debug.Log("A 팀 승리");
        }
        else
        {
            Debug.Log("B 팀 승리");
        }

        yield return null;
    }

    IEnumerator CardMove(Vector3 startPosition, Vector3 endPosition, CardData card, float deltaTime = 1.0f)
    {
        Vector3 moveVec = endPosition - startPosition;
        float time = 0;

        while (time < deltaTime)
        {
            card.transform.position = startPosition + (moveVec * (time / deltaTime));
            time += Time.deltaTime;
            yield return null;
        }

        card.transform.position = startPosition + (moveVec * 1);
        time += Time.deltaTime;
    }

    IEnumerator FirstDivision()
    {
        yield return StartCoroutine(m_player.DivisionCardMove(m_cardDeck.transform.position, true));
        m_player.ArrangementCard();
        m_player.SetCardCount();

        if (m_player.CheckBomb())
        {
            m_bombBtn.Active();
        }
        else
        {
            m_bombBtn.Inactive();
        }

        m_rutineState = RutineState.ChooseLargeTichuState;
    }

    IEnumerator ChooseLargeTichu()
    {
        m_LargeTichuPopup.gameObject.SetActive(true);

        while(m_rutineState != RutineState.SecondDivisionState)
        {
            yield return null;
        }
    }

    IEnumerator WaitingCardExchange()
    {
        //교환전에 카드를 자기자리에 셋팅을 하자
        m_players[0].SetCardOriginPosition();

        while (!m_isExchange)
        {
            //m_isAllReady = true;

            //for (int i = 0; i < playerSize; ++i)
            //{
            //    if (!m_players[i].IsExchagne())
            //    {
            //        m_isAllReady = false;
            //        break;
            //    }
            //}

            yield return null;
        }

        Debug.Log("교환 되었습니다.");

        m_rutineState = RutineState.RoundState;

        m_ExchangeUi.Hide();
        m_exchangeBtn.gameObject.SetActive(false);
        SetActivePlayBtn(false);

        m_turnPlayer = m_cardDeck.GetmahjongCard().m_ownerPlayer;
        m_startPlayer = m_turnPlayer;

        m_turnPlayer.MyTurn();

        if (m_turnPlayer == m_players[0])
        {
            m_passBtn.View();
        }
        else
        {
            m_passBtn.Hide();
            m_drawBtn.Hide();
        }
    }

    IEnumerator RoundPlay()
    {        
        while (!m_endRound)
        {
            yield return null;
        }
    }

    IEnumerator WaitWishSelect()
    {
        if (m_turnPlayer.name == "Player")
        {
            m_wishCardPopup.gameObject.SetActive(true);
        }
        else
        {
            string[] wishList = { "X", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            int selectWish = Random.Range(0, 13);
            m_wishCardPopup.NPCPlayerCallWish(wishList[selectWish]);
        }

        while (m_wishCardPopup.gameObject.activeSelf)
            yield return null;

        //빠져나왔으면 소원이 선택되었다.
        if (m_wishCardPopup.Selected())
        {
            m_turnPlayer.CallWishCard(m_wishCardPopup.GetWishCard());
            m_wishCallPlayer = m_turnPlayer;
        }
    }

    IEnumerator WaitWhoEatDragonCard()
    {
        if (m_turnPlayer.name == "Player")
        {
            m_givePointPopup.gameObject.SetActive(true);
            m_givePointPopup.SetInfo(m_turnPlayer.GetLeftPlayer().name, m_turnPlayer.GetRightPlayer().name);
        }
        else
        {
            yield return StartCoroutine(m_turnPlayer.ChooseEatDragon());
        }

        while (!m_isClosePointPopup)
        {
            yield return null;
        }
    }

    public IEnumerator GiveLeftPlayer()
    {
        m_givePointPopup.gameObject.SetActive(false);
        yield return StartCoroutine(m_turnPlayer.GetLeftPlayer().SetEatCard(CardDrawHandler.Instance.GetDrawCardList()));

        m_isClosePointPopup = true;
    }

    public void GiveLeftPlayerBtn()
    {
        StartCoroutine(GiveLeftPlayer());
    }

    public IEnumerator GiveRightPlayer()
    {
        m_givePointPopup.gameObject.SetActive(false);
        yield return StartCoroutine(m_turnPlayer.GetRightPlayer().SetEatCard(CardDrawHandler.Instance.GetDrawCardList()));

        m_isClosePointPopup = true;
    }

    public void GiveRightPlayerBtn()
    {
        StartCoroutine(GiveRightPlayer());
    }

    public void AchieveWish()
    {
        m_wishCardPopup.AchieveWish();
        m_wishCallPlayer.ClearWish();
    }

    public void CardExchangeBtn()
    {
        if (m_player.IsExchagne())
        {
            ChangeCardBtn();
            //StartCoroutine(CardExchange());
        }
    }

    public IEnumerator CardExchange()
    {
        //if (!m_isAllReady)
        //{
        //    yield break;
        //}

        while (!m_isAllReady)
        {
            yield return null;
        }

        //카드를 서로 교환한다.
        //for (int i = 0; i < playerSize; ++i)
        //{
        //    yield return StartCoroutine(m_players[i].CardExchange());
        //}

        yield return StartCoroutine(m_player.CardExchange());

        m_players[0].ViewExchangeCard();

        //카드를 정리한다.
        for (int i = 0; i < playerSize; ++i)
        {
            m_players[i].clearExchangeData();
        }

        //더미 소스
        m_isExchange = true;
        m_isAllReady = false;

        if (m_players[0].CheckBomb())
        {
            m_bombBtn.Active();
        }
        else
        {
            m_bombBtn.Inactive();
        }
    }

    IEnumerator SecondDivision()
    {
        //int size = m_cardDeck.GetDeckSize();
        //for (; count < size; ++count)
        //{
        //    CardData cardData = m_cardDeck.GetCard(count);

        //    m_players[count % 4].AddCard(m_cardDeck.GetCard(count));
        //    cardData.SetOwnerPlayer(m_players[count % 4]);
        //}

        //for (int i = 0; i < 4; ++i)
        //{
        //    yield return StartCoroutine(m_players[i].DivisionCardMove(m_cardDeck.transform.position, false));
        //    m_players[i].ArrangementCard();
        //    m_players[i].SetOriginTransform();
        //    m_players[i].SetCardCount();
        //}

        yield return StartCoroutine(m_player.DivisionCardMove(m_cardDeck.transform.position, false));
        m_player.ArrangementCard();
        m_player.SetOriginTransform();
        m_player.SetCardCount();

        if (m_player.CheckBomb())
        {
            m_bombBtn.Active();
        }
        else
        {
            m_bombBtn.Inactive();
        }
    }

    public void SetTopPlayer(Player player)
    {
        m_topPlayer = player;
    }

    public void SetActivePlayBtn(bool isActive)
    {
        m_drawBtn.gameObject.SetActive(isActive);
        m_passBtn.gameObject.SetActive(!isActive);
    }

    public void PassBtn()
    {
        m_turnPlayer.Pass();
    }

    public void DrawBomb()
    {
        if (m_players[0].DrawBomb())
        {
            for (int i = 0; i < playerSize; ++i)
            {
                m_players[i].GetTurnImage().gameObject.SetActive(false);
            }

            m_players[0].MyTurn();
            m_turnPlayer = m_players[0];
            m_turnPlayer.Pass();
        }
    }

    //패스를 다시 구상해보자.
    public IEnumerator Pass()
    {
        if (m_turnPlayer == null)
            yield break;

        //아무래도 참새가 이쪽에 있는건 좀 아닌거 같지만 일단 ㄱㄱㄱ
        if (!m_turnPlayer.NonPlayer && m_turnPlayer.GetCardCount() == 0)
        {
            //이건 내면서 0이 된거다.
            //이제 이녀석은 제외를 하자
            m_turnPlayer.NonPlayer = true;

            //카드가 다 줄었다. 1등인가 체크를 해보자.
            if (m_firstPlayer == null)
            {
                //first가 없다 즉 1등이다.
                m_firstPlayer = m_turnPlayer;
                m_firstPlayer.SuccesFirst(true);
            }
            else
            {
                m_turnPlayer.SuccesFirst(false);
            }

            //EndProcess전에 카드를 먹어줘야 된다.
            //이녀석이 나면 끝나는 상황이면 그 플레이어에게 중앙에 있는 카드를 먹여줘야 된다.
            if (CheckEndGame())
            {
                if (CardDrawHandler.Instance.HaveDragon() && CardDrawHandler.Instance.GetDrawCardType() == DRAWCARD_TYPE.SINGLE)
                {
                    //선택한 결과에 따라 다른 플레이어가 먹도록 해야 된다.
                    //코루틴을 부르자.
                    //Player인 경우 창을 띄우고 컴퓨터는 랜덤으로 선택하도록 하자.
                    yield return StartCoroutine(WaitWhoEatDragonCard());
                }
                else
                {
                    //용이 없으면 내가 먹자.
                    yield return StartCoroutine(m_turnPlayer.SetEatCard(CardDrawHandler.Instance.GetDrawCardList()));
                }
            }

            if (EndProcess())
            {
                yield break;
            }
        }

        //이번턴에 참새가 나왔으면 참새 확인창 소환
        if (m_turnPlayer != null && m_turnPlayer.DrawMajjong())
        {
            yield return StartCoroutine(WaitWishSelect());
        }

        //개가 나왔는지 확인을 하자.
        if (CardDrawHandler.Instance.DrawDog())
        {
            //개가 나왔으면 모든게 종료 턴을 내 파트너에게 넘긴다.
            //파트너가 카드가 없으면? 옆사람에게 가도록 정리를
            m_turnPlayer = m_turnPlayer.GetTemaPlayer();
            m_topPlayer = m_turnPlayer;
        }
        else
        {
            //개가 안 나왔으면 정상 루틴
            //옆 사람에게 턴을 넘긴다.
            //옆 사람이 카드가 0인 논플레이어다.
            //다시 한번 더 넘긴다.
            //만약 선이면 한턴은 받자..
            do
            {
                m_turnPlayer = m_turnPlayer.GetLeftPlayer();
            } while (m_turnPlayer != m_topPlayer /*&& m_turnPlayer != m_startPlayer */&& m_turnPlayer.NonPlayer);
        }

        //선으로부터 한바퀴 돌았다. 뎁스를 쌓아두자.
        //중앙에 카드가 이쁘게 쌓이도록 필요한 변수
        if (m_startPlayer == m_turnPlayer)
        {
            ++m_nRoundTurn;
        }

        //턴과 탑플레이어가 같다. 즉 카드를 먹을 수 있다.
        if (m_turnPlayer == m_topPlayer)
        {
            if (CardDrawHandler.Instance.HaveDragon() && CardDrawHandler.Instance.GetDrawCardType() == DRAWCARD_TYPE.SINGLE)
            {
                //선택한 결과에 따라 다른 플레이어가 먹도록 해야 된다.
                //코루틴을 부르자.
                //Player인 경우 창을 띄우고 컴퓨터는 랜덤으로 선택하도록 하자.
                yield return StartCoroutine(WaitWhoEatDragonCard());
            }
            else
            {
                //용이 없으면 내가 먹자.
                //m_turnPlayer.SetEatCard(CardDrawHandler.Instance.GetDrawCardList());
                yield return StartCoroutine(m_turnPlayer.SetEatCard(CardDrawHandler.Instance.GetDrawCardList()));
            }

            //카드를 먹었기 때문에 뎁스를 초기화
            m_nRoundTurn = 1;

            Debug.Log(m_turnPlayer.GetPoint());
            CardDrawHandler.Instance.initCardDraw();

            //먹은 사람으 논플레이어 즉 카드가 없는 사람이다.
            //그럼 종료처리를 하고 턴을 넘기자.
            if (m_turnPlayer.NonPlayer)
            {
                while(m_turnPlayer.NonPlayer)
                {
                    m_turnPlayer = m_turnPlayer.GetLeftPlayer();
                }
            }

            if (m_turnPlayer == m_players[0])
            {
                m_passBtn.View();
            }

            m_turnPlayer.MyTurn();
            m_startPlayer = m_turnPlayer;
        }
        else
        {
            //다른 플레이어 차례다
            m_turnPlayer.MyTurn();

            if (m_turnPlayer != m_players[0])
            {
                m_passBtn.Inactive();
            }
        }
    }

    public bool EndProcess()
    {
        bool result = false;

        //여기까진 턴플레이어가 안 바뀜
        //즉 턴플레이어가 종료된 플레이어

        //일단 종료된 값을 올린다.
        ++m_teamFlag[m_turnPlayer.TeamID];

        //팀플래그가 2가 되면 같은 팀이 먼저 났는지 확인을 해봐야 된다.
        if (m_teamFlag[0] - m_teamFlag[1] == 2)
        {
            //스코어 보드에 점수를 더해준다.
            int teamAAddPoint = 200 + m_players[0].GetBonusPoint() + m_players[2].GetBonusPoint();

            m_players[1].SuccesFirst(false);
            m_players[3].SuccesFirst(false);

            //A팀이 먼저 2명이 났음
            m_nTeamAPoint = m_nTeamAPoint + 200 + m_players[0].GetBonusPoint() + m_players[2].GetBonusPoint();
            m_nTeamBPoint = m_nTeamBPoint + m_players[1].GetBonusPoint() + m_players[3].GetBonusPoint();

            m_scoreBoardPopup.AddPoint(teamAAddPoint, m_nTeamAPoint, 0, m_nTeamBPoint);

            m_endRound = true;
            result = true;
        }
        else if (m_teamFlag[1] - m_teamFlag[0] == 2)
        {
            //스코어 보드에 점수를 더해준다.
            int teamBAddPoint = 200 + m_players[0].GetBonusPoint() + m_players[2].GetBonusPoint();

            m_players[0].SuccesFirst(false);
            m_players[2].SuccesFirst(false);

            //B팀이 먼저 2명이 났음
            m_nTeamBPoint = m_nTeamBPoint + 200 + m_players[1].GetBonusPoint() + m_players[3].GetBonusPoint();
            m_nTeamAPoint = m_nTeamAPoint + m_players[0].GetBonusPoint() + m_players[2].GetBonusPoint();

            m_scoreBoardPopup.AddPoint(0, m_nTeamAPoint, teamBAddPoint, m_nTeamBPoint);

            m_endRound = true;
            result = true;
        }
        else if (m_teamFlag[0] + m_teamFlag[1] == 3)
        {
            //일단 한팀이 2명이 난게 아니다.
            //그럼 한명만 남은 상황인지 확인을 해보자.

            //이렇게 되면 남은 플레이어는 1등에게 자기가 먹은 점수를
            //자기 손패는 다른 팀에게 점수를 줘야 된다.
            //일단 이번턴 카드는 현재 플레이어에게 먹어줘야 된다.
            int teamAAddPoint = 0;
            int teamBAddPoint = 0;

            for (int i = 0; i < playerSize; ++i)
            {
                if (m_players[i].GetCardCount() > 0)
                {
                    //이놈이 꼴지다.
                    //1등에서 점수를 주고
                    //티츄 성공 여부를 확인해주자,.
                    m_players[i].SuccesFirst(false);
                    m_firstPlayer.GivePoint(m_players[i].GetPoint());
                    m_players[i].SetPoint(0);
                    
                    switch (m_players[i].TeamID)
                    {
                        case 0:
                            {
                                teamBAddPoint = m_players[i].CalcHandCardPoint();
                            }
                            break;
                        case 1:
                            {
                                teamAAddPoint = m_players[i].CalcHandCardPoint();
                            }
                            break;
                    }

                    break;
                }
            }

            teamAAddPoint += m_players[0].GetPoint() + m_players[0].GetBonusPoint()
                + m_players[2].GetPoint() + m_players[2].GetBonusPoint();

            teamBAddPoint += m_players[1].GetPoint() + m_players[1].GetBonusPoint()
                + m_players[3].GetPoint() + m_players[3].GetBonusPoint();

            //점수 계산을 하자.
            m_nTeamAPoint += teamAAddPoint;
            m_nTeamBPoint += teamBAddPoint;

            m_scoreBoardPopup.AddPoint(teamAAddPoint, m_nTeamAPoint, teamBAddPoint, m_nTeamBPoint);

            //라운드 종료 처리
            m_endRound = true;

            result = true;
        }
        else
        {
            result = false;
        }

        if (result)
        {
            //ScoreBoard에 티츄 성공여부 정보를 보내자.
            for (int i = 0; i < playerSize; ++i)
            {
                m_scoreBoardPopup.LastScoreItem().SetTichuInfo(m_players[i].IsCallTichu(), m_players[i].IsCallLargeTichu(),
                    m_players[i] == m_firstPlayer, i);
            }
        }
        
        //그 이외에 종료조건이 아니다. 그냥 나가자?
        return result;

    }

    public bool CheckEndGame()
    {
        int teamID = m_turnPlayer.TeamID;

        if (teamID == 0)
        {
            if ((m_teamFlag[teamID] + 1 + m_teamFlag[1]) == 3)
            {
                return true;
            }
        }
        else if (teamID == 1)
        {
            if ((m_teamFlag[teamID] + 1 + m_teamFlag[0]) == 3)
            {
                return true;
            }
        }

        return false;
    }

    #region 네트워크 요청 함수

    //네트워크 함수들?
    public void CreateRoomBtn()
    {
        CreateRoomReq req = new CreateRoomReq();
        req.protocolType = (int)Request.CreateRoom;
        Network.Instance.SendMessage(req);
    }

    #region 방 입장 함수
    public void JoinRoomBtn()
    {
        //리퀘스트 함수 선언
        JoinRoomReq req = new JoinRoomReq();
        req.protocolType = (int)Request.JoinRoom;
        //방제가 들어가야 된다.
        req.roomCode = m_roomCode;
        Network.Instance.SendMessage(req);
    }
    #endregion

    #region 콜 라지티츄 함수
    public void CallLargeTichuBtn(bool isCall)
    {
        m_LargeTichuPopup.gameObject.SetActive(false);

        CallLargeTichuReq req = new CallLargeTichuReq();
        req.isCall = (isCall)? 1: 0;
        Network.Instance.SendMessage(req);
    }
    #endregion

    #region 콜 티츄 함수
    public void CallTichuBtn()
    {
        CallTichuReq req = new CallTichuReq();
        Network.Instance.SendMessage(req);
    }
    #endregion

    #region 카드 교환 함수
    public void ChangeCardBtn()
    {
        ChangeCardReq req = new ChangeCardReq();

        req.change.Add(m_player.GetLeftPlayer().PlayerIdx, m_player.GetCardExchangeData(0).GetOwnerPlayerCardIdx());
        req.change.Add(m_player.GetTemaPlayer().PlayerIdx, m_player.GetCardExchangeData(1).GetOwnerPlayerCardIdx());
        req.change.Add(m_player.GetRightPlayer().PlayerIdx, m_player.GetCardExchangeData(2).GetOwnerPlayerCardIdx());
        Network.Instance.SendMessage(req);
    }
    #endregion

    #region 폭탄 사용 버튼
    //public void UseBoomBtn()
    //{
    //    UseBoomReq req = new UseBoomReq();
    //    req.Add(cardIdx);
    //    req.Add(cardIdx);
    //    req.Add(cardIdx);
    //    req.Add(cardIdx);
    //    req.Add(cardIdx);
    //    Network.Instance.SendMessage(req);
    //}
    #endregion

    #endregion


    #region 네트워크 응답 함수

    #region 방 생성 응답 함수
    public void CreateRoomRes(Protocol.CreateRoomResp res)
    {
        string roomCode = res.roomCode;
        m_roomCode = roomCode;

        m_wiatUserPopup.gameObject.SetActive(true);
        m_wiatUserPopup.SetRoomCode(roomCode);
    }
    #endregion

    #region 방 입장 응답 함수
    public void JoinRoomRes(Protocol.JoinRoomResp res)
    {
        int userCount = res.userCount;

        m_wiatUserPopup.gameObject.SetActive(true);
        m_wiatUserPopup.SetRoomCode(m_roomCode);
        m_wiatUserPopup.SetUserCount(userCount);
    }
    #endregion

    #region 게임 시작 응답 함수
    //2차 분배 이후에 게임이 시작된다.
    //4인이 다 교환을 눌리면 해당 패킷이 온다.
    public void StartGameRes(Protocol.StartGameResp res)
    {
        //res.player.
        //playerSize = res.CurrentActivePlayer;
        //m_players = res.models.player;

        //StartCoroutine(TichuRoutine());
    }
    #endregion

    #region 1차 분배 응답 함수
    //게임 시작이 되면 바로 1차 분배를 한다.?
    public void RoomInitRes(Protocol.RoomInitResp res)
    {
        m_wiatUserPopup.gameObject.SetActive(false);
        m_CreateRoomPopup.gameObject.SetActive(false);
        m_JoinRoomPopup.gameObject.SetActive(false);

        //m_players[0].SetPlayers(ref m_players[1], ref m_players[2], ref m_players[3]);
        //m_players[1].SetPlayers(ref m_players[2], ref m_players[3], ref m_players[0]);
        //m_players[2].SetPlayers(ref m_players[3], ref m_players[0], ref m_players[1]);
        //m_players[3].SetPlayers(ref m_players[0], ref m_players[1], ref m_players[2]);

        //난 무조건 player 0가 되어야 된다.
        m_players[0].TeamID = res.team.teamNumber;
        m_players[0].PlayerIdx = res.player.index;

        m_player = m_players[0];

        //숫자에 따라서 playerIndex를 지정해주자.
        switch(res.player.index)
        {
            case 0:
                m_players[1].TeamID = 1;
                m_players[2].TeamID = 0;
                m_players[3].TeamID = 1;
                m_players[1].PlayerIdx = 1;
                m_players[2].PlayerIdx = 2;
                m_players[3].PlayerIdx = 3;
                break;
            case 1:
                m_players[1].TeamID = 0;
                m_players[2].TeamID = 1;
                m_players[3].TeamID = 0;
                m_players[1].PlayerIdx = 2;
                m_players[2].PlayerIdx = 3;
                m_players[3].PlayerIdx = 0;
                break;
            case 2:
                m_players[1].TeamID = 1;
                m_players[2].TeamID = 0;
                m_players[3].TeamID = 1;
                m_players[1].PlayerIdx = 3;
                m_players[2].PlayerIdx = 0;
                m_players[3].PlayerIdx = 1;
                break;
            case 3:
                m_players[1].TeamID = 0;
                m_players[2].TeamID = 1;
                m_players[3].TeamID = 0;
                m_players[1].PlayerIdx = 0;
                m_players[2].PlayerIdx = 1;
                m_players[3].PlayerIdx = 2;
                break;
        }

        if (m_player.TeamID == 0)
        {
            //0이면 A
            //1이면 B로 하자
            m_nTeamAPoint = res.team.TotalScore;
        }
        else
        {
            m_nTeamBPoint = res.team.TotalScore;
        }

        m_player.PlayerIdx = res.player.index;

        for (int i = 0; i < res.player.CardList.Count; ++i){
            m_player.AddCard(m_cardDeck.GetCard((CARD_TYPE)res.player.CardList[i].m_eCardType,
                (CARD_COLOR)res.player.CardList[i].m_eCardColor,
                res.player.CardList[i].m_nCardValue));
        }

        m_players[1].SetCardCount(8);
        m_players[2].SetCardCount(8);
        m_players[3].SetCardCount(8);

        StartCoroutine(TichuRoutine());
        //StartCoroutine(ChooseLargeTichu());
    }
    #endregion

    #region 2차 분배 응답 함수
    public void DistributeAllCardRes(Protocol.DistributeAllCardResp res)
    {
        if (m_player.PlayerIdx == res.player.index)
        {
            for (int i = 8; i < res.player.CardList.Count; ++i)
            {
                m_player.AddCard(m_cardDeck.GetCard((CARD_TYPE)res.player.CardList[i].m_eCardType,
                    (CARD_COLOR)res.player.CardList[i].m_eCardColor,
                    res.player.CardList[i].m_nCardValue));
            }
        }
        m_rutineState = RutineState.SecondDivisionState;

        //교환 단계로 넘어가자.
        StartCoroutine(SecondDivision());

        m_rutineState = RutineState.ExchangeState;

        m_ExchangeUi.View();
        m_exchangeBtn.gameObject.SetActive(true);
        m_drawBtn.Hide();
        m_passBtn.Hide();
    }
    #endregion

    #region 콜 라지 티츄 응답 함수
    public void CallLargeTichuRes(Protocol.CallLargeTichuResp res)
    {
        for (int i = 0; i < playerSize; ++i)
        {
            if (res.callTichu.ContainsKey(m_players[i].PlayerIdx))
            {
                m_players[i].SetTichu(res.callTichu[m_players[i].PlayerIdx]);
                break;
            }
        }

        //2차 분배?
        //서버에서 올꺼다.
    }
    #endregion

    #region
    public void CallTichuRes(Protocol.CallTichuResp res)
    {
        for (int i = 0; i < playerSize; ++i)
        {
            if (res.callTichu.ContainsKey(m_players[i].PlayerIdx))
            {
                m_players[i].SetTichu(res.callTichu[m_players[i].PlayerIdx]);
                break;
            }
        }
    }
    #endregion

    #endregion
}
