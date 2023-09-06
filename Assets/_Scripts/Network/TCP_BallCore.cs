using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Events;

public class TCP_BallCore : MonoBehaviour
{
    // Set ip and port.
    [SerializeField]
    private TextOrHint ipInput;
    [SerializeField]
    private TextOrHint portInput;
    [SerializeField]
    private TextOrHint idInput;

    // Message field, client and server will use to show something.
    [SerializeField]
    private TCP_BallUI ui;

    // Event handler
    [SerializeField]
    private TCP_BallCommand eventHandle;

    private IEnumerator connecting;


    public static TCP_BallCore instance;
    public static Action<string> messageEvent;
    public static Action<string> errorEvent;
    public static NetworkMode networkMode = NetworkMode.None;


    // Using for start server or client.
    private static TCP_BallServer server;
    private static TCP_BallClient client;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Only one TCP Core can available!");
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        errorEvent = ui.ErrorHandle;
        messageEvent = ui.Message;
    }

    // Update is called once per frame
    void Update()
    {
        if (TCP_BallClient.ready)
        {
            client.Update();
        }
        if (true)
        {

        }
    }

    public static bool CheckInstanceNull()
    {
        if (instance == null)
        {
            Debug.LogError("TCP Core instance doesn't exist!");
            return true;
        }

        return false;
    }

    public static bool AttemptToOpenServer()
    {
        instance.ui.ResetText();
        int port;
        if (CheckInstanceNull())
        {
            return false;
        }
        if (!int.TryParse(instance.portInput.text, out port))
        {
            messageEvent.Invoke("Port error!");
            instance.ui.ConnectFail();
            return false;
        }
        server = TCP_BallServer.OpenServer(instance.ipInput.text, port, instance.idInput.text);
        if (server == null)
        {
            instance.ui.ConnectFail();
            return false;
        }
        else
        {
            AttemptToConnectServer();
        }

        return true;
    }

    public static bool AttemptToConnectServer()
    {
        if(networkMode != NetworkMode.Server)
        {
            networkMode = NetworkMode.Client;
        }

        instance.ui.ResetText();
        int port;
        if (CheckInstanceNull())
        {
            return false;
        }
        if(!int.TryParse(instance.portInput.text, out port))
        {
            messageEvent.Invoke("Port error!");
            instance.ui.ConnectFail();
            return false;
        }
        messageEvent.Invoke("Connecting...");
        client = TCP_BallClient.ConnectToServer(instance.ipInput.text, port, instance.idInput.text);
        if (client == null)
        {
            instance.ui.ConnectFail();
            return false;
        }
        instance.connecting = instance.ConnectingServer();
        instance.StartCoroutine(instance.connecting);

        return true;
    }
    private IEnumerator ConnectingServer()
    {
        while (client == null && TCP_BallClient.ready)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Connect fail
        if (client == null)
        {
            messageEvent.Invoke("Connect failed");
            if(networkMode == NetworkMode.Server)
            {
                CloseServer();
            }
            instance.ui.ConnectFail();
        }
        // Connect success
        else if (TCP_BallClient.ready)
        {
            ui.GoToRoom();
        }

        StopCoroutine(connecting);
        connecting = null;
        yield return null;
    }

    public void SetNetworkServer()
    {
        networkMode = NetworkMode.Server;
    }
    public void SetNetworkClient()
    {
        networkMode = NetworkMode.Client;
    }
    public void NetworkModeReset()
    {
        networkMode = NetworkMode.None;
    }

    public static void CloseServer()
    {
        if (server != null)
        {
            server.CloseServer();
        }
    }
}
