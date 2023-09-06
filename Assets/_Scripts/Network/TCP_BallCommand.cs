using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static RankingResultUI;

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

    public static List<CommandData> ClientReceiveEvent(string rawData)
    {
        Debug.Log(rawData);

        return CommandCore.Decode(instance.command, rawData);
    }

    public static string ServerBroadcastEvent(List<CommandData> datas)
    {
        string rawData = CommandCore.Encode(instance.command, datas);
        Debug.Log(rawData);

        return rawData;
    }
}
