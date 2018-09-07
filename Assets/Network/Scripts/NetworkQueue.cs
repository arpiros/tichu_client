using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkQueue : MonoBehaviour {
    #region sigleton & lifecycle
    private static NetworkQueue s_Instance = null;
    public static NetworkQueue Instance
    {
        get
        {
            if (s_Instance == null)
            {
                GameObject go = new GameObject("_NetworkQueue");
                s_Instance = go.AddComponent<NetworkQueue>();

                DontDestroyOnLoad(go);
            }
            return s_Instance;
        }
    }
    #endregion

    Queue<NetworkRequest> queue = new Queue<NetworkRequest>();
    private bool mRunningQueue = false;
    private bool pendingProcess = true;

    // Use this for initialization
    void Start () {
        mRunningQueue = true;
        StartCoroutine(QueueWorker());
    }
	
	// Update is called once per frame
	void Update () {
        if (!mRunningQueue)
        {
            mRunningQueue = true;
            StartCoroutine(QueueWorker());
        }
	}

    public void ClearAll()
    {
        pendingProcess = true;
        queue.Clear();
    }

    public void Request(ServerCommands cmd, HTTPCallback cb)
    {
        NetworkRequest req = new NetworkRequest
        {
            command = cmd,
            callback = cb,
            form = null
        };
        queue.Enqueue(req);
    }

    IEnumerator QueueWorker()
    {
        while(true)
        {
            if (queue.Count > 0 && pendingProcess)
            {
                NetworkRequest req = queue.Peek();
                string url = ServerRoutes.GetPathURL(req.command);

                using (WWW www = new WWW(url, req.form))
                {
                    yield return www;

                    if (www.error == null)
                    {
                        req.callback(www.text);
                    }
                    else
                    {
                        Debug.Log(www.error);
                    }
                }

                if (queue.Count > 0)
                {
                    queue.Dequeue();
                }
                pendingProcess = true;
            }
            yield return null;
        }
    }
}
