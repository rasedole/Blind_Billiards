using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;

public class TCP_BallServer
{
    private static Dictionary<string, TcpClient> roomPlayer;
    private static List<TcpClient> pendingClients;

    private TcpListener listener;

    private TCP_BallServer() { }

    public static TCP_BallServer OpenServer()
    {
        TCP_BallServer server = new TCP_BallServer();

        //clients = new List<TCP_Link>();
        //disconnectList = new List<TCP_Link>();

        //try
        //{
        //    TCP_Core.SetIpAndPort();
        //    server = new TcpListener(TCP_Core.ipForServer, TCP_Core.port);
        //    server.Start();

        //    StartListening();
        //    serverStarted = true;
        //    TCP_Core.Message($"서버가 {TCP_Core.port}에서 시작되었습니다.");
        //}
        //catch (Exception e)
        //{
        //    TCP_Core.Message($"Socket error: {e.Message}");
        //}

        server.listener = new TcpListener(TCP_Core.ipForServer, TCP_Core.port);
        return server;
    }
}
