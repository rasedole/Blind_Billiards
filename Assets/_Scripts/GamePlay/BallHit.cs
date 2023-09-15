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
            _moveData.ballIndex = GameManager.Instance.GetIndexOfBall(name);
            if(TCP_BallCore.networkMode == NetworkMode.Client)
            {
                _moveData.startTime = Time.time - GameManager.Instance.shootTime;
                Debug.LogError("Client Time is : " + Time.time + "  And ShootTime is " + GameManager.Instance.shootTime);
            }
            else
            {
                _moveData.startTime = Time.time - GameManager.Instance.shootTime;
                Debug.LogError("Server Time is : " + Time.time + "  And ShootTime is " + GameManager.Instance.shootTime);
            }


            return _moveData;
        }
    }

    private MoveData _moveData;

    void Start()
    {
        _moveData = new MoveData();
        _moveData.startPos = gameObject.transform.position;

        if (GetComponent<BallMove>().isMove)
        {
            _moveData.startPos = transform.position;
        }
        _moveData.ballIndex = GameManager.Instance.GetIndexOfBall(this.name);
        _moveData.startTime = Time.time - GameManager.Instance.shootTime;
    }

    //충돌이 발생할 시 MoveDate구조체를 게임데이터에게 쌓는다.
    private void OnCollisionEnter(Collision collision)
    {
        if (!GameManager.Instance.isNobodyMove && !GuestReplayer.replaying)
        {
            if (TCP_BallCore.networkMode == NetworkMode.Server)
            {
                TCP_BallServer.Moved(moveData);
                GameManager.Instance.AddMoveData(moveData);
            }
            if(TCP_BallCore.networkMode != NetworkMode.Client)
            {
                GetComponent<BallDoll>().CollisionEvent();

                if(collision.gameObject.tag is "Player")
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
