using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//목적1 : 게스트로부터 공의 발사정보를 받아들여 계산을 하고 계산 결과에 따른 위치정보, 속도, 시간과 같은 정보를 게스트에게 보낸다.
//속성1 : 발사 방향, 발사 속도, 발사 위치, 발사 시간, 발사 후 충돌 위치 정보, 속도, 시간

public class HostCalculation : MonoBehaviour
{
    //속성1 : 발사 방향, 발사 속도, 발사 위치, 발사 시간, 발사 후 충돌 위치 정보, 속도, 시간
    protected Vector3 guestDirection;
    protected float guestPower;
    protected Vector3 guestPosition;
    protected float guestTime;
    protected List<Vector3> collisionPosition;
    protected List<Vector3> collisionSpeed;
    protected List<float> collisionTime;

    public void GetGuestInformation(Vector3 direction, float power, Vector3 position, float time, out List<Vector3> outPosition, out List<Vector3> outVelocity, out List<float> outTime)
    {
        Debug.Log("게스트 발사 정보 입력");
        guestDirection = direction;
        guestPower = power;
        guestPosition = position;
        guestTime = time;

        collisionPosition = new List<Vector3>();
        collisionSpeed = new List<Vector3>();
        collisionTime = new List<float>();

        collisionPosition.Add(position);
        collisionSpeed.Add(new Vector3(0, 0, 0f));
        collisionTime.Add(0f);

        outPosition = collisionPosition;
        outVelocity = collisionSpeed;
        outTime = collisionTime;
    }

}
