using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankingResultUI : MonoBehaviour
{
    public static RankingResultUI instance;

    [SerializeField] GameObject[] rankObj;

    private int maxRankingRow = 5;

    List<RankData> savedRankDatas = new List<RankData>();
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
        savedRankDatas = new List<RankData>();
        maxRankingRow = rankObj.Length;
    }

    public void RankShow(List<RankData> rankDatas)
    {
        LoadRankData(rankDatas.Count);
        maxRankingRow = rankDatas.Count;

        // 새 점수 추가
        foreach (RankData rankData in rankDatas)
        {
            savedRankDatas.Add(rankData);
        }

        // score순 내림차순 정렬
        savedRankDatas = savedRankDatas.OrderByDescending(x => x.score).ToList();

        // maxRankingRow 밖의 순위는 삭제
        if(maxRankingRow < savedRankDatas.Count)
        {
            savedRankDatas.RemoveRange(maxRankingRow, savedRankDatas.Count-maxRankingRow);
        }

        if (savedRankDatas.Count < maxRankingRow)
            maxRankingRow = savedRankDatas.Count;

        // 화면에 보여주기.
        ShowRankData();

        // 변경된 순위 데이터 저장
        SaveRankData(rankDatas.Count);
    }

    // 게임한 인원수(1p/2p/..)에 해당하는 저장된 랭킹 데이터 받아오기
    void LoadRankData(int headCount)
    {
        RankData rankData = new RankData();
        for (int i = 1; i <= maxRankingRow; i++)
        {
            // 저장 key 형식예시 : Rank 1P 1 playtime
            //         형식     : Rank | 플레이어수 | 랭킹(order) | playTime/score
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
            // 필요없는 행 비활성화
            if (rankCount > maxRankingRow)
                break;

            // 0번째 자식 == 순위
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = rankCount.ToString();

            // 1번째 자식 == 게임 시간
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = savedRankDatas[rankCount - 1].playTime.ToString("yyyy-MM-dd HH:mm");

            // 2번째 자식 == 점수
            obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = savedRankDatas[rankCount - 1].score.ToString();

            rankCount++;

            obj.SetActive(true);
        }
    }

    // savedRankDatas PlayerPrefs 저장
    void SaveRankData(int headCount)
    {
        for (int i = 1; i <= maxRankingRow; i++)
        {
            PlayerPrefs.SetString("Rank " + headCount + "P " + i + "playTime", savedRankDatas[i - 1].playTime.ToString());
            PlayerPrefs.SetString("Rank " + headCount + "P " + i + "score", savedRankDatas[i - 1].score.ToString());
        }
    }

    public void UpdateRankData(List<RankData> rankDatas)
    {
        RankShow(rankDatas);
    }
}
