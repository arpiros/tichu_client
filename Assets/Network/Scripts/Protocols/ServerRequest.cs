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
		[JsonProperty("pt")]
		public int protocolType;
	}

	public class CreateRoomReq : BaseReq
	{
		
	}
}

