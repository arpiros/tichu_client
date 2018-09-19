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
		public CreateRoomReq()
		{
			protocolType = (int)Protocol.Request.CreateRoom;
		}
	}

	public class JoinRoomReq : BaseReq
	{
		public string roomCode;
		public JoinRoomReq()
		{
			protocolType = (int)Protocol.Request.JoinRoom;
		}
	}

	public class CallLargeTichuReq : BaseReq
	{
		public int isCall;

		public CallLargeTichuReq()
		{
			protocolType = (int)Protocol.Request.CallLargeTichu;
		}
	}

	public class ChangeCardReq : BaseReq
	{
		public Dictionary<int, int> change;
		public ChangeCardReq()
		{
			protocolType = (int)Protocol.Request.ChangeCard;
		}
	}

	public class CallTichuReq : BaseReq
	{
		public CallTichuReq()
		{
			protocolType = (int)Protocol.Request.CallTichu;
		}
	}
	
	public class UseBoomReq : BaseReq
	{
		public List<int> cards;
		
		public UseBoomReq()
		{
			protocolType = (int)Protocol.Request.Boom;
		}
	}
}

