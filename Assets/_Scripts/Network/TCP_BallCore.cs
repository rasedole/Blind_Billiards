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
    private TCP_BallUI message;

    // Event handler
    [SerializeField]
    private TCP_BallCommand eventHandle;
    [SerializeField]
    private UnityEvent clientConnectEvent;


    private static TCP_BallCore instance;

    // Using for start server or client.
    private static TCP_BallServer server;
    private static TCP_BallClient client;


    [HideInInspector]
    public static Action<string> messageEvent;
    [HideInInspector]
    public static Action<string> errorEvent;
    [HideInInspector]
    public static Action<string> clientReceiveEvent;
    [HideInInspector]
    public static GameState gameState = GameState.None;
    [HideInInspector]
    public static NetworkMode networkMode = NetworkMode.None;


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
        errorEvent = message.ErrorHandle;
        messageEvent = message.Message;
        clientReceiveEvent = eventHandle.ClientReceiveEvent;
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
        instance.message.ResetText();
        if (CheckInstanceNull())
        {
            networkMode = NetworkMode.None;
            return false;
        }


        return true;
    }

    public static bool AttemptToConnectServer()
    {
        if(networkMode != NetworkMode.Server)
        {
            networkMode = NetworkMode.Client;
        }

        instance.message.ResetText();
        int port;
        if (CheckInstanceNull())
        {
            return false;
        }
        if(!int.TryParse(instance.portInput.text, out port))
        {
            messageEvent.Invoke("Port error!");
            networkMode = NetworkMode.None;
            return false;
        }
        client = TCP_BallClient.ConnectToServer(instance.ipInput.text, port, instance.idInput.text);
        if (client == null)
        {
            networkMode = NetworkMode.None;
            return false;
        }
        messageEvent.Invoke("Connecting...");

        return true;
    }
    private IEnumerator ConnectingServer()
    {
        while (client != null || !TCP_BallClient.ready)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Connect fail
        if (client == null)
        {
            messageEvent.Invoke("Connect failed");
            if(networkMode == NetworkMode.Server)
            {

            }
            networkMode = NetworkMode.None;
        }
        // Connect success
        else if (TCP_BallClient.ready)
        {
            clientConnectEvent.Invoke();
        }
    }
}

public enum GameState
{
    None,
    Connect,
    Room,
    InGame
}
public enum NetworkMode
{
    None,
    Client,
    Server
}
