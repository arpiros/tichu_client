using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExchagneUI : MonoBehaviour {

    public GameObject m_exchangeUI;
    public Image m_leftCard;
    public Image m_partnerCard;
    public Image m_rightCard;

	public void Hide()
    {
        m_exchangeUI.SetActive(false);
    }

    public void View()
    {
        m_exchangeUI.SetActive(true);
    }
}
