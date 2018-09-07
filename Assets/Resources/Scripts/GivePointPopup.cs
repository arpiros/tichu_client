using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GivePointPopup : MonoBehaviour {

    public Text m_pointText;
    public Text m_leftPlayerName;
    public Text m_rightPlayerName;
    public Button m_leftPlayerBtn;
    public Button m_rightPlayerBtn;

    public void SetInfo(string leftName, string rightName)
    {
        m_leftPlayerName.text = leftName;
        m_rightPlayerName.text = rightName;

        SetPoint();
    }

    public void SetPoint()
    {
        int point = CardDrawHandler.Instance.TotalPoint();

        if (point >= 0)
        {
            m_pointText.color = new Color(0, 0, 0);
        }
        else
        {
            m_pointText.color = new Color(255, 0, 0);
        }

        m_pointText.text = point.ToString();
    }

}
