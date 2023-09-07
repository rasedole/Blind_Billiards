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
    RoomMaxKick = 3,

}

public enum GameState
{
    None = -2,
    Connect = -1,
    Room = 0,
    InGame = 1
}

[Serializable]
public enum NetworkMode
{
    None,
    Client,
    Server
}
