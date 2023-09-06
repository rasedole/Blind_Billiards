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
    private UnityEvent clientConnectEvent;


    private UnityAction<string> clientReceiveEvent;


    private static TCP_BallCommand instance;


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
