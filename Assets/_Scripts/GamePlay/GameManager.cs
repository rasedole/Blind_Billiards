using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//솔로 상태라면 배치된 게임 오브젝트를 찾아서 플레이어 리스트로 만든다.
//서버, 클라이언트 상태라면 서버에 연결된 플레이어를 추가한다.

public class GameManager : MonoBehaviour
{
    //싱글톤을 이용해서 쉽게 사용할 수 있도록 함
    public static GameManager Instance;

    public List<GameObject> gamePlayers;
    public List<BallEntryPlayerData> entryPlayerDataList;

    //속성추가 : 공을 발사한 이후부터 공이 모두 멈추고 턴을 넘겨주는 사이에도 조작할 수 없도록 하기 위해서 bool 변수로 isNobodyMove 선언
    public bool isNobodyMove = true;

    //속성3 : MoveData List, 공을 발사했을 때의 시간
    public List<MoveData> ballMoveData;
    [HideInInspector] public float shootTime;

    //속성5 : 색상 리스트
    public List<Color> ballColors;

    [SerializeField] GameObject colorBallPrefab;
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        //if(TCP_BallCore.networkMode == NetworkMode.None)
        //{
            //gamePlayers = GameObject.FindGameObjectsWithTag("Player");
        //}
    }

    private void Start()
    {
        //if(TCP_BallCore.networkMode == NetworkMode.None)
        //{
        //    for (int i = 0; i < gamePlayers.Count; i++)
        //    {
        //        if (i == 0)
        //        {
        //            gamePlayers[i].GetComponent<BallDoll>().showcaseColor = ballColors[i];
        //            gamePlayers[i].GetComponent<BallDoll>().Init(ballColors[i], BallShowMode.MyPlayer);
        //        }
        //        else
        //        {
        //            gamePlayers[i].GetComponent<BallDoll>().showcaseColor = ballColors[i];
        //            gamePlayers[i].GetComponent<BallDoll>().Init(ballColors[i], BallShowMode.OtherPlayer);
        //        }
        //    }
        //}
    }

    public IEnumerator CheckMovement(float time = 0)
    {
        yield return new WaitForSeconds(time);
        foreach(var ball in gamePlayers)
        {
            isNobodyMove = true;
            if (ball.GetComponent<BallMove>().isMove == true)
            {
                isNobodyMove = false;
                break;
            }
        }
        if (isNobodyMove)
        {
            TurnManager.Instance.EndTurn();
        }
    }

    public void SoloPlaySet(int turn)
    {
        for (int i = 0; i < gamePlayers.Count; i++)
        {
            BallDoll ballDollGO = gamePlayers[i].GetComponent<BallDoll>();
            if (i == turn)
            {
                ballDollGO.Init(ballDollGO.showcaseColor, BallShowMode.MyPlayer);
            }
            else
            {
                ballDollGO.Init(ballDollGO.showcaseColor, BallShowMode.OtherPlayer);
            }
        }
    }

    public void AddPlayerData(string playerID)
    {
        foreach(var playerData in entryPlayerDataList)
        {
            if (playerData.id == playerID)
                Debug.Log("이미 존재하는 플레이어를 추가하려고 했습니다.");
                return;
        }

        BallEntryPlayerData data = new BallEntryPlayerData();
        int randomNum = Random.Range(0, ballColors.Count);

        data.color = ballColors[randomNum];
        data.id = playerID;
        data.score = 0;
        data.index = entryPlayerDataList.Count;

        ballColors.Remove(ballColors[randomNum]);

        entryPlayerDataList.Add(data);
    }

    public void MakeBallByData()
    {
        //셔플
        for (int i = 0; i < entryPlayerDataList.Count; i++)
        {
            int randomNum = Random.Range(0, entryPlayerDataList.Count);
            BallEntryPlayerData tempData = entryPlayerDataList[i];
            entryPlayerDataList[i] = entryPlayerDataList[randomNum];
            entryPlayerDataList[randomNum] = tempData;
        }

        //셔플한 순서 index에 입력
        for(int i = 0; i<entryPlayerDataList.Count; i++)
        {
            entryPlayerDataList[i].index = i;
        }

        foreach (var playerData in entryPlayerDataList)
        {
            GameObject ballGO = Instantiate(colorBallPrefab);
            ballGO.transform.position = spawnPoints[playerData.index].position;
            ballGO.GetComponent<BallDoll>().Init(playerData.color, BallShowMode.OtherPlayer);
            gamePlayers.Add(ballGO);
        }

        TurnManager.Instance.GetListFromGameManager();

        for(int i = 0; i < gamePlayers.Count; i++)
        {
            gamePlayers[i].GetComponent<BallLineRender>().ResetBallStatus();
        }
    }
}
