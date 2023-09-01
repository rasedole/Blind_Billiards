using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class TCP_Core : MonoBehaviour
{
    // Set ip and port.
    [SerializeField]
    TextOrHint ipInput, portInput, idInput;
    public static void SetIpAndPort()
    {
        if (instance == null)
        {
            Message("Instance doesn't exist!");
            return;
        }

        instance.usingIP = instance.ipInput.text;
        instance.usingPort = instance.portInput.text;
        instance.usingId = instance.idInput.text;
    }

    // Message field, client and server will use to show something.
    [SerializeField]
    TMP_InputField messageField;
    public static void Message(string data)
    {
        if (instance != null)
            instance.messageField.text += "\n" + data + "\n";

#if UNITY_EDITOR
        Debug.Log(data);
#endif
    }

    // Don't edit below! ===========================================================================
    static TCP_Core instance;
    string usingIP = "", usingPort = "", usingId = "";

    // Using for start server or client.
    //static List<TCP_Server> servers = new List<TCP_Server>();
    //static List<TCP_Client> clients = new List<TCP_Client>();
    static TCP_Server server = new TCP_Server();
    static TCP_Client client = new TCP_Client();

    // Get ip, string to IPAddress.
    public static IPAddress ipForServer
    {
        get
        {
            IPAddress ip_;

            if (instance != null)
                IPAddress.TryParse(instance.usingIP, out ip_);
            else
            {
                Message("Instance doesn't exist!");
                ip_ = IPAddress.None;
            }

            return ip_;
        }
    }

    // Get ip(string).
    public static string ipString
    {
        get
        {
            if (instance != null)
                return instance.usingIP;

            return "";
        }
    }

    // Get port number, string to int.
    public static int port
    {
        get
        {
            int port_ = 0;

            if (instance != null)
                int.TryParse(instance.usingPort, out port_);
            else
                Message("Instance doesn't exist!");

            return port_;
        }
    }

    // Get nick(string).
    public static string id
    {
        get
        {
            if (instance != null)
                return instance.usingId;

            return "";
        }
    }

    // Tcp commands.
    [SerializeField]
    CommandCore command_;
    public static CommandCore command
    {
        get
        {
            if (instance != null)
                return instance.command_;

            return null;
        }
    }

    // Boot TCP.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Message("Only one TCP_Core can available!");
            Destroy(gameObject);
        }
    }

    // Server or client's task.
    // Must be update everytime.
    void FixedUpdate()
    {
        server.ListenAndCheckDisconnect();
        client.Listen();
    }

    public static void StartServer()
    {
        if (instance == null)
        {
            Message("Instance doesn't exist!");
            return;
        }

        server.ServerCreate();
    }

    public static void StartClient()
    {
        if (instance == null)
        {
            Message("Instance doesn't exist!");
            return;
        }

        client.ConnectToServer();
    }

    public static void StopServer()
    {
        server.CloseServer();
    }

    public static void StopClient()
    {
        client.CloseSocket();
    }

    public void OnSendButton(TextOrHint sendInput)
    {
        if (sendInput.text.Trim() == "") return;

        string message = sendInput.text;
        sendInput.text = "";
        client.Send(0, message);
    }

    void OnApplicationQuit()
    {
        client.CloseSocket();
    }
}

public class TCP_Link
{
    public TcpClient tcp;
    public string id;

    public TCP_Link(TcpClient clientSocket)
    {
        id = "Guest";
        tcp = clientSocket;
    }
}