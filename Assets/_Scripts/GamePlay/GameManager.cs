using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//목적1 : 게임에 턴을 지정해서 자신의 턴인 플레이어만 조작을 할 수 있도록 한다.
//속성1 : 현재 턴, 게임플레이어 오브젝트 집합
//속성추가 : 공을 발사한 이후부터 공이 모두 멈추고 턴을 넘겨주는 사이에도 조작할 수 없도록 하기 위해서 bool 변수로 isNobodyMove 선언
//리플레이 실행중에 실행되지 않도록 ReplayTest.replaying을 함께 사용
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

//목적4 : 정렬된 스코어를 기준으로 플레이어와 점수를 표시한다
//속성4 : 스코어UI
//순서4-1. 게임 시작시 턴 순서에 따라서 0점으로 초기화된 점수를 표시한다
//순서4-2. 점수가 바뀌면 바뀐 점수를 순위에 따라서 표시한다..

//목적5 : 공의 색상을 랜덤하게 설정해준다.
//속성5 : 색상 리스트

//목적6 : 게임 진행 중 플레어의 연결이 끊어졌을 경우 해당 플레이어의 이름과 점수를 빨간색으로 변경하고 해당 플레이어의 턴을 자동으로 넘기도록 한다.

//목적7 : 플레이어 수에 따라서 스폰포인트에 생성한다.
//속성7 : 플레이어 스폰포인트, 플레이어 수

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
    public List<MoveData> ballMoveData;
    [HideInInspector] public float shootTime;

    //속성4 : 스코어UI
    public TMP_Text scoreUI;

    //속성5 : 색상 리스트
    //public List<Material> ballColors;
    public List<Color> ballColors;

    //속성7 : 플레이어 스폰포인트, 플레이어 수, 플레이어 프리팹
    public GameObject[] spawnPoints;
    public int playerNumber = 3;
    public GameObject playerPrefab;
    public VariableJoystick playerJoystick;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < playerNumber; i++)
        {
            GameObject playerGO = Instantiate(playerPrefab, spawnPoints[i].transform.position, Quaternion.identity);
            playerGO.name = "TestBall " + i;
            playerGO.GetComponent<BallMove>().joystick = playerJoystick;    
        }

        turn = 0;
        //순서1-1. 게임 플레이어 오브젝트를 모두 찾아서 집합에 넣는다.
        gamePlayers = GameObject.FindGameObjectsWithTag("Player");

        //게스트 리플레이어 balls에 정렬된 순서로 넣어주는 코드
        //GameObject.Find("GuestReplayer").GetComponent<GuestReplayer>().balls = gamePlayers;

        //순서1-2. 집합에 순서에 따라 각 플레이어에게 턴을 배정한다.
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            gamePlayers[i].GetComponent<BallMove>().myTurn = i;
            int randomColor = Random.Range(0, ballColors.Count);
            BallDoll ballDoll = gamePlayers[i].GetComponent<BallDoll>();
            ballDoll.showcaseColor = ballColors[randomColor];
            if (turn == gamePlayers[i].GetComponent<BallMove>().myTurn)
            {
                ballDoll.Init(ballDoll.showcaseColor, BallShowMode.MyPlayer);
            }
            else
            {
                ballDoll.Init(ballDoll.showcaseColor, BallShowMode.OtherPlayer);
                ballDoll.GetComponent<Animator>().Play("Hide");
            }
            ballColors.Remove(ballColors[randomColor]);
        }
    }

    private void Start()
    {
        //순서2-1. 게임 시작할 때 각 턴에 해당하는 플레이어와 현재 턴을 넣어준다.
        currentTurn.text = "Turn" + (turn + 1).ToString();
        turnTable.text = "";
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            turnTable.text += "Turn " + (i + 1) + " : " + gamePlayers[i].name + "\n";
        }

        //순서4-1. 게임 시작시 턴 순서에 따라서 0점으로 초기화된 점수를 표시한다
        scoreUI.text = "";
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            scoreUI.text += (i + 1).ToString() + ". " + gamePlayers[i].name + " " + gamePlayers[i].GetComponent<BallMove>().score + "\n";
        }
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
                for (int i = 0; i < gamePlayers.Length; i++)
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
    public IEnumerator EndTurn()
    {
        //이 코루틴은 공을 쏜 후에 실행되므로 isNobodyMove를 false로 설정한다.
        isNobodyMove = false;

        //모든 공이 멈출때 까지 반복
        while (!isNobodyMove)
        {
            yield return new WaitForSeconds(1f);
            //모든 공의 속도를 측정해서 0.05보다 작다면 isNobodyMove에 true값을 그대로 넣고 속도가 0.05보다 큰 공이 있다면 isNobodyMove에 false를 넣고 break를 통해 반복문을 나온다.
            for (int i = 0; i < gamePlayers.Length; i++)
            {
                isNobodyMove = true;
                if (gamePlayers[i].GetComponent<BallMove>().isMove)
                {
                    //Debug.Log(gamePlayers[i].gameObject.name + gamePlayers[i].GetComponent<Rigidbody>().velocity.magnitude);
                    isNobodyMove = false;
                    break;
                }
            }
        }

        //모든 공이 움직임을 멈추고 반복문이 종료한 이후에 턴을 진행시킨다.
        if (turn < gamePlayers.Length - 1)
        {
            turn++;
        }
        else
        {
            turn = 0;
        }

        for (int i = 0; i < gamePlayers.Length; i++)
        {
            BallDoll ballDoll = gamePlayers[i].GetComponent<BallDoll>();
            if (turn == gamePlayers[i].GetComponent<BallMove>().myTurn)
            {
                ballDoll.Init(ballDoll.showcaseColor, BallShowMode.MyPlayer);
            }
            else
            {
                ballDoll.Init(ballDoll.showcaseColor, BallShowMode.OtherPlayer);
                ballDoll.GetComponent<Animator>().Play("Hide");
            }
        }

        //순서2-2. 턴이 바뀌면 현재 턴의 값을 바꿔준다.
        currentTurn.text = "Turn" + (turn + 1).ToString();
    }

    //순서4-2. 점수가 바뀌면 바뀐 점수를 순위에 따라서 표시한다.
    public void UpdateScore()
    {
        //플레이어명과 스코어를 배열로 저장
        string[] playerName = new string[gamePlayers.Length];
        int[] scores = new int[gamePlayers.Length];
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            playerName[i] = gamePlayers[i].name;
            scores[i] = gamePlayers[i].GetComponent<BallMove>().score;
        }

        //배열을 점수 오름차순으로 정렬
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            for (int j = 0; j < gamePlayers.Length - (i + 1); j++)
            {
                if (scores[j] < scores[j + 1])
                {
                    int temp = scores[j + 1];
                    scores[j + 1] = scores[j];
                    scores[j] = temp;
                    string tempText = playerName[j + 1];
                    playerName[j + 1] = playerName[j];
                    playerName[j] = tempText;
                }
                else if (scores[j] == scores[j + 1])
                {
                    if (gamePlayers[j].GetComponent<BallMove>().myTurn > gamePlayers[j + 1].GetComponent<BallMove>().myTurn)
                    {
                        int temp = scores[j + 1];
                        scores[j + 1] = scores[j];
                        scores[j] = temp;
                        string tempText = playerName[j + 1];
                        playerName[j + 1] = playerName[j];
                        playerName[j] = tempText;
                    }
                }
            }
        }

        scoreUI.text = "";
        //배열을 순서대로 점수로 출력
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            scoreUI.text += (i + 1).ToString() + ". " + playerName[i] + " " + scores[i] + "\n";
        }

        ReLoadTurnTable();
    }

    public void ReLoadTurnTable()
    {
        turnTable.text = "";
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            if (!gamePlayers[i].GetComponent<BallMove>().isConnected)
            {
                turnTable.text += "<color=red>" + "Turn " + (i + 1) + " : " + gamePlayers[i].name + "\n";
            }
            else
            {
                turnTable.text += "<color=white>" + "Turn " + (i + 1) + " : " + gamePlayers[i].name + "\n";
            }
        }
    }
}
