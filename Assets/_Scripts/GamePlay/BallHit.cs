using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

//목적1 : 서버나 솔로상태라면 공이 다른 오브젝트와 충돌할 경우 공의 이동 정보를 저장한다.
//속성1 : 공의 이동 정보

public class BallHit : MonoBehaviour
{
    //속성1 : 공의 이동 정보
    public MoveData moveData
    {
        get
        {
            if (GetComponent<BallMove>().isMove)
            {
                _moveData.startPos = transform.position;
            }
            _moveData.ballIndex = ballID;
            _moveData.startTime = Time.time - GameManager.Instance.shootTime;

            return _moveData;
        }
    }
    private MoveData _moveData = new();

    public int ballID;

    void Start()
    {
        _moveData.startPos = gameObject.transform.position;
    }

    //충돌이 발생할 시 MoveDate구조체를 게임데이터에게 쌓는다.
    private void OnCollisionEnter(Collision collision)
    {
        if (!GameManager.Instance.isNobodyMove)
        {
            if (!GuestReplayer.replaying)
            {
                if (TCP_BallCore.networkMode == NetworkMode.Server)
                {
                    GameManager.Instance.ballMoveData.Add(moveData);

                }
                if(TCP_BallCore.networkMode != NetworkMode.Client)
                {
                    GetComponent<BallDoll>().CollisionEvent();

                    if(collision.gameObject.tag == "Player")
                    {
                        if(TurnManager.Instance.GetTurnBall() == this.gameObject)
                        {
                            ScoreManager.Instance.PlusScore(this.gameObject);
                        }
                    }
                }
            }
        }
    }
}
