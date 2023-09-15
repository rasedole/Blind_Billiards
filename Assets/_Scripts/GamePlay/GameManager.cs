using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//솔로 상태라면 배치된 게임 오브젝트를 찾아서 플레이어 리스트로 만든다.
//서버, 클라이언트 상태라면 서버에 연결된 플레이어를 추가한다.

public class GameManager : MonoBehaviour
{
    //싱글톤을 이용해서 쉽게 사용할 수 있도록 함
    public static GameManager Instance;

    public List<GameObject> gamePlayers;
    public List<BallEntryPlayerData> entryPlayerDataList = new();

    //속성추가 : 공을 발사한 이후부터 공이 모두 멈추고 턴을 넘겨주는 사이에도 조작할 수 없도록 하기 위해서 bool 변수로 isNobodyMove 선언
    public bool isNobodyMove = true;

    //Client용
    public bool isAlreadyShoot = false;

    //속성3 : MoveData List, 공을 발사했을 때의 시간
    public List<MoveData> ballMoveData;
    [HideInInspector] public float shootTime;

    //속성5 : 색상 리스트
    public List<Color> ballColors;

    [SerializeField] GameObject colorBallPrefab;
    public Transform[] spawnPoints;

    public GameObject gameObjects;

    public TMP_InputField maxPlayer;

    //###새로만든 UI에 있는 조이스틱으로 변경필요 코드
    public FixedJoystick joystick;

    public string myID = "Test";

    public GuestReplayer guestReplayer;

    public TMP_InputField inputID;

    public TMP_InputField chatInput;
    //public GameObject gameUI;

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

