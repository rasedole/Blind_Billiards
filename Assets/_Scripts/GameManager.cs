using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//목적1 : 게임에 턴을 지정해서 자신의 턴인 플레이어만 조작을 할 수 있도록 한다.
//속성1 : 현재 턴, 게임플레이어 오브젝트 집합
//속성추가 : 공을 발사한 이후부터 공이 모두 멈추고 턴을 넘겨주는 사이에도 조작할 수 없도록 하기 위해서 bool 변수로 isNobodyMove 선언
//순서1-1. 게임 플레이어 오브젝트를 모두 찾아서 집합에 넣는다.
//순서1-2. 집합에 순서에 따라 각 플레이어에게 턴을 배정한다.
//순서1-3. 조이스틱에서 Shoot()을 호출했을 때 움직이는 공이 없다면 현재 플레이어에 해당하는 공의 Shoot()을 실행한다.
//순서1-4. Shoot()을 실행한 이후에 모든 공이 멈추면 isNobodyMove값을 통해 멈췄음을 알려준다.

//목적2 : 게임에 턴과 각 턴에 해당하는 플레이어를 UI로 출력한다.
//속성2 : 현재 턴 UI, 턴 플레이어 UI
//순서2-1. 게임 시작할 때 각 턴에 해당하는 플레이어와 현재 턴을 넣어준다.
//순서2-2. 턴이 바뀌면 현재 턴의 값을 바꿔준다.

//목적3 : 호스트에 의해서 실행된 움직임을 저장하고 게스트에게 해당 값을 보내서 리플레이 시킨다.
//속성3 : MoveData List, 공을 발사했을 때의 시간

public class GameManager : MonoBehaviour
{
    //싱글톤을 이용해서 쉽게 사용할 수 있도록 함
    public static GameManager Instance;

    //속성1 : 현재 턴, 게임플레이어 오브젝트 집합
    public int turn;
    protected GameObject[] gamePlayers;

    //속성추가 : 공을 발사한 이후부터 공이 모두 멈추고 턴을 넘겨주는 사이에도 조작할 수 없도록 하기 위해서 bool 변수로 isNobodyMove 선언
    public bool isNobodyMove = true;

    //속성2 : 현재 턴 UI, 턴 플레이어 UI
    public TMP_Text currentTurn;
    public TMP_Text turnTable;

    //속성3 : MoveData List, 공을 발사했을 때의 시간
    public List<MoveData>ballMoveData;
    public float shootTime;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        turn = 0;
        //순서1-1. 게임 플레이어 오브젝트를 모두 찾아서 집합에 넣는다.
        gamePlayers = GameObject.FindGameObjectsWithTag("Player");

        //순서1-2. 집합에 순서에 따라 각 플레이어에게 턴을 배정한다.
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            gamePlayers[i].GetComponent<BallMove>().myTurn = i;
        }
    }

    private void Start()
    {
        //순서2-1. 게임 시작할 때 각 턴에 해당하는 플레이어와 현재 턴을 넣어준다.
        currentTurn.text = "Turn" + turn.ToString();
        turnTable.text = "Turn 0 : " + gamePlayers[0].name + "\n" + "Turn 1 : " + gamePlayers[1].name + "\n" + "Turn 2 : " + gamePlayers[2].name;
    }

    public void Shoot()
    {
        //순서1-3. 조이스틱에서 Shoot()을 호출했을 때 움직이는 공이 없다면 현재 플레이어에 해당하는 공의 Shoot()을 실행한다.
        if (!GuestReplayer.replaying)
        {
            if (isNobodyMove)
            {
                shootTime = Time.time;

                ballMoveData.Clear();
                for(int i = 0; i < gamePlayers.Length; i++)
                {
                    ballMoveData.Add(gamePlayers[i].GetComponent<BallMove>().moveData);
                }

                gamePlayers[turn].GetComponent<BallMove>().Shoot();
                //순서1-4. Shoot()을 실행한 이후에 모든 공이 멈추면 코루틴을 통해서 isNobodyMove값을 통해 멈췄음을 알려준다.
                StartCoroutine(EndTurn());
            }
            return;
        }
    }

    //순서1-4. Shoot()을 실행한 이후에 모든 공이 멈추면 isNobodyMove값을 통해 멈췄음을 알려준다.
    IEnumerator EndTurn()
    {
        //이 코루틴은 공을 쏜 후에 실행되므로 isNobodyMove를 false로 설정한다.
        isNobodyMove = false;

        //모든 공이 멈출때 까지 반복
        while(!isNobodyMove)
        {
            yield return new WaitForSeconds(1f);
            //모든 공의 속도를 측정해서 0.05보다 작다면 isNobodyMove에 true값을 그대로 넣고 속도가 0.05보다 큰 공이 있다면 isNobodyMove에 false를 넣고 break를 통해 반복문을 나온다.
            for (int i = 0; i < gamePlayers.Length; i++)
            {
                isNobodyMove = true;
                if (gamePlayers[i].GetComponent<Rigidbody>().velocity.magnitude > 0.05f)
                {
                    //Debug.Log(gamePlayers[i].gameObject.name + gamePlayers[i].GetComponent<Rigidbody>().velocity.magnitude);
                    isNobodyMove = false;
                    break;
                }
            }
        }

        //모든 공이 움직임을 멈추고 반복문이 종료한 이후에 턴을 진행시킨다.
        if(turn < gamePlayers.Length-1)
        {
            turn++;
        }
        else
        {
            turn = 0;
        }
        
        //순서2-2. 턴이 바뀌면 현재 턴의 값을 바꿔준다.
        currentTurn.text = "Turn" + turn.ToString();
    }
}
