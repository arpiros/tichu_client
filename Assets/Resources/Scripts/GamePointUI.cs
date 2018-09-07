using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePointUI : MonoBehaviour {

    public void OnMouseDown()
    {
        if (!GameManager.Instance.m_scoreBoardPopup.gameObject.activeSelf)
        {
            GameManager.Instance.m_scoreBoardPopup.gameObject.SetActive(true);
        }
    }
}
