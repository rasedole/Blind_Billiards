using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public static class TCP_BallGameManagerGetterAdapter
{
    public static BallEntryPlayerData lastPlayer
    {
        get
        {
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
        if (GameManager.Instance.entryPlayerDataList.Count > changedCount)
        {
            List<BallEntryPlayerData> removelist = GameManager.Instance.entryPlayerDataList;
            for (int i = 0; i < changedCount; i++)
            {
                removelist.RemoveAt(0);
            }

            return removelist;
        }
        return null;
    }

    public static string myID
    {
        get
        {
            return GameManager.Instance.myID;
        }
    }

    public static string nowTurnID
    {
        get
        {
            return TurnManager.Instance.GetTurnBall().name;
        }
    }

    public static int MoveDataListCount()
    {
        return GameManager.Instance.ballMoveData.Count;
    }

    public static List<int> MoveDataNullList(int maxCount)
    {
        List<int> indexList = new();
        List<int> removeList = new();

;       for (int i = 0; i < maxCount; i++)
        {
            indexList.Add(i);
        }

        foreach (var _moveData in GameManager.Instance.ballMoveData)
        {
            removeList.Add(_moveData.index);
        }

        int offset = 0;
        while (indexList.Count > offset && removeList.Count > 0)
        {
            if(indexList[offset] == removeList[0])
            {
                removeList.RemoveAt(0);
                indexList.RemoveAt(offset);
            }
            else
            {
                offset++;
            }
        }

        return indexList;
    }

    public static List<MoveData> MoveDataListCallback(List<int> indexList)
    {
        List<MoveData> moveList = new();
        for(int i = 0; i < indexList.Count; i++)
        {
            moveList.Add(GameManager.Instance.ballMoveData[indexList[i]]);
        }
        return moveList;
    }
}