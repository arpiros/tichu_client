using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardItem : MonoBehaviour {
    public Text[] playerText = new Text[4];

    public Text teamAPointText;
    public Text teamAPrevPointText;
    public Text teamBPointText;
    public Text teamBPrevPointText;

    Color m_black = new Color(0, 0, 0, 1);
    Color m_gray = new Color(112 / 255.0f, 119 / 255.0f, 127 / 255.0f);
    Color m_red = new Color(248 / 255.0f, 84 / 255.0f, 84 / 255.0f);
    Color m_failedTichuColor = new Color(218 / 255.0f, 22 / 255.0f, 22 / 255.0f);

    public void SetPoint(int teamAPoint, int teamAPrevPoint, int teamBPoint, int teamBPrevPoint)
    {
        //이전 점수는 글자 색깔이 상관없으니 먼저 처리하자.
        teamAPrevPointText.text = teamAPrevPoint.ToString();
        teamBPrevPointText.text = teamBPrevPoint.ToString();

        string aSign = (teamAPoint >= 0) ? "+" : "-";
        string bSign = (teamBPoint >= 0) ? "+" : "-";

        teamAPointText.text = aSign + teamAPoint.ToString();
        teamBPointText.text = bSign + teamBPoint.ToString();

        teamAPointText.color = (teamAPoint >= 0) ? m_gray : m_red;
        teamBPointText.color = (teamBPoint >= 0) ? m_gray : m_red;
    }

    public void SetTichuInfo(bool isCallTichu, bool isCallLargeTichu, bool isSucceuss, int idx)
    {
        if (isCallLargeTichu)
        {
            playerText[idx].text = "GT";
        }
        else if (isCallTichu)
        {
            playerText[idx].text = "T";
        }
        else
        {
            playerText[idx].text = "";
        }

        playerText[idx].color = (isSucceuss) ? m_black : m_failedTichuColor;
    }
}