        //StartCoroutine(CheckClientsTurnEnd());
    }

    private IEnumerator CheckClientsTurnEnd()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            Debug.LogError(TCP_BallCore.allClientTurnChecked);
        }
    }

    //Client는 작동X
    public IEnumerator CheckMovement(float time = 0)
    {
        yield return new WaitForSeconds(time);
        if(TCP_BallCore.networkMode != NetworkMode.Client)
        {
            foreach (var ball in gamePlayers)
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
                if (TCP_BallCore.networkMode == NetworkMode.None)
                {
                    TurnManager.Instance.EndTurn();
                }
                else if (TCP_BallCore.networkMode == NetworkMode.Server)
                {
                    //TurnEnd에 현재 공의 점수로 바꿔야함
                    StartCoroutine(CheckTurnEnd());
                }
            }
        }
    }

    private IEnumerator CheckTurnEnd()
    {
        bool trigger = true;
        while (trigger)
        {
            yield return new WaitForSeconds(1f);
            if (TCP_BallCore.allClientTurnChecked)
            {
                foreach (var ball in entryPlayerDataList)
                {
                    if (ball.id == TurnManager.Instance.GetTurnBall().name)
                    {
                        TCP_BallServer.TurnEnd(ball.score - ScoreManager.Instance.savedScore);
                        Debug.LogError(ball.score + "BallScore - " + ScoreManager.Instance.savedScore + "PastScore");
                        trigger = false;
                        break;
                    }
                }
            }
        }
    }

    public void InitSetting()
    {
        for (int i = 0; i < gamePlayers.Count; i++)
        {
            BallDoll ballDollGO = gamePlayers[i].GetComponent<BallDoll>();
            if (TCP_BallCore.networkMode != NetworkMode.None)
            {
                if (gamePlayers[i].name == myID)
                {
                    ballDollGO.Init(ballDollGO.showcaseColor, BallShowMode.MyPlayer);
                }
                else
                {
                    ballDollGO.Init(ballDollGO.showcaseColor, BallShowMode.OtherPlayer);
                }
            }
            else
            {
                if (i == TurnManager.Instance.currentTurn)
                {
                    ballDollGO.Init(ballDollGO.showcaseColor, BallShowMode.MyPlayer);
                }
                else
                {
                    ballDollGO.Init(ballDollGO.showcaseColor, BallShowMode.OtherPlayer);
                    ballDollGO.CollisionEvent();
                }
            }
        }

        joystick.GetComponent<BallLineRender>().ResetBallStatus();

    }

    public void AddPlayerData(string playerID)
    {
        BallEntryPlayerData data = new();
        int randomNum = Random.Range(0, ballColors.Count);

        data.color = ballColors[randomNum];
        data.id = playerID;
        data.score = 0;
        if (entryPlayerDataList == null)
        {
            data.index = 0;
        }
        else
        {
            data.index = entryPlayerDataList.Count;
        }

        ballColors.Remove(ballColors[randomNum]);

        Debug.Log(data.id + "를 플레이어로 추가헀습니다.");
        entryPlayerDataList.Add(data);
    }

    public void RemovePlayerData(string playerID)
    {
        if (entryPlayerDataList != null)
        {
            foreach (var playerData in entryPlayerDataList)
            {
                if (playerData.id == playerID)
                {
                    entryPlayerDataList.Remove(playerData);
                    return;
                }
            }
            Debug.Log("There is no player data ID : " + playerID);
        }
        else
        {
            Debug.Log("PlayerData is empty");
        }
    }

    public void RemoveRoomPlayer(List<string> playerID)
    {
        if (entryPlayerDataList != null)
        {
            for (int i = 0; i < playerID.Count; i++)
            {
                foreach (var playerData in entryPlayerDataList)
                {
                    if (playerData.id == playerID[i])
                    {
                        entryPlayerDataList.Remove(playerData);
                        break;
                    }
                }
                Debug.Log("There is no player data ID : " + playerID[i]);
            }
        }
        else
        {
            Debug.Log("PlayerData is empty");
        }
    }

    public void MakeBallByData()
    {
        foreach (var playerData in entryPlayerDataList)
        {
            GameObject ballGO = Instantiate(colorBallPrefab);
            ballGO.transform.position = spawnPoints[playerData.index].position;
            ballGO.GetComponent<BallDoll>().Init(playerData.color, BallShowMode.OtherPlayer);
            ballGO.GetComponent<BallDoll>().showcaseColor = playerData.color;
            gamePlayers.Add(ballGO);
            ballGO.name = playerData.id;
        }

        if (TCP_BallCore.networkMode != NetworkMode.None)
        {
            foreach (var ball in gamePlayers)
            {
                guestReplayer.balls.Add(ball.GetComponent<BallDoll>());
            }
        }
    }

    public void StartGameSolo(int playerNumber)
    {
        MakeLocalPlayer(playerNumber);
        MakeBallByData();
        TurnManager.Instance.GetListFromGameManager();
        InitSetting();
        UI_InGame.MakeNew(entryPlayerDataList);
    }

    public void StartGameFromRoom()
    {
        MakeBallByData();
        TurnManager.Instance.GetListFromGameManager();
        InitSetting();
        UI_InGame.MakeNew(entryPlayerDataList);
    }

    public void StartGameFromRoomClient()
    {
        MakeBallByData();
        TurnManager.Instance.GetListFromGameManager();
        InitSetting();
        UI_InGame.MakeNew(entryPlayerDataList);
    }

    public void MakeLocalPlayer(int _playerNumber)
    {
        for (int i = 0; i < _playerNumber; i++)
        {
            int randomID = Random.Range(0, 999);
            foreach(var data in entryPlayerDataList)
            {
                if(("LocalPlayer" + randomID) == data.id)
                {
                    randomID += 1000;
                }
            }
            AddPlayerData("LocalPlayer" + randomID);
        }
    }

    public bool CheckMyBall()
    {
        if (TurnManager.Instance.GetTurnBall().name == myID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GetAllPlayerFromServer(List<BallEntryPlayerData> datas)
    {
        entryPlayerDataList = datas;
        if (TCP_BallUI.gameState == GameState.Room)
        {
            UI_RoomManager.MakeNew(datas);
        }
    }

    public void GetLastPlayerFromServer(BallEntryPlayerData data)
    {
        if (TCP_BallCore.networkMode != NetworkMode.Server)
        {
            entryPlayerDataList.Add(data);
        }
        if (TCP_BallUI.gameState == GameState.Room)
        {
            UI_RoomManager.MakeNew(new List<BallEntryPlayerData>() { data });
        }
    }

    public void SetID()
    {
        myID = inputID.text;
    }

    public void SetID(string _id)
    {
        myID = _id;
    }

    public void AddMoveData(MoveData _moveData)
    {
        if(_moveData.index == -1)
        {
            if (ballMoveData == null)
            {
                _moveData.index = 0;
            }
            else
            {
                _moveData.index = ballMoveData.Count;
            }
        }
        ballMoveData.Add(_moveData);
    }

    public int GetIndexOfBall(string id)
    {
        foreach (var ball in entryPlayerDataList)
        {
            if (ball.id == id)
            {
                return ball.index;
            }
        }
        Debug.LogError("There is no ball in data has ID " + id);
        return -1;
    }

    public void ClearPlayerData()
    {
        entryPlayerDataList.Clear();
    }

    public void ClearMoveData()
    {
        Debug.LogWarning("!");
        ballMoveData.Clear();
    }

    public void ClearBall()
    {
        foreach(var ball in gamePlayers)
        {
            Destroy(ball);
        }
    }

    public void EndGameSolo()
    {
        List<RankingResultUI.RankData> rankDatas = new();
        //RankingResultUI.RankData rankData = new();
        //RankingResultUI.RankShow(rankDatas);
    }

    public void EndGameServer()
    {
        //RankingResultUI.RankData rankData = new();
    }

    public void EndGameClient()
    {
        //RankingResultUI.RankData rankData = new();
    }
}
