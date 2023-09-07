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
    GameObject[] ballList;

    //순서1 : 턴을 다음 턴으로 넘긴다.
    public void EndTurn()
    {
        currentTurn++;
        GameObject.Find("VariableJoystick").GetComponent<BallLineRender>().ResetBallStatus();
    }

    //순서2 : 턴에 해당하는 공을 반환한다.
    public GameObject GetTurnBall()
    {
        return ballList[currentTurn];
    }
    public GameObject GetTurnBall(int turn)
    {
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
}
