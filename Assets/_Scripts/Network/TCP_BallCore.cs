using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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


    public static TCP_BallCore instance
    {
        get { return _instance; }
    }
    public static Action<string> messageEvent
    {
        get { return _messageEvent; }
    }
    public static Action<string> errorEvent
    {
        get { return _errorEvent; }
    }
    public static NetworkMode networkMode
    {
        get { return _networkMode; }
    }


    // Using for start server or client.
    private static TCP_BallServer server;
    private static TCP_BallClient client;
    private static NetworkMode _networkMode = NetworkMode.None;
    private static TCP_BallCore _instance;
    private static Action<string> _messageEvent;
    private static Action<string> _errorEvent;


    private void Awake()
    {
        if (instance == null)
        {
            _instance = this;
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
        _errorEvent = ui.ErrorHandle;
        _messageEvent = ui.Message;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (TCP_BallClient.ready)
        {
            client.Update();
        }
        if (TCP_BallServer.started)
        {
            server.Update();
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
        server = TCP_BallServer.OpenServer(instance.ipInput.text, port);
        if (server == null)
        {
            instance.ui.ConnectFail();
            return false;
        }

        return AttemptToConnectServer();
    }

    public static bool AttemptToConnectServer()
    {
        if(_networkMode != NetworkMode.Server)
        {
            _networkMode = NetworkMode.Client;
        }

        instance.ui.ResetText();
        int port;
        if (CheckInstanceNull())
        {
            return false;
        }
        if(!int.TryParse(instance.portInput.text, out port))
        {
            if (networkMode == NetworkMode.Server)
            {
                CloseServer();
            }
            messageEvent.Invoke("Port error!");
            instance.ui.ConnectFail();
            return false;
        }
        messageEvent.Invoke("Connecting...");
        client = TCP_BallClient.ConnectToServer(instance.ipInput.text, port, instance.idInput.text);
        if (client == null)
        {
            if(networkMode == NetworkMode.Server)
            {
                CloseServer();
            }
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
            if(_networkMode == NetworkMode.Server)
            {
                CloseServer();
            }
            instance.ui.ConnectFail();
        }
        // Connect success, setting id
        else if (TCP_BallClient.ready)
        {
            client.Send(TCP_BallCommand.ClientOnConnect(instance.idInput.text));
        }

        StopCoroutine(connecting);
        connecting = null;
        yield return null;
    }

    public void SetNetworkServer()
    {
        _networkMode = NetworkMode.Server;
    }
    public void SetNetworkClient()
    {
        _networkMode = NetworkMode.Client;
    }
    public void NetworkModeReset()
    {
        _networkMode = NetworkMode.None;
    }

    public static void CloseServer()
    {
        if (server != null)
        {
            server.CloseServer();
        }
    }
}
