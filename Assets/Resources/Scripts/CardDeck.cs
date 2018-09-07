using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDeck : MonoBehaviour {
    public CardData cardDataPrefab;

    public int x = -445;
    public int y = -809;
    public int xOffset = 100;
    public int yOffset = 200;

    private List<CardData> m_cardDeck = new List<CardData>();
    private CardData m_mahjong = null;

    string[] valueArray = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "새", "봉", "용", "개" };

    private void Awake()
    {
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 13; ++j)
            {
                CardData card = Instantiate(cardDataPrefab);
                card.transform.parent = transform;

                card.SetCard(CARD_TYPE.NONE, (CARD_COLOR)(i), valueArray[j]);
                //card.transform.position = new Vector3(x + (j * xOffset), -y + (i * yOffset) , 1);
                //card.transform.position = new Vector3(540, 960, 1);
                card.transform.position = transform.position;

                m_cardDeck.Add(card);
            }
        }

        //특수카드 삽입
        for (int i = 1; i < 5; ++i)
        {
            CardData specialCard = Instantiate(cardDataPrefab);
            specialCard.transform.parent = transform;

            specialCard.SetCard((CARD_TYPE)i, CARD_COLOR.NONE, valueArray[12 + i]);
            //specialCard.transform.position = new Vector3(540, 960, 1);
            specialCard.transform.position = transform.position;

            m_cardDeck.Add(specialCard);

            if (specialCard.type == CARD_TYPE.MAHJONG)
            {
                m_mahjong = specialCard;
            }
        }

        //셔플을 하자
        CardData temp;
        int size = m_cardDeck.Count;

        for (int i = 0; i < size; ++i)
        {
            int index1 = Random.Range(0, size);
            int index2 = Random.Range(0, size);

            temp = m_cardDeck[index1];
            m_cardDeck[index1] = m_cardDeck[index2];
            m_cardDeck[index2] = temp;
        }

        //임시 소스
        //봉황이 플레이어에게 가도록
        //int num = m_cardDeck.FindIndex(item => item.type == CARD_TYPE.PHOENIX);
        int num = m_cardDeck.FindIndex(item => item.type == CARD_TYPE.DRACHE);

        temp = m_cardDeck[0];
        m_cardDeck[0] = m_cardDeck[num];
        m_cardDeck[num] = temp;
    }

    // Use this for initialization
    void Start () {
        for (int i = 0; i < m_cardDeck.Count; ++i)
        {
            m_cardDeck[i].transform.SetSiblingIndex(0);
            m_cardDeck[i].Init();
        }
    }

    public void Init()
    {
        for (int i = 0; i < m_cardDeck.Count; ++i)
        {
            m_cardDeck[i].transform.SetSiblingIndex(0);
            m_cardDeck[i].transform.position = transform.position;
            m_cardDeck[i].Init();
        }

        //셔플을 하자
        CardData temp;
        int size = m_cardDeck.Count;

        for (int i = 0; i < size; ++i)
        {
            int index1 = Random.Range(0, size);
            int index2 = Random.Range(0, size);

            temp = m_cardDeck[index1];
            m_cardDeck[index1] = m_cardDeck[index2];
            m_cardDeck[index2] = temp;
        }
    }

    public CardData GetCard(int index)
    {
        return m_cardDeck[index];
    }

    public int GetDeckSize()
    {
        return m_cardDeck.Count;
    }

    public CardData GetmahjongCard()
    {
        return m_mahjong;
    }
}
