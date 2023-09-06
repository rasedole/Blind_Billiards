using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEditor.PackageManager;
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

    private static Dictionary<string, TcpClient> roomPlayer;
    private static List<TcpClient> pendingClients;
    private static TCP_BallServer instance;

    private TcpListener listener;

    private TCP_BallServer() { }

    public static TCP_BallServer OpenServer(string ip, int port, string id)
    {
        TCP_BallServer server = new TCP_BallServer();

        roomPlayer = new Dictionary<string, TcpClient>();
        pendingClients = new List<TcpClient>();

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
        pendingClients.Add(_listener.EndAcceptTcpClient(ar));
        listener.BeginAcceptTcpClient(AcceptTcpClient, listener);
    }

    private static void CheckID(TcpClient client, string idInput)
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
                    new List<TcpClient>
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
                new List<TcpClient>
                {
                    client
                }
            );

        roomPlayer.Add(idTemp, client);
        pendingClients.Remove(client);
    }

    private static void Broadcast(List<CommandData> datas, List<TcpClient> clients)
    {
        string rawData = TCP_BallCommand.ServerBroadcastEvent(datas);
        try
        {
            foreach (TcpClient client in clients)
            {
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.WriteLine(rawData);
                writer.Flush();
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
}
