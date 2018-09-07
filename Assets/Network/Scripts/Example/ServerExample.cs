using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class ServerExample : MonoBehaviour
{

    private void Start()
    {
    }

    public void OnAbout()
    {
        ServerAPI.About(AboutResp);
    }

    void AboutResp(BaseResp resp)
    {
        Debug.Log("Callback Ok");
    }
}