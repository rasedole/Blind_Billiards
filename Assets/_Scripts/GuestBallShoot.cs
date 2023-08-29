using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//목적1 : 조이스틱을 당겼다가 놓으면 발사 정보를 호스트에게 보낸다.
//속성1 : 호스트, 조이스틱, 날아갈 방향, 날리는 힘, temph, tempv
//속성 추가: 호스트에게서 받아온 정보를 저장할 위치, 속도, 시간
//순서1-1. 조이스틱을 당긴 값을 저장한다.
//순서1-2. 조이스틱을 놓는다.
//순서1-3. 조이스틱을 놓기 직전 값을 기준으로 방향을 정한다.
//순서1-4. 발사 정보를 호스트에게 보낸다.

//목적2 : 조이스틱을 당길 때 날아가는 방향을 표시하는 선을 그린다.
//속성2 : 선을 그릴 LineRenderer
//순서2-1. 조이스틱을 당긴 값으로 공이 날아갈 방향을 구한다.
//순서2-2. 해당 방향을 LineRenderer에 넣어준다.

//목적3 : 게임매니저에 현재 턴을 지정하고 자신의 턴이 아닐 때는 작동하지 않도록 한다.
//속성3 : 내가 조작할 수 있는 턴
//순서3-1. 내 턴이 아니면 return한다.

public class GuestBallShoot : MonoBehaviour
{
    //속성1 : 호스트, 조이스틱, 날아갈 방향, 날리는 힘, temph, tempv
    public GameObject host;
    public VariableJoystick joystick;
    public Vector3 direction;
    public float power = 380;
    public float temph = 0;
    public float tempv = 0;
    //속성 추가: 호스트에게서 받아온 정보를 저장할 위치, 속도, 시간
    protected List<Vector3> positions;
    protected List<Vector3> velocities;
    protected List<float> times;

    //속성2 : 선을 그릴 LineRenderer
    protected LineRenderer lineRenderer;

    //속성3 : 내가 조작할 수 있는 턴
    public int myTurn;

    protected bool isMove = false;
    float currentTime = 0;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = GetComponent<MeshRenderer>().materials[0].GetColor("_Color");
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            transform.position = Vector3.Lerp(positions[counter], positions[counter + 1], (currentTime) / times[counter+1]);
            if (currentTime < times[counter])
            {
                currentTime += Time.deltaTime;
                Debug.Log(times[counter] + "보다 " +  currentTime + "이 작습니다.");
            }
            else
            {
                if(counter < times.Count - 2)
                {
                    counter++;
                    currentTime = 0;
                }
                else
                {
                    currentTime = 0;
                    counter = 0;
                    isMove = false;
                }
            }
        }


        //순서3-1. 내 턴이 아니면 return한다.
        if (GameManager.Instance.turn != myTurn || !GameManager.Instance.isNobodyMove)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            return;
        }

        //순서1-1. 조이스틱을 당긴 값을 저장한다.
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        //조이스틱을 놓는 순간 h와 v값이 0이 되기에 두 값이 0이 아닐 경우에만 temph, tempv를 갱신한다
        if (h != 0 || v != 0)
        {
            temph = h;
            tempv = v;
        }

        //순서1-2. 조이스틱을 놓는다.
        //순서1-3. 조이스틱을 놓기 직전 값을 기준으로 방향을 정한다.
        //VariableJoyStick에 구현되어 있는 OnPointerUp이 조이스틱을 떼는 순간에 작동하기에 그 함수가 실행될 때 하단에 있는 Shoot()함수를 실행시킴으로서 공의 발사정보를 호스트에게 보낸다.
        direction = Vector3.left * temph + Vector3.back * tempv;

        //순서2-1. 조이스틱을 당긴 값으로 공이 날아갈 방향을 구한다.
        Vector3 offset = new Vector3(joystick.transform.position.x - joystick.Horizontal * 12, 0, joystick.transform.position.y - joystick.Vertical * 12);

        //순서2-2. 해당 방향을 LineRenderer에 넣어준다.
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + offset);
    }

    public void Shoot()
    {
        //순서1-4. 발사 정보를 호스트에게 보낸다.
        Debug.Log("게스트 발사");
        host.GetComponent<HostCalculation>().GetGuestInformation(direction, power, transform.position, Time.time, out positions, out velocities, out times);
        isMove = true;
        Debug.Log("게스트 이동");
        //GuestBallMove(positions, velocities, times);
    }

    //public void GuestBallMove(List<Vector3> position, List<Vector3> velocity, List<float> time)
    //{
    //    float currentTime = Time.time;
    //    float setTime = currentTime;

    //    return;
    //}
}
