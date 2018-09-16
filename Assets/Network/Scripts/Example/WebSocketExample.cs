using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using WebSocketSharp;

public class WebSocketExample : MonoBehaviour
{
    public void OnClickConnect()
    {
        Network.Instance.Connect();
    }

    public void OnClickCreateRoom()
    {
        CreateRoomReq req = new CreateRoomReq();
        req.protocolType = (int)Request.CreateRoom;
        Network.Instance.SendMessage(req);
    }
}