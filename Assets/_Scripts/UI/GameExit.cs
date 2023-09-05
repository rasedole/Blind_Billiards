using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void GameEnd()
    {
        Application.Quit();
        Debug.Log("Game End");
    }
}
