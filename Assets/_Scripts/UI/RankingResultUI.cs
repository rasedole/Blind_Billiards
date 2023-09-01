using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using static RankingResultUI;

public class RankingResultUI : MonoBehaviour
{
    private static RankingResultUI instance;

    [SerializeField] GameObject[] rankObj;

    private int maxRankingRow = 5;

    List<RankData> savedRankDatas;
    public struct RankData
    {
        public DateTime playTime;
        public int score;
    }

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        maxRankingRow = rankObj.Length;
        savedRankDatas = new List<RankData>();

        List<RankData> testRankDatas = new List<RankData>();

        DateTime nowDate = DateTime.Now;
        testRankDatas.Add(new RankData { playTime = nowDate, score = 3252 });
        testRankDatas.Add(new RankData { playTime = nowDate, score = 1634 });
        testRankDatas.Add(new RankData { playTime = nowDate, score = 2467 });
        testRankDatas.Add(new RankData { playTime = nowDate, score = 3436 });
        testRankDatas.Add(new RankData { playTime = nowDate, score = 1327 });
        testRankDatas.Add(new RankData { playTime = nowDate, score = 9999 });
        RankShow(testRankDatas);
    }

    public void RankShow(List<RankData> rankDatas)
    {
        LoadRankData(rankDatas.Count);

        // �� ���� �߰�
        foreach(RankData rankData in rankDatas)
        {
            savedRankDatas.Add(rankData);
        }

        // score�� �������� ����
        savedRankDatas = savedRankDatas.OrderByDescending(x => x.score).ToList();

        // maxRankingRow ���� ������ ����
        if(maxRankingRow < savedRankDatas.Count)
        {
            savedRankDatas.RemoveRange(maxRankingRow, savedRankDatas.Count-maxRankingRow);
        }

        // ȭ�鿡 �����ֱ�.
        ShowRankData();

        // ����� ���� ������ ����
        SaveRankData(rankDatas.Count);
    }

    // ������ �ο���(1p/2p/..)�� �ش��ϴ� ����� ��ŷ ������ �޾ƿ���
    void LoadRankData(int headCount)
    {
        RankData rankData = new RankData();
        for (int i = 1; i <= maxRankingRow; i++)
        {
            // ���� key ���Ŀ��� : Rank 1P 1 playtime
            //         ����     : Rank | �÷��̾�� | ��ŷ(order) | playTime/score
            if (PlayerPrefs.HasKey("Rank " + headCount + "P " + i + "playtime"))
            {
                rankData.playTime = DateTime.Parse(PlayerPrefs.GetString("Rank " + headCount + "P " + i + "playtime"));
                rankData.score = PlayerPrefs.GetInt("Rank " + headCount + "P " + i + "score");
                savedRankDatas.Add(rankData);
            }
        }
    }

    void ShowRankData()
    {
        int rankCount = 1;

        foreach (GameObject obj in rankObj)
        {
            // �ʿ���� �� ��Ȱ��ȭ
            if (rankCount > maxRankingRow)
                break;

            // 0��° �ڽ� == ����
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = rankCount.ToString();

            // 1��° �ڽ� == ���� �ð�
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = savedRankDatas[rankCount - 1].playTime.ToString("yyyy-MM-dd HH:mm");

            // 2��° �ڽ� == ����
            obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = savedRankDatas[rankCount - 1].score.ToString();

            rankCount++;

            obj.SetActive(true);
        }
    }

    // savedRankDatas PlayerPrefs ����
    void SaveRankData(int headCount)
    {
        for (int i = 1; i <= maxRankingRow; i++)
        {
            PlayerPrefs.SetString("Rank " + headCount + "P " + i + "playTime", savedRankDatas[i - 1].playTime.ToString());
            PlayerPrefs.SetString("Rank " + headCount + "P " + i + "score", savedRankDatas[i - 1].score.ToString());
        }
    }
}