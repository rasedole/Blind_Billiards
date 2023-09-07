using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//목적1 : 조이스틱을 당겼다가 놓으면 당긴 반대 방향으로 공이 날아가도록 한다.
//속성1 : 조이스틱, 날아갈 방향, 공을 날리기 위한 공의 RigidBody, 날리는 힘
//속성추가 : 조이스틱을 놓는 순간 이벤트가 발생하는데 그러면 조이스틱의 vertical, horizontal 값이 0이 되기에 조이스틱을 놓기 직전 값을 기준으로 날아가기 위해서 tempH, tempV 사용;
//순서1-1. 조이스틱을 당긴 값을 저장한다.
//순서1-2. 조이스틱을 놓는다.
//순서1-3. 조이스틱을 놓기 직전 값을 기준으로 방향을 정한다.
//순서1-4. 해당 방향을 향해 날리는 힘을 곱해서 힘을 가한다.

//목적3 : 게임매니저에 현재 턴을 지정하고 자신의 턴이 아닐 때는 작동하지 않도록 한다.
//속성3 : 내가 조작할 수 있는 턴
//속성추가 : 자신의 턴 중에도 한번 공을 발사한 후 멈추기 전까지는 추가로 발사하지 못하도록 하기위해 isMove bool변수를 사용
//리플레이 실행중에 실행되지 않도록 ReplayTest.replaying을 함께 사용
//순서3-1. 내 턴이 아니면 return한다.

//목적4 : 공이 충돌할 때 마다 자신의 MoveData를 게임 매니저에게 전달한다.
//속성4 : MoveData

//목적5 : 자신의 턴인 플레이어의 공이 다른 공과 부딫히면 스코어를 올린다.
//속성5 : 스코어
//순서5-1. 자신의 턴인 공이 다른 공과 부딫힌다.
//순서5-2. 점수를 올린다.

//속성추가 : 네트워크 연결 bool 변수

public class BallMove : MonoBehaviour
{
    //공이 움직임을 체크해서 멈춘 순간에 MoveData를 전송하기 위한 변수
    public bool isMove = false;

    void Start()
    {

    }

    void FixedUpdate()
    {
        //if (rigidbody.velocity.magnitude > 0.04f)
        {
            isMove = true;
        }

        if (isMove)
        {
            //if (rigidbody.velocity.magnitude < 0.04f && rigidbody.velocity.magnitude != 0)
            {
            //    rigidbody.drag = 0.7f;
            }
            //else if (rigidbody.velocity.magnitude < 1f)
            {
            //    rigidbody.drag = 0.3f;
            }

            //if (rigidbody.velocity.magnitude < 0.04f)
            {
                if (!GuestReplayer.replaying)
                {
                    //GameManager.Instance.ballMoveData.Add(moveData);
                }
                isMove = false;
            }
        }

        else
        {
        //    rigidbody.drag = 0.1f;
        }

        //순서3-1. 내 턴이 아니면 return한다.
        //if (GameManager.Instance.turn != myTurn || !GameManager.Instance.isNobodyMove || GuestReplayer.replaying)
        {
            return;
        }

        //연결이 되어 있다면 작동한다.
        //if (isConnected)
        {
            //순서6-1. 조이스틱을 당긴다.
            //순서1-1. 조이스틱을 당긴 값을 저장한다.

            //조이스틱을 놓는 순간 h와 v값이 0이 되기에 두 값이 0이 아닐 경우에만 temph, tempv를 갱신한다


            //순서1-2. 조이스틱을 놓는다.     
            //순서1-3. 조이스틱을 놓기 직전 값을 기준으로 방향을 정한다.
            //VariableJoyStick에 구현되어 있는 OnPointerUp이 조이스틱을 떼는 순간에 작동하기에 그 함수가 실행될 때 하단에 있는 Shoot()함수를 실행시킴으로서 공을 발사한다.

        }
        //연결이 끊어졌다면 초기화시키고 턴을 종료한다.
        //else
        {
            //lineRenderer.SetPosition(0, transform.position);
            //lineRenderer.SetPosition(1, transform.position);
            GameManager.Instance.ReLoadTurnTable();
            //direction = Vector3.zero;
            GameManager.Instance.Shoot();
        }
    }
}
