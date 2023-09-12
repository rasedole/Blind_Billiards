using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Cient라면 업데이트 값 사용X
public class BallMove : MonoBehaviour
{
    //공이 움직임을 체크해서 멈춘 순간에 MoveData를 전송하기 위한 변수
    public bool isMove = false;

    new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(TCP_BallCore.networkMode == NetworkMode.Client)
        {
            return;
        }

        if (rigidbody.velocity.magnitude > 0.04f)
        {
            isMove = true;
        }

        if (isMove)
        {
            if (rigidbody.velocity.magnitude < 0.04f && rigidbody.velocity.magnitude != 0)
            {
                rigidbody.drag = 0.7f;
            }
            else if (rigidbody.velocity.magnitude < 1f)
            {
                rigidbody.drag = 0.3f;
            }

            if (rigidbody.velocity.magnitude < 0.01f)
            {
                if(TCP_BallCore.networkMode == NetworkMode.Server)
                {
                    if (!GuestReplayer.replaying)
                    {
                        TCP_BallServer.Moved(GetComponent<BallHit>().moveData);
                        GameManager.Instance.AddMoveData(GetComponent<BallHit>().moveData);
                    }
                }
                isMove = false;
                StartCoroutine(GameManager.Instance.CheckMovement());
            }
        }
        else
        {
            rigidbody.drag = 0.1f;
        }
    }
}
