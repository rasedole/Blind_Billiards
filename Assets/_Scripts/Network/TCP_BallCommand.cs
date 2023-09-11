using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class TCP_BallCommand : MonoBehaviour
{
    // Tcp commands.
    [SerializeField]
    private CommandCore command;
    [SerializeField]
    private TCP_BallUI ui;
    [SerializeField]
    private UnityEvent<string> clientSetSelfIDEvent;
    [SerializeField]
    private UnityEvent<string> _serverEntryNewClientEvent;
    [SerializeField]
    private UnityEvent<BallEntryPlayerData> clientAddOtherPlayerEvent;
    [SerializeField]
    private UnityEvent<List<BallEntryPlayerData>> clientGetAllPlayerEvent;
    [SerializeField]
    private UnityEvent<List<string>> removeRoomPlayerEvent;


    private static TCP_BallCommand instance;


    public static UnityEvent<string> serverEntryNewClientEvent
    {
        get { return instance._serverEntryNewClientEvent; }
    }


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private static bool TryTranslateEntryPlayer(CommandData id, CommandData index, CommandData r, CommandData g, CommandData b, out BallEntryPlayerData outData)
    {
        outData = null;
        BallEntryPlayerData data = new BallEntryPlayerData();

        if
            (
                id.command != 1 ||
                index.command != 3 ||
                r.command != 5 ||
                g.command != 6 ||
                b.command != 7
            )
        {
            //Debug.Log("13567 > " + id.command + "" + index.command + "" + r.command + "" + g.command + "" + b.command);
            Debug.Log(CommandCore.Encode(instance.command, new List<CommandData>() { id, index, r, g, b }));
            TCP_BallCore.messageEvent.Invoke("Type error!");
            return false;
        }

        try
        {
            // ID
            data.id = id.text;

            // Turn order(client index)
            data.index = int.Parse(index.text);

            // Color RGB
            Color color = new Color();
            color.r = float.Parse(r.text);
            color.g = float.Parse(g.text);
            color.b = float.Parse(b.text);
            data.color = color;
        }
        catch
        {
            TCP_BallCore.messageEvent.Invoke("Value error!");
            return false;
        }

        outData = data;
        return true;
    }

    /* ========== Client ========== */
    public static List<CommandData> ClientReceiveEvent(string rawData)
    {
        Debug.Log("ClientReceiveEvent > " + rawData);
        List<CommandData> datas = CommandCore.Decode(instance.command, rawData);
        int index = 0;

        while (index < datas.Count)
        {
            Debug.LogWarning("while");
            // Check error
            if (datas[0].command != 0)
            {
                TCP_BallCore.messageEvent.Invoke("Header error!");
                return null;
            }

            Debug.LogWarning("switch");
            switch (Enum.Parse<TCP_BallHeader>(datas[index].text))
            {
                // Check id
                case TCP_BallHeader.SetID:
                    Debug.LogWarning("SetID");
                    if (datas.Count < 2 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }

                    if (datas[1 + index].command == 1)
                    {
                        // Client GameManager Function
                        instance.clientSetSelfIDEvent.Invoke(datas[1 + index].text);
                        TCP_BallClient.Flush();
                    }
                    else
                    {
                        TCP_BallCore.messageEvent.Invoke("Type error!");
                        return null;
                    }
                    datas.RemoveRange(index, 2);
                    break;

                // Server has new player entry
                case TCP_BallHeader.Entry:
                    Debug.LogWarning("Entry");
                    if (datas.Count < 6 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }

                    BallEntryPlayerData playerData;
                    if(TryTranslateEntryPlayer
                        (
                            datas[1 + index], 
                            datas[2 + index], 
                            datas[3 + index], 
                            datas[4 + index], 
                            datas[5 + index], 
                            out playerData
                        ))
                    {
                        instance.clientAddOtherPlayerEvent.Invoke(playerData);
                        datas.RemoveRange(index, 6);
                    }
                    else
                    {
                        return null;
                    }

                    break;

                // Get all player list
                case TCP_BallHeader.AllPlayerList:
                    Debug.LogWarning("AllPlayerList");
                    if (datas.Count < 6 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }

                    index++;
                    List<BallEntryPlayerData> allPlayerList = new List<BallEntryPlayerData>();
                    while
                        (
                            datas.Count >= (6 + index) &&
                            datas[index].command == 1
                        )
                    {
                        Debug.LogWarning("TCP_BallHeader.AllPlayerList");
                        // Read one player
                        BallEntryPlayerData onePlayer;

                        if (TryTranslateEntryPlayer
                            (
                                datas[index], 
                                datas[index + 1], 
                                datas[index + 2], 
                                datas[index + 3], 
                                datas[index + 4],
                                out onePlayer
                            ))
                        {
                            allPlayerList.Add(onePlayer);
                            datas.RemoveRange(index, 5);
                        }
                        else
                        {
                            return null;
                        }
                    }

                    // Get server gamestate
                    if (datas[index].command != 3)
                    {
                        Debug.Log(CommandCore.Encode(instance.command, datas) + "");
                        TCP_BallCore.messageEvent.Invoke("Type error!");
                        return null;
                    }
                    else
                    {
                        switch (Enum.Parse<GameState>(datas[index].text))
                        {
                            case GameState.Connect:
                            case GameState.Room:
                                instance.ui.GoToRoom();

                                break;

                            case GameState.InGame:

                                break;

                            default:
                                TCP_BallCore.messageEvent.Invoke("Value error!");
                                break;
                        }
                    }
                    index--;
                    datas.RemoveRange(index, 2);

                    instance.clientGetAllPlayerEvent.Invoke(allPlayerList);
                    break;

                // Kicked at room
                case TCP_BallHeader.RoomMaxKick:
                    if (TCP_BallUI.gameState == GameState.Room)
                    {
                        instance.ui.KickedInRoom();
                    }
                    else if (TCP_BallUI.gameState == GameState.Connect)
                    {
                        TCP_BallCore.messageEvent.Invoke("Room is full!");
                        instance.ui.ConnectFail();
                    }
                    TCP_BallClient.DisconnectClient();
                    return null;

                // Other player disconnect at room
                case TCP_BallHeader.RoomDisconnect:
                    if (datas.Count < 1 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }

                    // Add all ID
                    index++;
                    List<string> idList = new List<string>();
                    while
                        (
                            datas.Count > index &&
                            datas[index].command == 1
                        )
                    {
                        idList.Add(datas[index].text);
                        datas.RemoveAt(index);
                    }

                    // Remove room player
                    index--;
                    datas.RemoveAt(index);
                    instance.removeRoomPlayerEvent.Invoke(idList);
                    break;

                case TCP_BallHeader.RoomMaxCountChanged:
                    int value = 0;
                    if (datas.Count < 1 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }
                    else if
                        (
                            datas[index + 1].command != 3 ||
                            int.TryParse(datas[index + 1].text, out value)
                        )
                    {
                        if(TCP_BallCore.networkMode != NetworkMode.Server)
                        {
                            instance.ui.SetRoomMaxPlayer(datas[index + 1].text);
                        }
                    }
                    else
                    {
                        TCP_BallCore.messageEvent.Invoke("Type error!");
                        return null;
                    }

                    datas.RemoveRange(index, 2);
                    break;

                default:
                    break;
            }
        }
        Debug.LogWarning("return");
        return datas;
    }

    public static string ClientOnConnect(string id)
    {
        // Set id
        return CommandCore.Encode
            (
                instance.command,
                new List<CommandData>
                {
                    new CommandData(0, ((int)TCP_BallHeader.SetID).ToString()),
                    new CommandData(1, id)
                }
            );
    }

    public static string ClientSendEvent(List<CommandData> datas)
    {
        string rawData = CommandCore.Encode(instance.command, datas);
        Debug.Log("ClientSendEvent > " + rawData);

        return rawData;
    }



    /* ========== Server ========== */
    public static string ServerBroadcastEvent(List<CommandData> datas)
    {
        string rawData = CommandCore.Encode(instance.command, datas);
        Debug.Log("ServerBroadcastEvent > " + rawData);

        return rawData;
    }
    public static List<CommandData> ServerReceiveEvent(string rawData, TCP_BallServerConnectClients clients)
    {
        Debug.Log("ServerReceiveEvent > " + rawData);
        List<CommandData> datas = CommandCore.Decode(instance.command, rawData);
        int index = 0;

        while(index < datas.Count)
        {
            // Check error
            if (datas[0].command != 0)
            {
                TCP_BallCore.messageEvent.Invoke("Header error!");
                return null;
            }

            switch (Enum.Parse<TCP_BallHeader>(datas[0].text))
            {
                // Check id
                case TCP_BallHeader.SetID:
                    if (datas.Count < 2)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }

                    if (datas[1].command == 1)
                    {
                        TCP_BallServer.CheckID(clients, datas[1].text);
                    }
                    else
                    {
                        TCP_BallCore.messageEvent.Invoke("Type error!");
                        return null;
                    }
                    datas.RemoveRange(0, 2);
                    break;

                case TCP_BallHeader.RoomDisconnect:
                    index++;
                    break;

                default:
                    break;
            }
        }
        return datas;
    }

}
