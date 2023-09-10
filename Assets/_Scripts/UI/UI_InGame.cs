using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UI_InGame : MonoBehaviour
{
    [SerializeField]
    private RectTransform playersPos;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private TextMeshProUGUI headerText;
    [SerializeField]
    private TMP_InputField chatting;

    private static Dictionary<string, UI_OnePlayerInRoom> roomPool = new Dictionary<string, UI_OnePlayerInRoom>();
    private static Dictionary<string, int> scoreList = new Dictionary<string, int>();
    private static UI_InGame instance;
    private static string myID;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void MakeNew(List<BallEntryPlayerData> allPlayerList, string _myID)
    {
        instance.gameObject.SetActive(true);
        myID = _myID;

        foreach (BallEntryPlayerData player in allPlayerList)
        {
            if (roomPool.ContainsKey(player.id))
            {
                return;
            }

            roomPool.Add
                (
                    player.id,
                    ObjectPoolingManager.Pooling(instance.playerPrefab).GetComponent<UI_OnePlayerInRoom>()
                );
            roomPool[player.id].Set(player.color, "0", instance.playersPos);
            scoreList.Add(player.id, 0);
        }

        instance.headerText.text = "Rank -\nScore " + 0;
    }

    public static void ResetList()
    {
        instance.chatting.text = "";

        if (roomPool.Count > 0)
        {
            List<string> ids = roomPool.Keys.ToList();

            foreach (string id in ids)
            {
                roomPool[id].gameObject.SetActive(false);
                roomPool[id].transform.SetParent(instance.transform);
                roomPool.Remove(id);
            }
        }
        roomPool.Clear();
        scoreList.Clear();
    }

    public static void ScoreChange(string id, int score)
    {
        // Set score
        roomPool[id].score = score.ToString();
        scoreList[id] = score;

        // Turn off all ui
        List<string> ids = new List<string>();
        List<int> scores = scoreList.Values.ToList();
        int rankNow = 1;
        scores.Sort();

        foreach(UI_OnePlayerInRoom p in roomPool.Values)
        {
            p.gameObject.SetActive(false);
            p.transform.SetParent(instance.transform);
        }

        // Calculate rank
        while (scores.Count > 0)
        {
            foreach(string idNow in scoreList.Keys)
            {
                if (int.Parse(roomPool[idNow].score) == scores[scores.Count - 1])
                {
                    // Found this id rank
                    ids.Add(idNow);

                    // Check mine
                    if (idNow == myID)
                    {
                        instance.headerText.text = "Rank " + rankNow + "\nScore " + score;
                    }

                    // Sort UI
                    roomPool[idNow].transform.SetParent(instance.playersPos);
                    roomPool[idNow].gameObject.SetActive(true);

                    scores.RemoveAt(scores.Count - 1);
                    rankNow++;
                }
            }
        }
    }

    public static void Chatting(string id, string text)
    {
        instance.chatting.text += "[" + id + "] : " + text + "\n"; 
    }
}
