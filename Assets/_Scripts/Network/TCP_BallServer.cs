using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Windows;

public class TCP_BallServer
{
    public static bool started
    {
        get
        {
            return instance != null && instance.listener != null;
        }
    }

    private static Dictionary<string, TCP_BallServerConnectClients> roomPlayer;
    private static List<TCP_BallServerConnectClients> pendingClients;
    private static List<string> disconnectList;
    private static TCP_BallServer instance;

    private TcpListener listener;

    private TCP_BallServer() { }

    public static TCP_BallServer OpenServer(string ip, int port)
    {
        TCP_BallServer server = new TCP_BallServer();

        roomPlayer = new Dictionary<string, TCP_BallServerConnectClients>();
        pendingClients = new List<TCP_BallServerConnectClients>();

        try
        {
            // IP parse
            IPAddress _ip;
            if(!IPAddress.TryParse(ip, out _ip))
            {
                TCP_BallCore.messageEvent.Invoke("IP parse error!");
            }

            server.listener = new TcpListener(_ip, port);
            server.listener.Start();

            server.listener.BeginAcceptTcpClient(server.AcceptTcpClient, server.listener);
            TCP_BallCore.messageEvent.Invoke($"서버가 {port}에서 시작되었습니다.");
        }
        catch (Exception e)
        {
            TCP_BallCore.errorEvent.Invoke(e.Message);
        }

        instance = server;
        return instance;
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener _listener = (TcpListener)ar.AsyncState;
        pendingClients.Add
            (
                new TCP_BallServerConnectClients(_listener.EndAcceptTcpClient(ar))
            );
        listener.BeginAcceptTcpClient(AcceptTcpClient, listener);
    }

    public static void CheckID(TCP_BallServerConnectClients client, string idInput)
    {
        string idTemp = idInput;
        if (roomPlayer.ContainsKey(idTemp))
        {
            // Increase id offset
            int idOffset = 0;
            while (roomPlayer.ContainsKey(idTemp + idOffset))
            {
                idOffset++;
            }
            idTemp = idTemp + idOffset;

            // Return new id to client
            Broadcast
                (
                    new List<CommandData>
                    {
                        new CommandData(0, ((int)TCP_BallHeader.SetID).ToString()),
                        new CommandData(1, idTemp)
                    },
                    new List<TCP_BallServerConnectClients>
                    {
                        client
                    }
                );
        }

        // Broadcast this client entering to all other clients
        Broadcast
            (
                new List<CommandData>
                {
                    new CommandData(0, ((int)TCP_BallHeader.Entry).ToString()),
                    new CommandData(1, idTemp)
                },
                new List<TCP_BallServerConnectClients>
                {
                    client
                }
            );

        // Entry client to new player
        roomPlayer.Add(idTemp, client);
        pendingClients.Remove(client);
    }

    private static void Broadcast(List<CommandData> datas, List<TCP_BallServerConnectClients> clients)
    {
        string rawData = TCP_BallCommand.ServerBroadcastEvent(datas);
        try
        {
            foreach (TCP_BallServerConnectClients client in clients)
            {
                if (client.writer == null)
                {
                    client.writer = new StreamWriter(client.client.GetStream());
                }
                client.writer.WriteLine(rawData);
                client.writer.Flush();
            }
        }
        catch (Exception e)
        {
            TCP_BallCore.errorEvent.Invoke(e.Message);
        }
    }

    public void CloseServer()
    {
        if (listener != null)
        {
            listener.Stop();
        }
        listener = null;
    }

    public void Update()
    {
        if(disconnectList != null)
        {
            disconnectList.Clear();
        }
        disconnectList = new List<string>();

        foreach (KeyValuePair<string, TCP_BallServerConnectClients> pair in roomPlayer)
        {
            // Check client connected
            bool connected;
            try
            {
                if (pair.Value.client != null && pair.Value.client.Client != null && pair.Value.client.Client.Connected)
                {
                    if (pair.Value.client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        connected = !(pair.Value.client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                    }

                    connected = true;
                }
                else
                {
                    connected = false;
                }
            }
            catch
            {
                connected = false;
            }

            // Remember client disconnected 
            if (!connected)
            {
                pair.Value.client.Close();
                disconnectList.Add(pair.Key);
            }
            // Listen client send message
            else
            {
                NetworkStream s = pair.Value.client.GetStream();
                if (s.DataAvailable)
                {
                    if(pair.Value.reader == null)
                    {
                        pair.Value.reader = new StreamReader(s, true);
                    }
                    string data = pair.Value.reader.ReadLine();
                    if (data != null)
                    {
                        List<CommandData> commands = TCP_BallCommand.ServerReceiveEvent(data, pair.Value);
                    }
                }
            }
        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            //Broadcast($"{disconnectList[i].id} 연결이 끊어졌습니다", clients);

            //clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }
}
