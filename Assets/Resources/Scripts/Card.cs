using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    eNONE = 0,
    eSPARROW = 1,
    eDOG = 2,
    eDRAGON = 3,
    ePHOENIX = 4,
}

public enum CardColor
{
    eNONE = 0,
    eRED = 1,
    eBLUE = 2,
    eGREEN = 3,
    eBLACK = 4,
}

public class Card : MonoBehaviour {

    public bool bIsObverse;
    bool bIsSelect;

    public Image m_obverseImage;
    public Image m_backImage;

    int m_nCardValue;
    CardType m_eCardType;
    CardColor m_eCardColor;

    public Card()
    {
    }

    public Card(CardType eType, CardColor eColor, int nValue)
    {
        m_eCardType = eType;
        m_eCardColor = eColor;
        m_nCardValue = nValue;
    }

	// Use this for initialization
	void Start () {
		
	}
	
}
