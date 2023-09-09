using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TCP_BallGameManagerGetterAdapter
{
    public static BallEntryPlayerData lastPlayer
    {
        get
        {
            //GameManager.Instance.AddPlayerData("1");
            //BallEntryPlayerData temp = GameManager.Instance.entryPlayerDataList[GameManager.Instance.entryPlayerDataList.Count - 1];
            //Debug.Log("ID: " + temp.id + "\n" + "index: " + temp.index + "\n" + "COlor: " + temp.color + "\n" + "Score: " + temp.score + "\n");
            return GameManager.Instance.entryPlayerDataList[GameManager.Instance.entryPlayerDataList.Count-1];
        }
    }

    public static List<BallEntryPlayerData> allEntryPlayers
    {
        get
        {
            
            //PlayerData of All Player in GameManager
            return GameManager.Instance.entryPlayerDataList;
        }
    }

    public static List<BallEntryPlayerData> RoomMaxCountDecrease(int changedCount)
    {
        return null;
    }
}
