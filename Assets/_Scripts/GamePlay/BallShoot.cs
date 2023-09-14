using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//목적1 : 조이스틱의 입력에 따라 원하는 방향으로 원하는 힘으로 공을 발사한다.
//속성1 : 조이스틱, h, v, tempV, tempH, Power, 방향

//서버나 솔로라면 슛 그대로
//클라이언트라면 슛 정보를 서버에게 보내도록 변경

public class BallShoot : MonoBehaviour
{
    //속성1 : 조이스틱, h, v, tempV, tempH, Power, 방향
    [SerializeField] float power = 50;

    public Vector3 direction
    {
        get
        {
            return _direction;
        }
    }

    FixedJoystick joystick;
    float tempV = 0;
    float tempH = 0;
    Vector3 _direction;

    private void Start()
    {
        joystick = GetComponent<FixedJoystick>();
    }

    private void Update()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        if (h != 0 || v != 0)
        {
            tempH = h;
            tempV = v;
        }

        _direction = Vector3.left * tempH + Vector3.back * tempV;
    }

    public void Shoot()
    {
        if(TCP_BallCore.networkMode == NetworkMode.None)
        {
            if (GameManager.Instance.isNobodyMove)
            {
                GameManager.Instance.shootTime = Time.time;
                TurnManager.Instance.GetTurnBall().GetComponent<Rigidbody>().AddForce(_direction * power, ForceMode.Impulse);
                GameManager.Instance.isNobodyMove = false;

                foreach(var balls in GameManager.Instance.gamePlayers)
                {
                    GameManager.Instance.AddMoveData(balls.GetComponent<BallHit>().moveData);
                }
            }
            GameManager.Instance.joystick.GetComponent<BallLineRender>().ResetLineRender();
            StartCoroutine(GameManager.Instance.CheckMovement(1));
        }
        else
        {
            if((TCP_BallCore.networkMode != NetworkMode.Client || !GameManager.Instance.isAlreadyShoot) && GameManager.Instance.isNobodyMove)
            {
                TCP_BallCore.ShootTheBall(direction);
                GameManager.Instance.isAlreadyShoot = true;
            }
        }
    }

    public void ShootBall(Vector3 clientDirection)
    {
        GameManager.Instance.ClearMoveData();

        GameManager.Instance.shootTime = Time.time;
        TurnManager.Instance.GetTurnBall().GetComponent<Rigidbody>().AddForce(clientDirection * power, ForceMode.Impulse);
        GameManager.Instance.isNobodyMove = false;

        foreach (var balls in GameManager.Instance.gamePlayers)
        {
            GameManager.Instance.AddMoveData(balls.GetComponent<BallHit>().moveData);
            TCP_BallServer.Moved(balls.GetComponent<BallHit>().moveData);
        }
        GameManager.Instance.joystick.GetComponent<BallLineRender>().ResetLineRender();
        StartCoroutine(GameManager.Instance.CheckMovement(1));
    }
}
