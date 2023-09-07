using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class TCP_BallCommand : MonoBehaviour
{
    // Tcp commands.
    [SerializeField]
    private CommandCore command;
    [SerializeField]
    private UnityEvent<string> clientSetSelfIDEvent;
    [SerializeField]
    private UnityEvent<string> _serverEntryNewClientEvent;
    [SerializeField]
    private UnityEvent<BallEntryPlayerData> clientAddOtherPlayerEvent;
    [SerializeField]
    private UnityEvent<List<BallEntryPlayerData>> clientGetAllPlayerEvent;


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
        //ui.GoToRoom();
        Debug.Log(rawData);
        List<CommandData> datas = CommandCore.Decode(instance.command, rawData);
        int index = 0;

        while (index < datas.Count)
        {
            // Check error
            if (datas[0].command != 0)
            {
                TCP_BallCore.messageEvent.Invoke("Header error!");
                return null;
            }

            switch (Enum.Parse<TCP_BallHeader>(datas[index].text))
            {
                // Check id
                case TCP_BallHeader.SetID:
                    if (datas.Count < 2 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }

                    if (datas[1 + index].command == 1)
                    {
                        // Client GameManager Function
                        instance.clientSetSelfIDEvent.Invoke(datas[1 + index].text);
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
                    if (datas.Count < 6 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }
                    // Check error
                    if
                        (
                            datas[1 + index].command != 1 ||
                            datas[2 + index].command != 3 ||
                            datas[3 + index].command != 5 ||
                            datas[4 + index].command != 6 ||
                            datas[5 + index].command != 7
                        )
                    {
                        TCP_BallCore.messageEvent.Invoke("Type error!");
                        return null;
                    }

                    BallEntryPlayerData playerData = new BallEntryPlayerData();

                    // ID
                    playerData.id = datas[1 + index].text;

                    try
                    {
                        Color color = new Color();

                        // Turn order(client index)
                        playerData.index = int.Parse(datas[2 + index].text);

                        // Color RGB
                        color.r = float.Parse(datas[3 + index].text);
                        color.g = float.Parse(datas[4 + index].text);
                        color.b = float.Parse(datas[5 + index].text);
                        playerData.color = color;
                    }
                    catch
                    {
                        TCP_BallCore.messageEvent.Invoke("Value error!");
                        return null;
                    }

                    instance.clientAddOtherPlayerEvent.Invoke(playerData);
                    datas.RemoveRange(index, 6);
                    break;

                // Get all player list
                case TCP_BallHeader.AllPlayerList:
                    if (datas.Count < 6 + index)
                    {
                        TCP_BallCore.messageEvent.Invoke("No input value!");
                        return null;
                    }

                    index++;
                    List<BallEntryPlayerData> allPlayerList = new List<BallEntryPlayerData>();
                    while 
                        (
                            datas.Count < 6 + index &&
                            datas[index].command == 1
                        )
                    {
                        // Read one player
                        BallEntryPlayerData onePlayer;

                        if(TryTranslateEntryPlayer
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
                        }
                        else
                        {
                            return null;
                        }

                        instance.clientGetAllPlayerEvent.Invoke(allPlayerList);
                    }
                    break;

                default:
                    break;
            }
        }
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



    /* ========== Server ========== */
    public static string ServerBroadcastEvent(List<CommandData> datas)
    {
        string rawData = CommandCore.Encode(instance.command, datas);
        Debug.Log(rawData);

        return rawData;
    }
    public static List<CommandData> ServerReceiveEvent(string rawData, TCP_BallServerConnectClients clients)
    {
        Debug.Log(rawData);
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
                default:
                    break;
            }
        }
        return datas;
    }

}
