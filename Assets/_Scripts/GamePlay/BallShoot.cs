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
        if (TCP_BallCore.networkMode != NetworkMode.Client)
        {
            if (GameManager.Instance.isNobodyMove)
            {
                if(GameManager.Instance.CheckMyBall() || TCP_BallCore.networkMode == NetworkMode.None)
                {
                    GameManager.Instance.shootTime = Time.time;
                    Debug.Log("Shoot!");
                    TurnManager.Instance.GetTurnBall().GetComponent<Rigidbody>().AddForce(_direction * power, ForceMode.Impulse);
                    GameManager.Instance.isNobodyMove = false;

                    if (TCP_BallCore.networkMode == NetworkMode.Server)
                    {
                        foreach (var balls in GameManager.Instance.gamePlayers)
                        {
                            GameManager.Instance.AddMoveData(balls.GetComponent<BallHit>().moveData);
                        }
                    }
                    StartCoroutine(GameManager.Instance.CheckMovement(1));

                }
            }
        }
        else
        {
            //서버에게 direction값을 보낸다.
        }
    }

    //서버용 슛
    //public void Shoot(Vector3 clientDirection)
    //{
    //    if (TCP_BallCore.networkMode != NetworkMode.Server)
    //    {
    //        return;
    //    }

    //    if (GameManager.Instance.isNobodyMove)
    //    {
    //        GameManager.Instance.shootTime = Time.time;
    //        Debug.Log("Shoot!");
    //        TurnManager.Instance.GetTurnBall().GetComponent<Rigidbody>().AddForce(clientDirection * power, ForceMode.Impulse);
    //        GameManager.Instance.isNobodyMove = false;

    //        foreach (var balls in GameManager.Instance.gamePlayers)
    //        {
    //            GameManager.Instance.ballMoveData.Add(balls.GetComponent<BallHit>().moveData);
    //        }

    //        StartCoroutine(GameManager.Instance.CheckMovement(1));
    //    }
    //}

    public void ShootBall(Vector3 clientDirection)
    {
        Debug.Log("Test Ball Shoot");
        //if (TCP_BallCore.networkMode != NetworkMode.Server)
        //{
        //    return;
        //}

        if (GameManager.Instance.isNobodyMove)
        {
            GameManager.Instance.shootTime = Time.time;
            //Debug.Log(TurnManager.Instance.GetTurnBall().name);
            TurnManager.Instance.GetTurnBall().GetComponent<Rigidbody>().AddForce(clientDirection * power, ForceMode.Impulse);
            //TurnManager.Instance.GetTurnBall().GetComponent<Rigidbody>().AddForce(clientDirection * power, ForceMode.Impulse);
            GameManager.Instance.isNobodyMove = false;


            //for(int i = 0; i < GameManager.Instance.gamePlayers.Count; i++)
            //{
            //    //Debug.LogWarning(i);
            //    GameManager.Instance.AddMoveData(GetComponent<BallHit>().moveData);
            //}
            foreach (var balls in GameManager.Instance.gamePlayers)
            {

                GameManager.Instance.AddMoveData(balls.GetComponent<BallHit>().moveData);
            }

            StartCoroutine(GameManager.Instance.CheckMovement(1));
        }
    }
}
