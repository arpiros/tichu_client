using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NetworkSettings", menuName = "NetworkSettings", order = 1)]
public class NetworkSettings : ScriptableObject
{
    public string LocalURL = "ws://localhost:5555";
    public string DevBaseURL = "ws://ec2-13-209-6-26.ap-northeast-2.compute.amazonaws.com:5555";

    public enum ServerTargets
    {
        LocalHost,
        DevServer,
        ReleaseServer,
    }

    public ServerTargets serverTarget = ServerTargets.DevServer;

    private static NetworkSettings s_Instance;
    public static NetworkSettings Instance
    {
        get
        {
            if(s_Instance == null)
            {
                s_Instance = (NetworkSettings)Resources.Load("NetworkSettings", typeof(NetworkSettings));
            }

            return s_Instance;
        }
    }

    NetworkSettings()
    {

    }

    public string GetBaseURL()
    {
        switch(serverTarget)
        {
            case (ServerTargets.LocalHost):
                return LocalURL;
            case (ServerTargets.DevServer):
                return DevBaseURL;
            default:
                throw new System.Exception("Unknown Server Targets " + serverTarget.ToString());
        }
    }

    public string GetServerURL()
    {
        string url = string.Empty;

        if (string.IsNullOrEmpty(url))
        {
            url = GetBaseURL();
        }

        if (url.Length > 0)
        {
            url = url.TrimEnd('/');
        }

        return url;
    }

    public string MakeURLString(string cmd)
    {
        return GetBaseURL() + cmd;
    }
}
