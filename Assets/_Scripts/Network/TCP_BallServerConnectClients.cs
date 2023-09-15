using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class TCP_BallServerConnectClients
{
    public TcpClient client;
    public StreamWriter writer;
    public StreamReader reader;
    public NetworkStream stream;
    public bool turnCheck
    {
        get { return _turnCheck; }
        set 
        {
            Debug.Log(value + "");
            _turnCheck = value; 
        }
    }

    public TCP_BallServerConnectClients(TcpClient _client)
    {
        client = _client;
        stream = client.GetStream();
        writer = new StreamWriter(stream);
        reader = new StreamReader(stream, true);
    }

    private bool _turnCheck;

    public void Disconnect()
    {
        if(writer != null)
        {
            writer.Close();
            writer = null;
        }
        if(reader != null)
        {
            reader.Close();
            reader = null;
        }
        if(client != null)
        {
            client.Close();
            client = null;
        }
    }
}
