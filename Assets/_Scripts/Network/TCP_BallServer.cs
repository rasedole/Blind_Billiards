using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
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
    public static int maxPlayerCount
    {
        set
        {
            // Check room max player count decrease
            List<string> broadCastList = new List<string>();
            if (_maxPlayerCount > value)
            {
                List<BallEntryPlayerData> clients = TCP_BallGameManagerGetterAdapter.RoomMaxCountDecrease(value);
                if(clients != null && clients.Count > 0)
                {
                    foreach (BallEntryPlayerData client in clients)
                    {
                        // Kick overflow
                        RoomMaxDecreaseKick(roomPlayer[client.id]);
                        roomPlayer[client.id].client.Close();
                        roomPlayer[client.id].client = null;
                        roomPlayer.Remove(client.id);
                        broadCastList.Add(client.id);
                    }
                    BroadCastDisconnectAtRoom(broadCastList);
                }
            }

            // Notify to other player
            Broadcast
                (
                    new List<CommandData>()
                    {
                        new CommandData(0, ((int)TCP_BallHeader.RoomMaxCountChanged).ToString()) ,
                        new CommandData(3, value.ToString())
                    },
                    roomPlayer.Values.ToList()
                );
            _maxPlayerCount = value;
        }
    }

    private static Dictionary<string, TCP_BallServerConnectClients> roomPlayer;
    private static List<TCP_BallServerConnectClients> pendingClients;
    private static List<string> disconnectList;
    private static TCP_BallServer instance;
    private static int _maxPlayerCount;
    private static int moveDataIndex = 0;

    private TcpListener listener;

    private TCP_BallServer() { }

    public static TCP_BallServer OpenServer(string ip, int port)
    {
        TCP_BallServer server = new TCP_BallServer();

        roomPlayer = new Dictionary<string, TCP_BallServerConnectClients>();
        pendingClients = new List<TCP_BallServerConnectClients>();
        _maxPlayerCount = PlayerPrefs.GetInt("room", 4);

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
            server.CloseServer();
            return null;
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
        List<CommandData> broadcastDataToNewClient = new List<CommandData>();
        if (roomPlayer.ContainsKey(idTemp))
        {
            // Increase id offset
            int idOffset = 0;
            while (roomPlayer.ContainsKey(idTemp + idOffset))
            {
                // Check reconnect
                if (roomPlayer[idTemp + idOffset] == null)
                {
                    idTemp = idTemp + idOffset;
                    broadcastDataToNewClient.AddRange(GetAllEntryPlayer());
                    roomPlayer[idTemp] = client;
                    pendingClients.Remove(client);
                    return;
                }
                idOffset++;
            }
            idTemp = idTemp + idOffset;

            // Return new id to client 
            broadcastDataToNewClient.Add(new CommandData(0, ((int)TCP_BallHeader.SetID).ToString()));
            broadcastDataToNewClient.Add(new CommandData(1, idTemp));
        }

        // Check max player count in room
        if(roomPlayer.Count >= _maxPlayerCount)
        {
            RoomMaxDecreaseKick(client);
            return;
        }

        // Entry new client to server GameManager
        TCP_BallCommand.serverEntryNewClientEvent.Invoke(idTemp);

        // Broadcast this client entering to all other clients
        List<CommandData> broadcastDataToOtherClient = new List<CommandData>
            {
                // Head
                new CommandData(0, ((int)TCP_BallHeader.Entry).ToString())
            };
        broadcastDataToOtherClient.AddRange(TranslateEntryPlayer(TCP_BallGameManagerGetterAdapter.lastPlayer));
        Broadcast
            (
                broadcastDataToOtherClient,
                roomPlayer.Values.ToList()
            );

        // Return all player entrydata to new client
        broadcastDataToNewClient.AddRange(GetAllEntryPlayer());

        // Add room max count to new client
        broadcastDataToNewClient.Add(new CommandData(0, ((int)TCP_BallHeader.RoomMaxCountChanged).ToString()));
        broadcastDataToNewClient.Add(new CommandData(3, _maxPlayerCount.ToString()));

        // Broadcast to new client
        Broadcast
            (
                broadcastDataToNewClient,
                new List<TCP_BallServerConnectClients> { client }
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
            for(int i = 0; i < clients.Count; i++)
            {
                if (clients[i] != null && clients[i].client != null && clients[i].client.Connected)
                {
                    if (clients[i].writer == null) 
                    {
                        TCP_BallCore.messageEvent.Invoke("Server can't find writer!");
                        continue;
                    }
                    clients[i].writer.WriteLine(rawData);
                    clients[i].writer.Flush();
                }
            }
        }
        catch (Exception e)
        {
            TCP_BallCore.errorEvent.Invoke(e.Message);
        }
    }

    public void CloseServer()
    {
        if (started)
        {
            // Disconnect all clients
            Broadcast
                (
                    new List<CommandData> { new CommandData(0, ((int)TCP_BallHeader.ServerDisconnect).ToString()) },
                    roomPlayer.Values.ToList()
                );

            foreach (TCP_BallServerConnectClients client in roomPlayer.Values)
            {
                if(client.client != null)
                {
                    client.Disconnect();
                }
            }
            roomPlayer.Clear();

            // Stop server listener
            listener.Stop();
        }
        listener = null;
        TCP_BallCore.TurnCheckClear();
    }

    public void Update()
    {
        if(disconnectList != null)
        {
            disconnectList.Clear();
        }
        disconnectList = new List<string>();

        // Receive for entry player client
        foreach (KeyValuePair<string, TCP_BallServerConnectClients> pair in roomPlayer)
        {
            // Check client connected
            bool connected = CheckConnect(pair.Value);

            // Remember client disconnected 
            if (!connected)
            {
                if(pair.Value != null && pair.Value.client != null)
                {
                    pair.Value.client.Close();
                }
                disconnectList.Add(pair.Key);
            }
            // Listen client send message
            else
            {
                while (pair.Value.stream.DataAvailable)
                {
                    if(pair.Value.reader == null)
                    {
                        TCP_BallCore.messageEvent.Invoke("Server can't find reader!");
                        return;
                    }
                    string data = pair.Value.reader.ReadLine();

                    if (data != null)
                    {
                        List<CommandData> commands = TCP_BallCommand.ServerReceiveEvent(data, pair.Value);
                        if (commands != null && commands.Count > 0)
                        {
                            int index = 0;

                            while (index < commands.Count)
                            {
                                switch (Enum.Parse<TCP_BallHeader>(commands[0].text))
                                {
                                    case TCP_BallHeader.RoomDisconnect:
                                        disconnectList.Add(pair.Key);
                                        commands.RemoveAt(index);
                                        break;

                                    case TCP_BallHeader.Shoot:
                                        if(pair.Key == TCP_BallGameManagerGetterAdapter.nowTurnID)
                                        {
                                            Vector3 vector = new Vector3();
                                            vector.x = float.Parse(commands[index + 1].text);
                                            vector.y = float.Parse(commands[index + 2].text);
                                            vector.z = float.Parse(commands[index + 3].text);
                                            TCP_BallCommand.shootBall.Invoke(vector);
                                        }
                                        else
                                        {
                                            Debug.LogError("Order Error");
                                        }
                                        commands.RemoveRange(index, 4);
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < disconnectList.Count; i++)
        {
            if (roomPlayer.ContainsKey(disconnectList[i]) && roomPlayer[disconnectList[i]] != null)
            {
                roomPlayer[disconnectList[i]].Disconnect();
                roomPlayer[disconnectList[i]] = null;
            }

            // Check disconnect at room
            if(TCP_BallUI.gameState != GameState.InGame)
            {
                roomPlayer.Remove(disconnectList[i]);
            }

            if (TCP_BallUI.gameState == GameState.InGame)
            {
                //Broadcast(new List<CommandData>() { new CommandData(2, $"{disconnectList[i]}와의 연결이 끊어졌습니다") }, roomPlayer.Values.ToList());
            }
        }

        if (TCP_BallUI.gameState != GameState.InGame && disconnectList.Count > 0)
        {
            // Delete disconnected clients in room to all other players
            BroadCastDisconnectAtRoom(disconnectList);
            disconnectList.Clear();
        }

        // Receive for pending client
        int pendingClientsIndex = 0;
        while(pendingClientsIndex < pendingClients.Count)
        {
            // Check client connected
            bool connected = CheckConnect(pendingClients[pendingClientsIndex]);

            // Remember client disconnected 
            if (!connected)
            {
                pendingClients[pendingClientsIndex].client.Close();
                pendingClients.RemoveAt(pendingClientsIndex);
                continue;
            }
            pendingClientsIndex++;
        }

        // Listen client send message
        pendingClientsIndex = 0;
        while (pendingClientsIndex < pendingClients.Count)
        {
            if (pendingClients[pendingClientsIndex].stream != null && pendingClients[pendingClientsIndex].stream.DataAvailable)
            {
                if (pendingClients[pendingClientsIndex].reader == null)
                {
                    TCP_BallCore.messageEvent.Invoke("Server can't find reader!");
                    return;
                }
                string data = pendingClients[pendingClientsIndex].reader.ReadLine();
                if (data != null)
                {
                    List<CommandData> commands = TCP_BallCommand.ServerReceiveEvent(data, pendingClients[pendingClientsIndex]);
                }
            }
            pendingClientsIndex++;
        }
    }

    private static List<CommandData> TranslateEntryPlayer(BallEntryPlayerData player)
    {
        return new List<CommandData>
        {
            // ID
            new CommandData(1, player.id),
            // Turn order(client index)
            new CommandData(3, player.index.ToString()),
            // Color RGB
            new CommandData(5, player.color.r.ToString()),
            new CommandData(6, player.color.g.ToString()),
            new CommandData(7, player.color.b.ToString())
        };
    }

    private static List<CommandData> GetAllEntryPlayer()
    {
        // Return all player entrydata to new client
        List<CommandData> broadcastDataToNewClient = new List<CommandData>
        {
            new CommandData(0, ((int)TCP_BallHeader.AllPlayerList).ToString())
        };
        List<BallEntryPlayerData> entryPlayers = TCP_BallGameManagerGetterAdapter.allEntryPlayers;
        foreach (BallEntryPlayerData onePlayer in entryPlayers)
        {
            broadcastDataToNewClient.AddRange(TranslateEntryPlayer(onePlayer));
        }

        // Set server gamestate
        broadcastDataToNewClient.Add(new CommandData(3, ((int)TCP_BallUI.gameState).ToString()));
        return broadcastDataToNewClient;
    }

    private static void RoomMaxDecreaseKick(TCP_BallServerConnectClients client)
    {
        RoomMaxDecreaseKick(new List<TCP_BallServerConnectClients> { client });
    }
    private static void RoomMaxDecreaseKick(List<TCP_BallServerConnectClients> clients)
    {
        Broadcast
            (
                new List<CommandData> { new CommandData(0, ((int)TCP_BallHeader.RoomMaxKick).ToString()) },
                clients
            );
    }

    private static void BroadCastDisconnectAtRoom(List<string> broadCastList)
    {
        List<CommandData> disconnectCommand = new List<CommandData>() { new CommandData(0, ((int)TCP_BallHeader.RoomDisconnect).ToString()) };
        foreach (string id in broadCastList)
        {
            disconnectCommand.Add(new CommandData(1, id));
        }

        Broadcast
            (
                disconnectCommand,
                roomPlayer.Values.ToList()
            );
    }

    public static void StartGame()
    {
        List<CommandData> commandList = new List<CommandData>() { new CommandData(0, ((int)TCP_BallHeader.GameStart).ToString()) };
        Broadcast
            (
                commandList,
                roomPlayer.Values.ToList()
            );
    }

    public static void Moved(MoveData moveData)
    {
        if(TCP_BallCore.networkMode != NetworkMode.Server)
        {
            Debug.LogError("You are not server!");
        }

        Broadcast
        (
            new List<CommandData>()
            {
                new CommandData(0, ((int)TCP_BallHeader.BallMove).ToString()),
                new CommandData(3, moveDataIndex.ToString()),
                new CommandData(4, moveData.ballIndex.ToString()),
                new CommandData(5, moveData.startPos.x.ToString()),
                new CommandData(6, moveData.startPos.y.ToString()),
                new CommandData(7, moveData.startPos.z.ToString()),
                new CommandData(8, moveData.startTime.ToString())
            },
            roomPlayer.Values.ToList()
        );
        moveDataIndex++;
    }

    public static void TurnEnd(int score, int turn = 0)
    {
        if (TCP_BallCore.networkMode != NetworkMode.Server)
        {
            Debug.LogError("You are not server!");
        }

        TCP_BallCore.TurnCheck(turn, TurnEndChecking(score, turn, 0.3f));
    }
    private static IEnumerator TurnEndChecking(int score, int turn, float pingTime)
    {
        WaitForSeconds wait = new WaitForSeconds(pingTime);
        List<string> keys = roomPlayer.Keys.ToList();
        List<TCP_BallServerConnectClients> values = new List<TCP_BallServerConnectClients>();
        foreach (string key in keys)
        {
            if (roomPlayer[key] != null)
            {
                roomPlayer[key].turnCheck = false;
                values.Add(roomPlayer[key]);
            }
        }
        while (values.Count > 0)
        {
            int index = 0;
            while (index < values.Count)
            {
                if (values[index].turnCheck)
                {
                    values.RemoveAt(index);
                    continue;
                }
                index++;
            }

            Broadcast
            (
                new List<CommandData>()
                {
                new CommandData(0, ((int)TCP_BallHeader.TurnEnd).ToString()),
                new CommandData(3, turn.ToString()),
                new CommandData(3, moveDataIndex.ToString()),
                new CommandData(4, score.ToString())
                },
                values
            );

            yield return wait;
        }

        moveDataIndex = 0;
    }

    public static bool CheckPlayerConnect(string id)
    {
        if(roomPlayer.ContainsKey(id) && roomPlayer[id] != null)
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    private bool CheckConnect(TCP_BallServerConnectClients client)
    {
        try
        {
            if (client != null && client.client.Client != null && client.client.Client.Connected)
            {
                if (client.client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

}
