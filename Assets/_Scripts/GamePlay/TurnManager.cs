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
    public int gameTurn
    {
        get
        {
            return _gameTurn;
        }
        private set
        {
            _gameTurn = value;
        }
    }

    int _currentTurn;
    int _gameTurn;

    //속성2 : 플레이어 공 리스트
    [SerializeField] List<GameObject> ballList;

    private IEnumerator WaitTurnEnd()
    {

        bool trigger = true;
        while (trigger)
        {
            yield return new WaitForSeconds(1);
            Debug.LogWarning("Ending Check");
            if ((TCP_BallCore.networkMode == NetworkMode.Client && !GuestReplayer.replaying) || (TCP_BallCore.networkMode == NetworkMode.Server && TCP_BallCore.allClientTurnChecked && GameManager.Instance.serverShootEnable) || TCP_BallCore.networkMode == NetworkMode.None)
            {
                trigger = false;
                GameManager.Instance.GetComponent<TCP_BallUI>().GameEnd();
            }
        }

    }

    //순서1 : 턴을 다음 턴으로 넘긴다.
    public void EndTurn()
    {
        currentTurn++;
        if(currentTurn >= GameManager.Instance.gamePlayers.Count)
        {
            gameTurn++;
            Debug.LogWarning(gameTurn);
            Debug.LogWarning(GameManager.gameMaxTurn);
            currentTurn = 0;
        }
        if(gameTurn >= GameManager.gameMaxTurn)
        {
            Debug.LogWarning(gameTurn);
            StartCoroutine(WaitTurnEnd());
        }

        Debug.Log("CurrentTurn: " + currentTurn);
        GameManager.Instance.InitSetting();
    }

    //private List<MoveData> SortMoveData(List<MoveData> moveDatas)
    //{
    //    MoveData temp = new MoveData();
    //    List<MoveData> outList = new List<MoveData>();
    //    for(int i = 0; i < moveDatas.Count-1; i++)
    //    {
    //        for (int j = 0; j < moveDatas.Count-1-i; j++)
    //        {
    //            if (moveDatas[j].startTime > moveDatas[j + 1].startTime)
    //            {
    //                temp = moveDatas[j+1];
    //                moveDatas[j + 1] = moveDatas[j];
    //                moveDatas[j] = temp;
    //            }
    //        }
    //    }

    //    for(int i = 0; i < moveDatas.Count; i++)
    //    {
    //        MoveData inMoveData = new MoveData();
    //        inMoveData.index = i;
    //        inMoveData.startPos = moveDatas[i].startPos;
    //        inMoveData.startTime = moveDatas[i].startTime;
    //        inMoveData.ballIndex = moveDatas[i].ballIndex;
    //        outList.Add(inMoveData);
    //    }

    //    return outList;
    //}

    public void EndTurn(int _countOfMoveData, int _differenceOfScore)
    {
        //Debug.LogError("Call TurnEnd");

        //foreach (var data in GameManager.Instance.ballMoveData)
        //{
        //    Debug.LogError("Index : " + data.index + "Ball Time : " + data.startTime + "Ball Pos : " + data.startPos + "Ball Index : " + data.ballIndex);
        //}

        if (TCP_BallCore.networkMode == NetworkMode.Server)
        {
            EndTurn();
            return;
        }
        else
        {
            //Debug.LogError(_countOfMoveData);
            //Debug.LogError(GameManager.Instance.ballMoveData.Count);

            if (_countOfMoveData != GameManager.Instance.ballMoveData.Count)
            {
                Debug.LogError("공 데이터에 오류가 발생했습니다.");
                return;
            }

            ScoreManager.Instance.PlusScore(GetTurnBall(), _differenceOfScore);

            Debug.Log("Ball Move Data Sort");
            //GameManager.Instance.ballMoveData = SortMoveData(GameManager.Instance.ballMoveData);

            Debug.Log("TurnEnd-Replay");
            GuestReplayer.ReplayTurn(GameManager.Instance.ballMoveData);
            currentTurn++;
            Debug.Log("CurrentTurn: " + currentTurn);
            if (currentTurn >= GameManager.Instance.gamePlayers.Count)
            {
                gameTurn++;
                currentTurn = 0;
                Debug.Log("CurrentTurn: " + currentTurn);

                if ((TCP_BallCore.networkMode == NetworkMode.Client && !GuestReplayer.replaying) || (TCP_BallCore.networkMode == NetworkMode.Server && TCP_BallCore.allClientTurnChecked && GameManager.Instance.serverShootEnable) || TCP_BallCore.networkMode == NetworkMode.None)
                {
                    Debug.LogWarning(gameTurn);
                    StartCoroutine(WaitTurnEnd());
                }
            }
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
        _gameTurn = 0;
    }
}
