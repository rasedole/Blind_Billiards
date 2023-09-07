using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

//목적1 : 공이 다른 오브젝트와 충돌할 경우 공의 이동 정보를 저장한다.
//속성1 : 공의 이동 정보

public class BallHit : MonoBehaviour
{
    //속성1 : 공의 이동 정보
    public MoveData moveData
    {
        get
        {
            if (true)
            {
                _moveData.startPos = transform.position;
            }
            _moveData.ballIndex = 1;
            _moveData.startTime = Time.time - GameManager.Instance.shootTime;

            return _moveData;
        }
    }
    private MoveData _moveData = new();

    // Start is called before the first frame update
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
                GameManager.Instance.ballMoveData.Add(moveData);

                GetComponent<BallDoll>().CollisionEvent();
            }
        }

        if (collision.gameObject.tag is "Player")
        {
            if (GameManager.Instance.turn == 1)
            {
                //score++;

                GameManager.Instance.UpdateScore();
            }
        }
    }
}
