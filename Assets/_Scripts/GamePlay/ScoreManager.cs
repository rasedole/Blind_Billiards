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

    public int savedScore = 0;

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
                UI_InGame.ScoreChange(data.id, data.score);

                break;
            }
        }
    }

    public void PlusScore(GameObject ball, int _score)
    {
        foreach (var data in GameManager.Instance.entryPlayerDataList)
        {
            if (data.id == ball.name)
            {
                Debug.LogError(_score + "");
                data.score = _score;
                UI_InGame.ScoreChange(data.id, data.score);

                break;
            }
        }
    }
}
