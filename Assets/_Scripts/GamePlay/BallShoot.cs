using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//목적1 : 조이스틱의 입력에 따라 원하는 방향으로 원하는 힘으로 공을 발사한다.
//속성1 : 조이스틱, h, v, tempV, tempH, Power, 방향

public class BallShoot : MonoBehaviour
{
    //속성1 : 조이스틱, h, v, tempV, tempH, Power, 방향
    [SerializeField] float power = 50;

    VariableJoystick joystick;
    float tempV = 0;
    float tempH = 0;

    Vector3 direction;

    private void Start()
    {
        joystick = GetComponent<VariableJoystick>();
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

        direction = Vector3.left * tempH + Vector3.back * tempV;
    }

    public void Shoot()
    {
       TurnManager.Instance.GetTurnBall().GetComponent<Rigidbody>().AddForce(direction * power, ForceMode.Impulse);
    }
}
