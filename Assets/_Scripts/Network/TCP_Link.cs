using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

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
