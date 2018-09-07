using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour {
    public ScoreBoardItem scoreBoardPrefab;
    public Transform Content;

    List<ScoreBoardItem> m_scoreBoardItemList = new List<ScoreBoardItem>();

    public void AddPoint(int teamAPoint, int teamAPrevPoint, int teamBPoint, int teamBPrevPoint)
    {
        ScoreBoardItem scoreItem = Instantiate(scoreBoardPrefab);
        scoreItem.SetPoint(teamAPoint, teamAPrevPoint, teamBPoint, teamBPrevPoint);
        m_scoreBoardItemList.Add(scoreItem);
        scoreItem.transform.SetParent(Content);
    }

    public ScoreBoardItem LastScoreItem()
    {
        return m_scoreBoardItemList[m_scoreBoardItemList.Count - 1];
    }

    public void CloseScorePopup()
    {
        if (GameManager.Instance.m_scoreBoardPopup.gameObject.activeSelf)
        {
            GameManager.Instance.m_scoreBoardPopup.gameObject.SetActive(false);
        }
    }
}
