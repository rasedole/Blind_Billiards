using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//목적1 : 현재 턴을 가지고 있고 턴이 종료되면 턴을 넘긴다.
//속성1 : 현재 턴
//순서1 : 턴을 다음 턴으로 넘긴다.

//목적2 : 턴에 해당하는 공을 반환한다.
//속성2 : 플레이어 공 리스트
//순서2 : 턴에 해당하는 공을 반환한다.

//솔로플레이 상태일 시 턴이 종료되면 다음 턴 사람을 MyPlayer로 설정한다.

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    //속성1 : 현재 턴
    public int currentTurn
    {
        get
        {
            return _currentTurn;
        }
        private set
        {
            _currentTurn = value;
        }
    }
    int _currentTurn;

    //속성2 : 플레이어 공 리스트
    [SerializeField] List<GameObject> ballList;

    //순서1 : 턴을 다음 턴으로 넘긴다.
    public void EndTurn()
    {
        foreach(var data in GameManager.Instance.entryPlayerDataList)
        {
            if(data.id == GetTurnBall().name)
            {
                TCP_BallServer.TurnEnd(data.score);
                break;
            }
        }

        currentTurn++;
        if(currentTurn >= GameManager.Instance.gamePlayers.Count)
        {
            currentTurn = 0;
        }
        GameManager.Instance.joystick.GetComponent<BallLineRender>().ResetBallStatus();
        UIManager.Instance.UpdateTurn(currentTurn);
        if(TCP_BallCore.networkMode == NetworkMode.None)
        {
            GameManager.Instance.SoloPlaySet(currentTurn);
        }

        //GuestReplayer.ReplayTurn(GameManager.Instance.ballMoveData);

        //if(GetTurnBall().name != GameManager.Instance.myID)
        //{
        //    GameManager.Instance.joystick.gameObject.SetActive(false);
        //}
        //else
        //{
        //    GameManager.Instance.joystick.gameObject.SetActive(true);
        //}
    }

    public void EndTurn(int _countOfMoveData, int _differenceOfScore)
    {

        if(_countOfMoveData != GameManager.Instance.ballMoveData.Count)
        {
            Debug.LogError("공 데이터에 오류가 발생했습니다.");
            return;
        }

        for (int i = 0; i < _differenceOfScore; i++)
        {
            ScoreManager.Instance.PlusScore(GetTurnBall());
        }

        currentTurn++;
        if(currentTurn >= GameManager.Instance.gamePlayers.Count)
        {
            currentTurn = 0;
        }

        GameManager.Instance.joystick.GetComponent<BallLineRender>().ResetBallStatus();
        UIManager.Instance.UpdateTurn(currentTurn);
        if (TCP_BallCore.networkMode == NetworkMode.None)
        {
            GameManager.Instance.SoloPlaySet(currentTurn);
        }

        GuestReplayer.ReplayTurn(GameManager.Instance.ballMoveData);
    }

    //순서2 : 턴에 해당하는 공을 반환한다.
    public GameObject GetTurnBall()
    {
        if(ballList == null)
        {
            return null;
        }
        if(currentTurn >= ballList.Count)
        {
            int tempTurn = currentTurn;
            while(tempTurn >= ballList.Count)
            {
                tempTurn -= ballList.Count;
            }
            return ballList[tempTurn];
        }
        return ballList[currentTurn];
    }

    public GameObject GetTurnBall(int turn)
    {
        if(ballList == null)
        {
            return null;
        }
        return ballList[turn];
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void GetListFromGameManager()
    {
        ballList = GameManager.Instance.gamePlayers;

        //if (GetTurnBall().name != GameManager.Instance.myID)
        //{
        //    GameManager.Instance.joystick.gameObject.SetActive(false);
        //}
        //else
        //{
        //    GameManager.Instance.joystick.gameObject.SetActive(true);
        //}
    }
}
