using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TCP_BallCommand : MonoBehaviour
{
    // Tcp commands.
    [SerializeField]
    private CommandCore command;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClientReceiveEvent(string rawData)
    {
        Debug.Log(rawData);
    }
}
