using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServerCommands
{
    About
}

public class ServerRoutes {
    private static Dictionary<ServerCommands, string> contextlMap = new Dictionary<ServerCommands, string>();

    static ServerRoutes()
    {
        RegisterAll();
    }

    public static void RegisterAll()
    {
        RegisterRequestMap(ServerCommands.About, "/about");
    }

    public static void RegisterRequestMap(ServerCommands commands, string url)
    {
        contextlMap[commands] = url;
    }

    public static string GetPathURL(ServerCommands cmd)
    {
        return NetworkSettings.Instance.MakeURLString(contextlMap[cmd]);
    }
}
