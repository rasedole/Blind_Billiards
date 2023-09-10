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
        List<BallEntryPlayerData> removelist = GameManager.Instance.entryPlayerDataList;
        List<BallEntryPlayerData> outlist = new List<BallEntryPlayerData>();
        int typecount = 0;
        bool isexist = false;
        foreach (var data in GameManager.Instance.entryPlayerDataList)
        {
            isexist = false;
            foreach (var outdata in outlist)
            {
                if (data.id == outdata.id)
                {
                    isexist = true;
                    break;
                }
            }
            if (!isexist)
            {
                typecount++;
            }
            if (typecount > changedCount)
            {
                foreach (var ball in outlist)
                {
                    Debug.Log(ball.id + "++++");
                    removelist.Remove(ball);
                }
                foreach (var tmp in removelist)
                    Debug.Log(tmp.id + "!@!@");
                return removelist;
            }
            outlist.Add(data);
        }

        foreach (var ball in outlist)
        {
            Debug.Log(ball.id + "++++");
            removelist.Remove(ball);
        }
        foreach (var tmp in removelist)
            Debug.Log(tmp.id + "!@!@");
        return removelist;
    }
}
