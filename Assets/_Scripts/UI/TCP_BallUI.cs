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
    private TMP_InputField roomMaxCount;
    [SerializeField]
    private Animator connectAnimatorUI;
    [SerializeField]
    private Animator mainAnimatorUI;
    [SerializeField]
    private Animator roomAnimatorUI;
    [SerializeField]
    private TextMeshProUGUI roomInfo;
    [SerializeField]
    private GameObject kickedRoomUI;

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


    public static GameState gameState
    {
        get { return _gameState; }
    }


    private static GameState _gameState = GameState.None;


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
        UI_RoomManager.ResetList();

        enterRoomEvent.Invoke();
        roomMaxCount.readOnly = false;
        roomInfo.text = ip.text + " / " + port.text;
        roomInfo.gameObject.SetActive(false);

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
            roomMaxCount.readOnly = true;
        }

        if (TCP_BallCore.networkMode != NetworkMode.None)
        {
            roomInfo.gameObject.SetActive(true);
            connectAnimatorUI.Play("Out");
            roomAnimatorUI.Play("InFromLeft");
        }

        _gameState = GameState.Room;
        roomMaxCount.text = PlayerPrefs.GetInt("room", 4).ToString();
    }

    public void LastClient()
    {
        _gameState = GameState.Connect;
        ip.text = PlayerPrefs.GetString("ipClient", FindMyIP.localIP);
        port.text = PlayerPrefs.GetInt("portClient", 5000).ToString();
        id.text = PlayerPrefs.GetString("idClient", "Guest");
    }
    public void SubmitClient()
    {
        PlayerPrefs.SetString("ipClient", ip.text);
        PlayerPrefs.SetInt("portClient", int.Parse(port.text));
        PlayerPrefs.SetString("idClient", id.text);
    }

    public void LastServer()
    {
        _gameState = GameState.Connect;
        ip.text = FindMyIP.localIP;
        port.text = PlayerPrefs.GetInt("portServer", 5000).ToString();
        id.text = PlayerPrefs.GetString("idServer", "Guest");
    }
    public void SubmitServer()
    {
        PlayerPrefs.SetInt("portServer", int.Parse(port.text));
        PlayerPrefs.SetString("idServer", id.text);
    }

    public void SubmitRoom()
    {
        PlayerPrefs.SetInt("room", int.Parse(roomMaxCount.text));
    }

    public void ConnectFail()
    {
        connectFaildEvent.Invoke();
    }

    public void ExitRoom()
    {
        roomMaxCount.readOnly = false;
        if (TCP_BallCore.networkMode == NetworkMode.None)
        {
            exitRoomEventSolo.Invoke();
            return;
        }

        TCP_BallCore.CloseServer();
        exitRoomEventNetwork.Invoke();
    }

    public void SetRoomMaxPlayer(string maxPlayer)
    {
        // Check min value
        if(int.Parse(maxPlayer) < 2)
        {
            roomMaxCount.SetTextWithoutNotify(2.ToString());
        }

        // Check game mode
        if (TCP_BallCore.networkMode == NetworkMode.Server)
        {
            TCP_BallServer.maxPlayerCount = int.Parse(maxPlayer);
        }
        else if (TCP_BallCore.networkMode == NetworkMode.Client)
        {
            roomMaxCount.SetTextWithoutNotify(maxPlayer);
        }
        else
        {

        }
    }

    public void KickedInRoom()
    {
        kickedRoomUI.SetActive(true);
    }
}
