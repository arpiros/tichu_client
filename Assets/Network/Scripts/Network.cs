using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using WebSocketSharp;

public class Network : MonoBehaviour {
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

    public bool IsConnect
    {
        get; 
        private set;
    } 
    
    // Use this for initialization
    void Start () 
    {    
    }
    
    // Update is called once per frame
    void Update()
    {
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

    private static void NetworkOpen(object sender, EventArgs e)
    {
        
    }
    
    private static void NetworkOnMessage(object sender, MessageEventArgs e)
    {
        var resp = JsonConvert.DeserializeObject<Protocol.BaseResp>(e.Data);

        switch ((Protocol.Response)resp.ResponseType)
        {
            case Protocol.Response.CreateRoom:
                break;
            case Protocol.Response.JoinRoom:
                break;
            case Protocol.Response.RoomInit:
                break;
            case Protocol.Response.CallLargeTichu:
                break;
            case Protocol.Response.DistributeAllCard:
                break;
            case Protocol.Response.StartGame:
                break;
            case Protocol.Response.CallTichu:
                break;
                
            default:
                Debug.Log(e.Data);
                break;
        }
    }

    private static void NetworkOnError(object sender, ErrorEventArgs e)
    {
        //TODO error 
    }

    private static void NetworkOnClose(object sender, CloseEventArgs e)
    {
        //TODO error 
    }

}
