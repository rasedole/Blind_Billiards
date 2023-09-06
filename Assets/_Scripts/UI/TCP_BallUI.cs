using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TCP_BallUI : MonoBehaviour
{
    // Message field, client and server will use to show something.
    [SerializeField]
    private TMP_InputField connectMessage;
    [SerializeField]
    private TMP_InputField chattingMessage;
    [SerializeField]
    private TMP_InputField ip;
    [SerializeField]
    private TMP_InputField port;
    [SerializeField]
    private TMP_InputField id;
    [SerializeField]
    private TMP_InputField room;
    [SerializeField]
    private Animator connectAnimatorUI;
    [SerializeField]
    private Animator mainAnimatorUI;
    [SerializeField]
    private Animator roomAnimatorUI;

    // Events
    [SerializeField]
    private UnityEvent connectFaildEvent;
    [SerializeField]
    private UnityEvent connectingEvent;
    [SerializeField]
    private UnityEvent enterRoomEventSolo;
    [SerializeField]
    private UnityEvent enterRoomEvent;
    [SerializeField]
    private UnityEvent exitRoomEventSolo;
    [SerializeField]
    private UnityEvent exitRoomEventNetwork;

    public static GameState gameState = GameState.None;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Message(string text)
    {
        Debug.Log(text);

        // Print
        if (gameState == GameState.Connect)
        {
            connectMessage.text = text;
            return;
        }
        if(gameState == GameState.InGame)
        {
            chattingMessage.text += text;
        }
    }

    public void ResetText()
    {
        connectMessage.text = "";
        chattingMessage.text = "";
    }

    public void ErrorHandle(string err)
    {
        connectFaildEvent.Invoke();
        Debug.LogError(err);
    }

    public void ConnectButton()
    {
        connectingEvent.Invoke();
        if (TCP_BallCore.networkMode == NetworkMode.None)
        {
            return;
        }
        else if (TCP_BallCore.networkMode == NetworkMode.Server)
        {
            TCP_BallCore.AttemptToOpenServer();
        }
        else if (TCP_BallCore.networkMode == NetworkMode.Client)
        {
            TCP_BallCore.AttemptToConnectServer();
        }
    }

    public void GoToRoom()
    {
        enterRoomEvent.Invoke();
        if (TCP_BallCore.networkMode == NetworkMode.None)
        {
            mainAnimatorUI.Play("Out");
            roomAnimatorUI.Play("InFromRight");
            enterRoomEventSolo.Invoke();
        }
        else if (TCP_BallCore.networkMode == NetworkMode.Server)
        {
            SubmitServer();
        }
        else if (TCP_BallCore.networkMode == NetworkMode.Client)
        {
            SubmitClient();
        }

        if (TCP_BallCore.networkMode != NetworkMode.None)
        {
            connectAnimatorUI.Play("Out");
            roomAnimatorUI.Play("InFromLeft");
        }

        gameState = GameState.Room;
        room.text = PlayerPrefs.GetString("room", "4");
    }

    public void LastClient()
    {
        gameState = GameState.Connect;
        ip.text = PlayerPrefs.GetString("ipClient", FindMyIP.localIP);
        port.text = PlayerPrefs.GetString("portClient", "5000");
        id.text = PlayerPrefs.GetString("idClient", "Guest");
    }
    public void SubmitClient()
    {
        PlayerPrefs.SetString("ipClient", ip.text);
        PlayerPrefs.SetString("portClient", port.text);
        PlayerPrefs.SetString("idClient", id.text);
    }

    public void LastServer()
    {
        gameState = GameState.Connect;
        ip.text = FindMyIP.localIP;
        port.text = PlayerPrefs.GetString("portServer", "5000");
        id.text = PlayerPrefs.GetString("idServer", "Guest");
    }
    public void SubmitServer()
    {
        PlayerPrefs.SetString("portServer", port.text);
        PlayerPrefs.SetString("idServer", id.text);
    }

    public void SubmitRoom()
    {
        PlayerPrefs.SetString("room", room.text);
    }

    public void ConnectFail()
    {
        connectFaildEvent.Invoke();
    }

    public void ExitRoom()
    {
        if (TCP_BallCore.networkMode == NetworkMode.None)
        {
            exitRoomEventSolo.Invoke();
            return;
        }

        TCP_BallCore.CloseServer();
        exitRoomEventNetwork.Invoke();
    }
}