using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

//목적1 : 각 공의 점수를 저장하고 관리한다.
//속성1 : 플레이어 이름과 점수를 가지고 있는 Dictionary

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    //public TMP_Text scoreTextUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlusScore(GameObject ball)
    {
        foreach(var data in GameManager.Instance.entryPlayerDataList)
        {
            if(data.id == ball.name)
            {
                data.score++;
            }
        }

        UpdateScore();
    }

    public void UpdateScore()
    {
        string tmpText = "Rank 1st\n";
        string tmpID = "------";
        int tmpScore = 0;
        foreach(var data in GameManager.Instance.entryPlayerDataList)
        {
            if(data.score > tmpScore)
            {
                tmpID = data.id;
                tmpScore = data.score;
            }
        }
        tmpText += tmpID + " " + tmpScore;
        //scoreTextUI.text = tmpText;
    }
}
