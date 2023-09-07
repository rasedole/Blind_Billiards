using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TCP_BallHeader 
{
    None = -1,
    SetID = 0,
    Entry = 1,
    AllPlayerList = 2,

}

public enum GameState
{
    None,
    Connect,
    Room,
    InGame
}

[Serializable]
public enum NetworkMode
{
    None,
    Client,
    Server
}
