using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuestReplayer : MonoBehaviour
{
    public static bool replaying
    {
        get
        {
            if(instance == null)
            {
                return false;
            }
            return instance._replaying;
        }
    }

    private static GuestReplayer instance;

    public List<BallDoll> balls;

    private List<MoveData> moveDatas;
    private bool _replaying;
    private Dictionary<int, List<ReplayData>> replaySets;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one GuestReplayer!");
            Destroy(gameObject);
            return;
        }
        _replaying = false;
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_replaying)
        {
            return;
        }

        // Replay
        List<int> ballMoveEndsIndexList = new List<int>();
        foreach (int index in replaySets.Keys)
        {
            ReplayData nowReplayData = replaySets[index][0];
            nowReplayData.progress += Time.deltaTime;

            if(nowReplayData.progress > nowReplayData.term)
            {
                balls[index].transform.position = replaySets[index][0].endPos;
                replaySets[index].RemoveAt(0);

                // Find next
                if (replaySets[index].Count > 0)
                {
                    float offset = nowReplayData.progress - nowReplayData.term;
                    nowReplayData = replaySets[index][0];
                    nowReplayData.progress = offset;
                    balls[index].CollisionEvent();
                }
                else
                {
                    // End of one ball move
                    balls[index].transform.position = nowReplayData.endPos;
                    ballMoveEndsIndexList.Add(index);
                    continue;
                }
            }

            // Move ball
            balls[index].transform.position = Vector3.Lerp(nowReplayData.startPos, nowReplayData.endPos, (nowReplayData.progress / nowReplayData.term));
            replaySets[index][0] = nowReplayData;
        }

        // Check ball move ends
        foreach(int index in ballMoveEndsIndexList)
        {
            replaySets.Remove(index);
        }
        if (replaySets.Keys.Count < 1)
        {
            // Replay ends
            _replaying = false;
            GameManager.Instance.isAlreadyShoot = false;

            GameManager.Instance.joystick.GetComponent<BallLineRender>().ResetBallStatus();



            GameManager.Instance.ClearMoveData();
            Debug.LogError(GameManager.Instance.ballMoveData.Count + "BallMoveCount");

            Debug.LogError("Replay End");

            GameManager.Instance.InitSetting();

            Debug.LogError("CurrentTurn: " + TurnManager.Instance.currentTurn);
            Debug.LogError("TurnEnd InitSetting");
        }
    }

    public static void ReplayTurn(List<MoveData> _moveDatas)
    {
        Debug.LogError("ReplayTurn");
        if (instance == null)
        {
            Debug.LogError("There is no GuestReplayer!");
            return;
        }

        instance.moveDatas = _moveDatas;
        instance.StartReplay();
    }

    private void StartReplay()
    {
        //Debug.LogError("StartReplay");
        //for (int i = 0; i < moveDatas.Count; i++)
        //{
        //    if (moveDatas[i].index != i)
        //    {
        //        Debug.LogError("MoveData index error!");
        //        return;
        //    }
        //}

        // --Translate MoveData to ReplayData--

        // Sort ball index
        Dictionary<int, List<MoveData>> sortMoves = new Dictionary<int, List<MoveData>>();
        foreach (MoveData moveData in moveDatas)
        {
            if (!sortMoves.ContainsKey(moveData.ballIndex))
            {
                sortMoves.Add(moveData.ballIndex, new List<MoveData>());
            }
            sortMoves[moveData.ballIndex].Add(moveData);
        }

        // Initialize and clear ReplayDatas
        if (replaySets == null)
        {
            replaySets = new Dictionary<int, List<ReplayData>>();
        }
        replaySets.Clear();

        // Translate each ball MoveData
        foreach (int ballIndex in sortMoves.Keys)
        {
            if (!replaySets.ContainsKey(ballIndex))
            {
                replaySets.Add(ballIndex, new List<ReplayData>());
            }

            float lastGameTime = 0;
            while (sortMoves[ballIndex].Count > 0)
            {
                ReplayData lastReplayData;
                ReplayData nowReplayData;
                nowReplayData = new ReplayData();
                nowReplayData.progress = 0;

                if (replaySets[ballIndex].Count < 1)
                {
                    if (sortMoves[ballIndex][0].startTime > 0)
                    {
                        // Not move at start
                        nowReplayData.startPos = sortMoves[ballIndex][0].startPos;
                        nowReplayData.endPos = sortMoves[ballIndex][0].startPos;
                        nowReplayData.term = sortMoves[ballIndex][0].startTime;
                        nowReplayData = new ReplayData();
                    }
                    else
                    {
                        // Move at start
                        balls[ballIndex].CollisionEvent();
                    }

                    nowReplayData.startPos = sortMoves[ballIndex][0].startPos;
                    lastGameTime = sortMoves[ballIndex][0].startTime;
                    replaySets[ballIndex].Add(nowReplayData);
                    sortMoves[ballIndex].RemoveAt(0);
                    continue;
                }

                // First MoveData will skip this
                lastReplayData = replaySets[ballIndex][replaySets[ballIndex].Count - 1];

                lastReplayData.endPos = sortMoves[ballIndex][0].startPos;
                lastReplayData.term = sortMoves[ballIndex][0].startTime - lastGameTime;

                replaySets[ballIndex][replaySets[ballIndex].Count - 1] = lastReplayData;

                nowReplayData.startPos = sortMoves[ballIndex][0].startPos;
                lastGameTime = sortMoves[ballIndex][0].startTime;
                replaySets[ballIndex].Add(nowReplayData);

                sortMoves[ballIndex].RemoveAt(0);
            }

            // Delete last dummy
            if(replaySets[ballIndex].Count > 1)
            {
                replaySets[ballIndex].RemoveAt(replaySets[ballIndex].Count - 1);
            }
            else
            {
                ReplayData justTouch = replaySets[ballIndex][0];
                justTouch.endPos = justTouch.startPos;
                justTouch.term = 0;
                replaySets[ballIndex][0] = justTouch;
            }
        }

        // Replay
        _replaying = true;
    }
}

public struct ReplayData
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float term;
    public float progress;
}