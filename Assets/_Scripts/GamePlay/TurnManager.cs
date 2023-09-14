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
        currentTurn++;
        if(currentTurn >= GameManager.Instance.gamePlayers.Count)
        {
            currentTurn = 0;
        }

        Debug.Log("CurrentTurn: " + currentTurn);
        GameManager.Instance.InitSetting();
        GameManager.Instance.ClearMoveData();
    }

    public void EndTurn(int _countOfMoveData, int _differenceOfScore)
    {
        Debug.LogError("Call TurnEnd");
        if(TCP_BallCore.networkMode == NetworkMode.Server)
        {
            EndTurn();
            return;
        }
        else
        {
            Debug.LogError(_countOfMoveData);
            Debug.LogError(GameManager.Instance.ballMoveData.Count);


            if (_countOfMoveData != GameManager.Instance.ballMoveData.Count)
            {
                Debug.LogError("공 데이터에 오류가 발생했습니다.");
                return;
            }

            for (int i = 0; i < _differenceOfScore; i++)
            {
                ScoreManager.Instance.PlusScore(GetTurnBall());
            }

            GuestReplayer.ReplayTurn(GameManager.Instance.ballMoveData);

            currentTurn++;
            if (currentTurn >= GameManager.Instance.gamePlayers.Count)
            {
                currentTurn = 0;
            }

            Debug.Log("CurrentTurn: " + currentTurn);
            GameManager.Instance.InitSetting();
        }
    }

    //순서2 : 턴에 해당하는 공을 반환한다.
    public GameObject GetTurnBall()
    {
        if(ballList == null)
        {
            Debug.LogError("BallList is Empty!");
            return null;
        }
        return ballList[currentTurn];
    }

    public GameObject GetTurnBall(int turn)
    {
        if(ballList == null)
        {
            Debug.LogError("BallList is Empty!");
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
    }
}
