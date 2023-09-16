using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

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
    [SerializeField]
    private Animator inGameWrapper;
    [SerializeField]
    private Image nowTurnPlayerColor;
    [SerializeField]
    private TextMeshProUGUI nowTurnPlayerID;
    [SerializeField]
    private TextMeshProUGUI gameTurn;

    private static Dictionary<string, UI_OnePlayerInRoom> roomPool = new Dictionary<string, UI_OnePlayerInRoom>();
    private static Dictionary<string, InGameUIdata> uiDataList = new Dictionary<string, InGameUIdata>();
    private static UI_InGame instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void MakeNew(List<BallEntryPlayerData> allPlayerList)
    {
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
            uiDataList.Add(player.id, new InGameUIdata(0, player.color, player.index));
        }

        instance.headerText.text = "Rank -\nScore " + 0;
        SetNowTurnPlayer(0);
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
        uiDataList.Clear();
    }

    public static void ScoreChange(string id, int score)
    {
        // Set score
        roomPool[id].score = score.ToString();
        uiDataList[id].score = score;

        // Turn off all ui
        List<string> ids = uiDataList.Keys.ToList();
        List<int> scores = new List<int>();
        foreach(InGameUIdata data in uiDataList.Values)
        {
            scores.Add(data.score);
        }
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
            int scoreNow = scores[scores.Count - 1];
            string idNow = "";

            foreach(string index in ids)
            {
                if(scoreNow == int.Parse(roomPool[index].score))
                {
                    idNow = index;
                    roomPool[idNow].transform.SetParent(instance.playersPos);
                    roomPool[idNow].gameObject.SetActive(true);
                }
            }

            if(idNow != "")
            {
                // Check mine
                if (idNow == TCP_BallGameManagerGetterAdapter.myID)
                {
                    instance.headerText.text = "Rank " + rankNow + "\nScore " + scoreNow;
                }

                rankNow++;
                ids.Remove(idNow);
            }

            scores.RemoveAt(scores.Count - 1);
        }
    }

    public static void Chatting(string id, string text)
    {
        instance.chatting.text += "[" + id + "] : " + text + "\n"; 
    }

    public static void SetNowTurnPlayer(int index)
    {
        //nowTurnPlayerColor nowTurnPlayerID
        foreach(string id in uiDataList.Keys)
        {
            if (uiDataList[id].index == index)
            {
                uiDataList[id].color.a = 1f;
                instance.nowTurnPlayerColor.color = uiDataList[id].color;
                instance.nowTurnPlayerID.text = id + "ÀÇ Â÷·Ê";
            }
        }
        instance.gameTurn.text = (TurnManager.Instance.gameTurn + 1) + " / " + GameManager.gameMaxTurn;
    }
}

public class InGameUIdata
{
    public int score;
    public Color color;
    public int index;

    public InGameUIdata(int score, Color color, int index)
    {
        this.score = score;
        this.color = color;
        this.index = index;
    }
}
