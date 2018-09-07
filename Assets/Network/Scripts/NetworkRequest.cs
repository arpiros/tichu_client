using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HTTPCallback(string respData);

public class NetworkRequest {
    public ServerCommands command;
    public WWWForm form;
    public HTTPCallback callback;
}
