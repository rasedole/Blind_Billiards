using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class TCP_BallClient
{
    public static bool ready
    {
        get
        {
            return instance != null && instance.socket != null && instance.socket.Connected && instance.stream != null;
        }
    }

    private TcpClient socket;
    private string id;
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
            client.id = id;
            client.socket = new TcpClient(ip, port);
            client.stream = client.socket.GetStream();
            client.writer = new StreamWriter(client.stream);
            client.reader = new StreamReader(client.stream);

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
            //TCP_BallCore.messageEvent.Invoke("Can't connect to server!");
            //DisconnectClient(this);
        }

        return null;
    }

    public static void DisconnectClient(TCP_BallClient client = null)
    {
        // Check disconnect manualy
        if (ready)
        {
            if(TCP_BallCore.networkMode == NetworkMode.Client)
            {
                Send(new List<CommandData>() { new CommandData(0, ((int)TCP_BallHeader.RoomDisconnect).ToString()) });
            }
        }

        // Abort
        if(client != null)
        {
            //client.OnDisconnect();
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
            //instance.OnDisconnect();
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

    public void Update()
    {
        if (ready)
        {
            string data = "";
            while (stream.DataAvailable)
            {
                data += reader.ReadLine();
                if (data != null)
                {
                    List<CommandData> commands = TCP_BallCommand.ClientReceiveEvent(data);
                }
            }
            reader.DiscardBufferedData();
        }
    }

    // Send message to server
    public static void Send(List<CommandData> commands)
    {
        instance.Send(TCP_BallCommand.ClientSendEvent(commands));
    }
    public void Send(string rawData)
    {
        if (!ready)
        {
            return;
        }

        try
        {
            Debug.Log("Client Send > " + rawData);
            writer.WriteLine(rawData);
            writer.Flush();
        }
        catch (Exception e)
        {
            TCP_BallCore.errorEvent.Invoke(e.Message);
        }
    }

    public static void TurnEndChecking()
    {
        Send(new List<CommandData>()
        {
            new CommandData(0, ((int)TCP_BallHeader.TurnCheckedPing).ToString()) ,
            new CommandData(1, instance.id)
        });
    }
}
