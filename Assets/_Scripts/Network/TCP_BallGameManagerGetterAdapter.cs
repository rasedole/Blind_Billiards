using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TCP_BallGameManagerGetterAdapter
{
    public static BallEntryPlayerData lastPlayer
    {
        get
        {
            //PlayerData of LastPlayer in GameManager
            return GameManager.Instance.entryPlayerDataList[GameManager.Instance.entryPlayerDataList.Count];
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
}
