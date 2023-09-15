using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Cient라면 업데이트 값 사용X
public class BallMove : MonoBehaviour
{
    //공이 움직임을 체크해서 멈춘 순간에 MoveData를 전송하기 위한 변수
    public bool isMove = false;

    Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(TCP_BallCore.networkMode == NetworkMode.Client)
        {
            return;
        }

        if (_rigidbody.velocity.magnitude > 0.04f)
        {
            isMove = true;
        }

        if (isMove)
        {
            if (_rigidbody.velocity.magnitude < 0.04f && _rigidbody.velocity.magnitude != 0)
            {
                _rigidbody.drag = 0.7f;
            }
            else if (_rigidbody.velocity.magnitude < 1f)
            {
                _rigidbody.drag = 0.3f;
            }

            if (_rigidbody.velocity.magnitude < 0.01f)
            {
                if(TCP_BallCore.networkMode == NetworkMode.Server && !GuestReplayer.replaying)
                {
                    GameManager.Instance.AddMoveData(GetComponent<BallHit>().moveData);
                    TCP_BallServer.Moved(GameManager.Instance.ballMoveData[GameManager.Instance.ballMoveData.Count - 1]);
                }
                isMove = false;
                StartCoroutine(GameManager.Instance.CheckMovement());
            }
        }
        else
        {
            _rigidbody.drag = 0.1f;
        }
    }
}
