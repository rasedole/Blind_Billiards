using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_RoomManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform playersPos;
    [SerializeField]
    private GameObject playerPrefab;

    private static Dictionary<string, UI_OnePlayerInRoom> roomPool = new Dictionary<string, UI_OnePlayerInRoom>();

    private static UI_RoomManager instance;

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
            MakeNew(player.id, player.color);
        }
    }
    public static void MakeNew(string id, Color color)
    {
        if (roomPool.ContainsKey(id))
        {
            return;
        }

        roomPool.Add
            (
                id,
                ObjectPoolingManager.Pooling(instance.playerPrefab).GetComponent<UI_OnePlayerInRoom>()
            );
        roomPool[id].Set(color, id, instance.playersPos);
    }

    public static void ResetList()
    {
        if(roomPool.Count > 0)
        {
            List<string> ids = roomPool.Keys.ToList();

            foreach (string id in ids)
            {
                roomPool[id].gameObject.SetActive(false);
                roomPool[id].transform.SetParent(instance.transform);
                roomPool.Remove(id);
            }
        }
    }
}
