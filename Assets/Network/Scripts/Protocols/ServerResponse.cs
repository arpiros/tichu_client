using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Protocol
{
    public enum Response
    {
        CreateRoom = 0,
        JoinRoom,
        RoomInit,
        CallLargeTichu,
        DistributeAllCard,
        StartGame,
        CallTichu,
    }
    public class BaseResp
    {
        [JsonProperty("resp_t")]
        public int ResponseType;
    }
}

