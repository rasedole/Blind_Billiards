using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankingResultUI : MonoBehaviour
{
    private static RankingResultUI instance;

    [SerializeField]
    private OneRankRow[] rankRows;

    private int maxRankingRow = 5;

    List<RankData> savedRankDatas;

    [SerializeField]
    private Animator animator;

    private bool uiShow;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            uiShow = false;
            savedRankDatas = new List<RankData>();
        }
    }

    void Start()
    {
        savedRankDatas = new List<RankData>();
        maxRankingRow = rankRows.Length;
    }

    public void RankShow(List<RankData> rankDatas)
    {
        savedRankDatas.Clear();
        LoadRankData(rankDatas.Count);

        // 새 점수 추가
        foreach (RankData rankData in rankDatas)
        {
            savedRankDatas.Add(rankData);
        }

        // score순 내림차순 정렬
        savedRankDatas = savedRankDatas.OrderByDescending(x => x.score).ToList();

        // maxRankingRow 밖의 순위는 삭제
        if (maxRankingRow < savedRankDatas.Count)
        {
            savedRankDatas.RemoveRange(maxRankingRow, savedRankDatas.Count - maxRankingRow);
        }

        //Debug.LogWarning(savedRankDatas.Count + ", " + maxRankingRow);
        if (savedRankDatas.Count < maxRankingRow)
            maxRankingRow = savedRankDatas.Count;

        // 변경된 순위 데이터 저장
        SaveRankData(rankDatas.Count);

        // 화면에 보여주기.
        ShowRankData();

        Invoke("ResetUI", 6f);
    }

    // 게임한 인원수(1p/2p/..)에 해당하는 저장된 랭킹 데이터 받아오기
    void LoadRankData(int headCount)
    {
        RankData rankData = new RankData();
        maxRankingRow = rankRows.Length;
        for (int i = 0; i < maxRankingRow; i++)
        {
            // 저장 key 형식예시 : Rank 1P 1 playtime
            //         형식     : Rank | 플레이어수 | 랭킹(order) | playTime/score
            if (PlayerPrefs.HasKey("Rank(" + headCount + ")P(" + i + ")Turn(" + GameManager.gameMaxTurn + ")" + "playTime"))
            {
                rankData.playTime = DateTime.Parse(PlayerPrefs.GetString("Rank(" + headCount + ")P(" + i + ")Turn(" + GameManager.gameMaxTurn + ")" + "playTime"));
                rankData.id = PlayerPrefs.GetString("Rank(" + headCount + ")P(" + i + ")Turn(" + GameManager.gameMaxTurn + ")" + "id");
                rankData.score = PlayerPrefs.GetInt("Rank(" + headCount + ")P(" + i + ")Turn(" + GameManager.gameMaxTurn + ")" + "score");
                savedRankDatas.Add(rankData);
            }
        }
    }

    void ShowRankData()
    {
        int rankCount = 0;

        foreach (OneRankRow obj in rankRows)
        {
            // 필요없는 행 비활성화
            if (rankCount >= savedRankDatas.Count)
            {
                obj.gameObject.SetActive(false);
                continue;
            }

            // 0번째 자식 == 순위
            // 1번째 자식 == id
            // 2번째 자식 == 게임 시간
            // 3번째 자식 == 점수
            string date = savedRankDatas[rankCount].playTime.ToString("yyyy-MM-dd") +
                "\n" + savedRankDatas[rankCount].playTime.ToString("HH:mm");
            obj.Set
                (
                    (rankCount + 1).ToString(),
                    savedRankDatas[rankCount].id,
                    date,
                    savedRankDatas[rankCount].score.ToString()
                );
            rankCount++;
        }
        animator.Play("In");
    }

    // savedRankDatas PlayerPrefs 저장
    void SaveRankData(int headCount)
    {
        maxRankingRow = rankRows.Length;
        for (int i = 0; i < maxRankingRow; i++)
        {
            if (savedRankDatas.Count <= i)
            {
                break;
            }
            PlayerPrefs.SetString("Rank(" + headCount + ")P(" + i + ")Turn(" + GameManager.gameMaxTurn + ")" + "playTime", savedRankDatas[i].playTime.ToString());
            PlayerPrefs.SetString("Rank(" + headCount + ")P(" + i + ")Turn(" + GameManager.gameMaxTurn + ")" + "id", savedRankDatas[i].id);
            PlayerPrefs.SetString("Rank(" + headCount + ")P(" + i + ")Turn(" + GameManager.gameMaxTurn + ")" + "score", savedRankDatas[i].score.ToString());
        }
        //Debug.LogWarning(savedRankDatas.Count + ", !" + maxRankingRow);
    }

    public static void StartRankUI(List<RankData> rankDatas)
    {
        if (!instance.uiShow)
        {
            instance.uiShow = true;
            instance.RankShow(rankDatas);
        }
    }

    private void ResetUI()
    {
        animator.Play("Out");
        if(TCP_BallCore.networkMode == NetworkMode.Server)
        {
            TCP_BallUI.GameOver();
        }
        uiShow = false;
    }
    //public void UpdateRankData(List<RankData> rankDatas)
    //{
    //    RankShow(rankDatas);
    //}
}

public struct RankData
{
    public string id;
    public DateTime playTime;
    public int score;
}