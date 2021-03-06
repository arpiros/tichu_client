﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Protocol;
using UnityEngine;
using WebSocketSharp;

public class Network : MonoBehaviour
{
    #region sigleton & lifecycle

    private static Network s_Instance = null;

    public static Network Instance
    {
        get
        {
            if (s_Instance == null)
            {
                GameObject go = new GameObject("_Network");
                s_Instance = go.AddComponent<Network>();

                DontDestroyOnLoad(go);
            }

            return s_Instance;
        }
    }

    #endregion

    private WebSocket ws;

    public bool IsConnect { get; private set; }

    private bool mRunningQueue = false;
    private bool isQuitting = false;
    Queue<string> responseQueue = new Queue<string>();

    // Use this for initialization
    void Start()
    {
        mRunningQueue = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (mRunningQueue && IsConnect)
        {
            StartCoroutine(ResponseQueueWorker());
        }
    }

    IEnumerator ResponseQueueWorker()
    {
        while (!isQuitting)
        {
            if (responseQueue.Count > 0)
            {
                string respStr = responseQueue.Peek();
                var resp = JsonConvert.DeserializeObject<Protocol.BaseResp>(respStr);
                switch ((Protocol.Response) resp.ResponseType)
                {
                    case Response.CreateRoom:
                        var createRoomResp = JsonConvert.DeserializeObject<Protocol.CreateRoomResp>(respStr);
                        GameManager.Instance.CreateRoomRes(createRoomResp);
                        break;
                    case Protocol.Response.JoinRoom:
                        var joinRoomResp = JsonConvert.DeserializeObject<Protocol.JoinRoomResp>(respStr);
                        GameManager.Instance.JoinRoomRes(joinRoomResp);
                        break;
                    case Protocol.Response.RoomInit:
                        var roomInitResp = JsonConvert.DeserializeObject<Protocol.RoomInitResp>(respStr);
                        GameManager.Instance.RoomInitRes(roomInitResp);
                        break;
                    case Protocol.Response.CallLargeTichu:
                        var callLargeTichuResp = JsonConvert.DeserializeObject<Protocol.CallLargeTichuResp>(respStr);
                        GameManager.Instance.CallLargeTichuRes(callLargeTichuResp);
                        break;
                    case Protocol.Response.DistributeAllCard:
                        var distirbuteAllCardResp = JsonConvert.DeserializeObject<Protocol.DistributeAllCardResp>(respStr);
                        GameManager.Instance.DistributeAllCardRes(distirbuteAllCardResp);
                        break;
                    case Protocol.Response.StartGame:
                        var startGameResp = JsonConvert.DeserializeObject<Protocol.StartGameResp>(respStr);
                        GameManager.Instance.StartGameRes(startGameResp);
                        break;
                    case Protocol.Response.CallTichu:
                        var callTichuResp = JsonConvert.DeserializeObject<Protocol.CallTichuResp>(respStr);
                        GameManager.Instance.CallTichuRes(callTichuResp);
                        break;

                    default:
                        break;
                }
            }

            if (responseQueue.Count > 0)
            {
                responseQueue.Dequeue();    
            }

            yield return null;
        }
    }

    public void Connect()
    {
        ws = new WebSocket(NetworkSettings.Instance.GetServerURL());
        ws.OnOpen += NetworkOpen;
        ws.OnMessage += NetworkOnMessage;
        ws.OnError += NetworkOnError;
        ws.OnClose += NetworkOnClose;

        ws.Connect();
        IsConnect = true;
    }

    public void SendMessage(object value)
    {
        ws.Send(JsonConvert.SerializeObject(value));
    }

    private void NetworkOpen(object sender, EventArgs e)
    {
    }

    private void NetworkOnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        responseQueue.Enqueue(e.Data);
    }

    private void NetworkOnError(object sender, ErrorEventArgs e)
    {
        //TODO error 
        Debug.LogError(e.Message);
    }

    private void NetworkOnClose(object sender, CloseEventArgs e)
    {
        //TODO error
        Debug.LogError(e.Reason);
    }
}