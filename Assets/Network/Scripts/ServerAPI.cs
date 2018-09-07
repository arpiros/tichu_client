using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;

public partial class ServerAPI {

    static ServerAPI()
    {
        Reset();
    }

    public static void Reset()
    {
        NetworkQueue.Instance.ClearAll();
    }

    public static void About(Action<BaseResp> complete)
    {
        NetworkQueue.Instance.Request(ServerCommands.About, (string txt) =>
        {
            Debug.Log(txt);
            //var resp = Json
            callbackArg(complete, null);
        });
    }

    static void callbackArg<T>(Action<T> cb, T arg)
    {
        if (cb != null)
        {
            cb(arg);
        }
    }
}
