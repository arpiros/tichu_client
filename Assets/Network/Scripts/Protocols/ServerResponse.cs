using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Protocol
{
    public enum Response
    {
        
    }
    public class BaseResp
    {
        [JsonProperty("rt")]
        public int ResponseType;
    }
}

