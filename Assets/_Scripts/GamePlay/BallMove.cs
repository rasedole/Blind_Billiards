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


//목적2 : 조이스틱을 당길 때 날아가는 방향을 표시하는 선을 그린다.
//속성2 : 선을 그릴 LineRenderer
//순서2-1. 조이스틱을 당긴 값으로 공이 날아갈 방향을 구한다.
//순서2-2. 해당 방향을 LineRenderer에 넣어준다.

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

//목적6 : 레이를 통해서 진행 방향의 끝까지 Line을 연장해서 보여준다.
//속성6 : 레이, 레이 히트 정보
//순서6-1. 조이스틱을 당긴다.
//순서6-2. 조이스틱의 방향에 따라 레이를 발사한다.
//순서6-3. 레이가 오브젝트와 닿으면 길이를 저장해서 LineRender에게 연결해준다.

public class BallMove : MonoBehaviour
{
    //속성1 : 조이스틱, 날아갈 방향, 공을 날리기 위한 공의 RigidBody, 날리는 힘
    public VariableJoystick joystick;
    public Vector3 direction;
    protected new Rigidbody rigidbody;
    public float power = 50;
    //속성추가 : 조이스틱을 놓는 순간 이벤트가 발생하는데 그러면 조이스틱의 vertical, horizontal 값이 0이 되기에 조이스틱을 놓기 직전 값을 기준으로 날아가기 위해서 tempH, tempV 사용;
    public float temph = 0;
    public float tempv = 0;

    //게스트용 추가: 호스트에게서 받아온 정보를 저장할 위치, 속도, 시간
    public List<Vector3> positions;
    public List<Vector3> velocities;
    public List<float> times;

    //속성2 : 선을 그릴 LineRenderer
    protected LineRenderer lineRenderer;

    //속성3 : 내가 조작할 수 있는 턴
    public int myTurn;

    //속성4 : MoveData
    public MoveData moveData
    {
        get
        {
            if (isMove)
            {
                _moveData.startPos = transform.position;
            }
            _moveData.ballIndex = myTurn;
            _moveData.startTime = Time.time - GameManager.Instance.shootTime;

            return _moveData;
        }
    }
    private MoveData _moveData = new();

    //공이 움직임을 체크해서 멈춘 순간에 MoveData를 전송하기 위한 변수
    public bool isMove = false;

    //속성5 : 스코어
    public int score;

    //속성6 : 레이, 레이 히트 정보
    protected Ray lineRay;
    protected RaycastHit hitinfo = new();

    // Start is called before the first frame update
    void Start()
    {
        _moveData.startPos = gameObject.transform.position;

        rigidbody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = gameObject.GetComponent<BallDoll>().showcaseColor;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        //Debug.Log(gameObject.name + "은 " + myTurn + "에 움직인다.");
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody.velocity.magnitude < 0.04f && rigidbody.velocity.magnitude != 0)
        {
            rigidbody.velocity *= 0.99f;
            rigidbody.angularVelocity *= 0.99f;
        }

        if (rigidbody.velocity.magnitude > 0.04f)
        {
            isMove = true;
        }

        if (isMove)
        {
            if (rigidbody.velocity.magnitude < 0.04f)
            {
                if (!GuestReplayer.replaying)
                {
                    GameManager.Instance.ballMoveData.Add(moveData);
                }
                isMove = false;
            }
        }

        //순서3-1. 내 턴이 아니면 return한다.
        if (GameManager.Instance.turn != myTurn || !GameManager.Instance.isNobodyMove || GuestReplayer.replaying)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            return;
        }

        //순서6-1. 조이스틱을 당긴다.
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
        //VariableJoyStick에 구현되어 있는 OnPointerUp이 조이스틱을 떼는 순간에 작동하기에 그 함수가 실행될 때 하단에 있는 Shoot()함수를 실행시킴으로서 공을 발사한다.
        direction = Vector3.left * temph + Vector3.back * tempv;

        //순서2-1. 조이스틱을 당긴 값으로 공이 날아갈 방향을 구한다.
        Vector3 offset = new Vector3(joystick.transform.position.x - joystick.Horizontal, 0, joystick.transform.position.y - joystick.Vertical);

        //순서6-2. 조이스틱의 방향에 따라 레이를 발사한다.
        lineRay = new Ray(transform.position, offset.normalized);

        //순서6-3. 레이가 오브젝트와 닿으면 길이를 저장해서 LineRender에게 연결해준다.
        int _layerMask = 1 << LayerMask.NameToLayer("Wall");
        if (Physics.Raycast(lineRay, out hitinfo, 10, _layerMask))
        {
            //순서2-2. 해당 방향을 LineRenderer에 넣어준다.
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + offset * hitinfo.distance);
        }
        else
        {
            //순서2-2. 해당 방향을 LineRenderer에 넣어준다.
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + offset * 2);
        }
    }

    public void Shoot()
    {
        rigidbody.AddForce(direction * power, ForceMode.Impulse);
    }


    //충돌이 발생할 시 MoveDate구조체를 게임데이터에게 쌓는다.
    private void OnCollisionEnter(Collision collision)
    {
        if (!GameManager.Instance.isNobodyMove)
        {
            if(!GuestReplayer.replaying)
            {
                GameManager.Instance.ballMoveData.Add(moveData);

                GetComponent<BallDoll>().CollisionEvent();
            }
        }

        if (collision.gameObject.tag is "Player")
        {
            if (GameManager.Instance.turn == myTurn)
            {
                score++;

                GameManager.Instance.UpdateScore();
            }
        }
    }
}
