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

    public TCP_BallServerConnectClients(TcpClient _client)
    {
        client = _client;
        writer = null;
        reader = null;
    }

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
