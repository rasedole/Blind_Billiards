using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;

public class TCP_BallClient
{
    public static bool ready
    {
        get
        {
            return instance != null && instance.socket != null && instance.socket.Connected && instance.stream != null && instance.stream.DataAvailable;
        }
    }

    private TcpClient socket;
    private string id;
    private int idOffset;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private static TCP_BallClient instance;

    private TCP_BallClient() { }

    public static TCP_BallClient ConnectToServer(string ip, int port, string id)
    {
        // Check client allready exist
        if (instance != null)
        {
            TCP_BallCore.messageEvent.Invoke("TCP client allready exist!");
            return null;
        }

        // Make client
        TCP_BallClient client = new TCP_BallClient();
        try
        {
            client.idOffset = 0;
            client.id = id;
            client.socket = new TcpClient(ip, port);
            client.socket.BeginConnect(ip, port, (result) => { client.OnConnect(result); }, null);

            if (client.id == "")
            {
                client.id = "Guest";
            }

            instance = client;
            return instance;
        }
        catch (Exception e)
        {
            TCP_BallCore.errorEvent.Invoke(e.Message);
            DisconnectClient(client);
        }

        return null;
    }

    public static void DisconnectClient(TCP_BallClient client = null)
    {
        if(client != null)
        {
            client.OnDisconnect();
            if (client.writer != null)
            {
                client.writer.Close();
                client.writer = null;
            }
            if (client.reader != null)
            {
                client.reader.Close();
                client.reader = null;
            }
            if (client.stream != null)
            {
                client.stream.Close();
                client.stream = null;
            }
            if (client.socket != null)
            {
                client.socket.Close();
                client.socket = null;
            }
        }

        if(instance != null)
        {
            instance.OnDisconnect();
            if (instance.writer != null)
            {
                instance.writer.Close();
                instance.writer = null;
            }
            if (instance.reader != null)
            {
                instance.reader.Close();
                instance.reader = null;
            }
            if (instance.stream != null)
            {
                instance.stream.Close();
                instance.stream = null;
            }
            if (instance.socket != null)
            {
                instance.socket.Close();
                instance.socket = null;
            }

            instance = null;
        }
    }

    private void OnConnect(IAsyncResult callback)
    {
        if(callback.IsCompleted)
        {
            if (socket.Connected)
            {
                stream = socket.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
            }
            else
            {
                TCP_BallCore.messageEvent.Invoke("Can't connect to server!");
                DisconnectClient(this);
            }
        }
        else
        {
            Debug.Log("not yet testing");
        }
    }
    private void OnDisconnect()
    {
        
    }

    public void Update()
    {
        if (ready)
        {
            // Read data
            string data = reader.ReadLine();
            if (data != null)
            {
                TCP_BallCore.clientReceiveEvent.Invoke(data);
            }
        }
    }


    // Send message to server
    public void Send(string rawData)
    {
        if (!ready)
        {
            return;
        }

        try
        {
            writer.WriteLine(rawData);
            writer.Flush();
        }
        catch (Exception e)
        {
            TCP_BallCore.errorEvent.Invoke(e.Message);
        }
    }
}
