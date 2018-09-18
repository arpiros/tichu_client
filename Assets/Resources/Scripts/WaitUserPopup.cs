using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitUserPopup : MonoBehaviour {

    public Text m_progresPlayerText;
    public Text m_roomCodeText;

    int m_maxPlayer = 4;

    public void SetRoomCode(string roomCode)
    {
        m_roomCodeText.text = roomCode;
    }

    public void SetUserCount(int userCount)
    {
        m_progresPlayerText.text = userCount.ToString() + " / " + m_maxPlayer.ToString(); 
    }

}
