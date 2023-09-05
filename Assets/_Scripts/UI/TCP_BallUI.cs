using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TCP_BallUI : MonoBehaviour
{
    // Message field, client and server will use to show something.
    [SerializeField]
    private TMP_InputField connectMessage;
    [SerializeField]
    private TMP_InputField chattingMessage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Message(string text)
    {
        Debug.Log(text);

        // Check error

        // Print
        if (TCP_BallCore.gameState == GameState.Connect)
        {
            connectMessage.text = text;
            return;
        }
        if(TCP_BallCore.gameState == GameState.InGame)
        {
            chattingMessage.text += text;
        }
    }

    public void ResetText()
    {
        connectMessage.text = "";
        chattingMessage.text = "";
    }

    public void ErrorHandle(string err)
    {
        Debug.LogError(err);
    }

    public void GoToRoom()
    {

    }
}
