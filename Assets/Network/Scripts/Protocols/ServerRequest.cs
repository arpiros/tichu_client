using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Protocol
{
	public enum Request
	{
		CreateRoom = 0,
		JoinRoom,
		CallLargeTichu,
		ChangeCard,
		CallTichu,
		Boom,
		MoveTurn,
		
	}
	public class BaseReq
	{
		[JsonProperty("req_t")]
		public int protocolType;
	}

	public class CreateRoomReq : BaseReq
	{
		
	}

	public class JoinRoomReq : BaseReq
	{
		public string roomCode;
	}

	public class CallLargeTichuReq : BaseReq
	{
		public int isCall;
	}

	public class ChangeCardReq : BaseReq
	{
		public Dictionary<int, int> change;
	}

	public class CallTichuReq : BaseReq
	{
		
	}
	
	public class UseBoomReq : BaseReq
	{
		public List<int> cards;
	}
}

