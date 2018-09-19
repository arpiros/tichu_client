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

    public class CreateRoomResp : BaseResp
    {
        public string roomCode;
    }

    public class JoinRoomResp : BaseResp
    {
        public int userCount;
    }

    public class RoomInitResp : BaseResp
    {
        public Team team;
        public Player player;

    }

    public class CallLargeTichuResp : BaseResp
    {
        public Dictionary<int, int> callTichu;
    }

    public class DistributeAllCardResp : BaseResp
    {
        public Player player;
        public Dictionary<int, int> callTichu;
    }

    public class StartGameResp : BaseResp
    {
        public Player player;
        public int currentActivePlayer;
    }

    public class CallTichuResp : BaseResp
    {
        public Dictionary<int, int> callTichu;
    }




    public class Team
    {
        public int teamNumber;
        public int TotalScore;
    }

    public class Player
    {
        public int index;
        public int teamNumber;
        public List<Card> CardList;
        public bool isMyTurn;
    }
}

